using Castle.Core.Smtp;
using DAL.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Implementations
{
    public class NewsletterService : INewsletterService
    {
        private readonly IGenericRepository<Subscriber> _subscriberRepo;
        private readonly IGenericRepository<SentEmail> _sentEmailRepo;
        private readonly IEmailService _emailSender;
        private readonly ILogger<NewsletterService> _logger;

        public NewsletterService(IGenericRepository<Subscriber> subscriberRepo,
                                     IGenericRepository<SentEmail> sentEmailRepo,
                                         IEmailService emailSender,
                                             ILogger<NewsletterService> logger)
        {
            _subscriberRepo = subscriberRepo;
            _sentEmailRepo = sentEmailRepo;
            _emailSender = emailSender;
            _logger = logger;
        }

        public async Task<PaginatedList<Subscriber>> GetSubscribersAsync(string query, int page, int pageSize)
        {
            var baseQuery = _subscriberRepo.GetAll();

            if (!string.IsNullOrEmpty(query))
            {
                baseQuery = baseQuery.Where(s =>
                    s.Email.Contains(query) ||
                    s.SubscribedAt.ToString("yyyy-MM-dd").Contains(query)
                );
            }

            var orderedQuery = baseQuery.OrderByDescending(s => s.SubscribedAt);
            return await PaginatedList<Subscriber>.CreateAsync(orderedQuery, page, pageSize);
        }

        public async Task<(bool Success, string Message)> SendNewsletterAsync(string subject, string body)
        {
            try
            {
                var subscribers = await _subscriberRepo.GetAll().ToListAsync();
                var successCount = 0;

                foreach (var subscriber in subscribers)
                {
                    try
                    {
                        await _emailSender.SendEmailAsync(subscriber.Email, subject, body);
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send email to {Email}", subscriber.Email);
                    }
                }

                if (successCount == 0)
                {
                    return (false, "Failed to send emails to any subscribers.");
                }

                var sentEmail = new SentEmail
                {
                    Subject = subject,
                    Body = body,
                    SentAt = DateTime.Now
                };

                await _sentEmailRepo.Add(sentEmail);

                return (true, $"Email sent successfully to {successCount} out of {subscribers.Count} subscribers.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending newsletter");
                return (false, "An error occurred while sending the newsletter. Please try again.");
            }
        }

        public async Task<List<SentEmail>> GetEmailHistoryAsync()
        {
            return await _sentEmailRepo.GetAll()
                .OrderByDescending(e => e.SentAt)
                .ToListAsync();
        }


    }
}