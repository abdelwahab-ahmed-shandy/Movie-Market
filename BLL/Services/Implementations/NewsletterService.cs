using Castle.Core.Smtp;
using DAL.Repositories.IRepositories;
using DAL.ViewModels.Subscriber;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MovieMart.Models;
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

        public async Task<SubscriberListVM> GetSubscribersAsync(string query, int page, int pageSize)
        {
            var source = _subscriberRepo.GetAll();

            if (!string.IsNullOrEmpty(query))
            {
                source = source.Where(s => s.Email.Contains(query));
            }

            var paginatedList = await PaginatedList<Subscriber>.CreateAsync(source, page, pageSize);

            return new SubscriberListVM
            {
                Subscribers = paginatedList.Select(s => new SubscriberVM
                {
                    Id = s.Id,
                    Email = s.Email,
                    SubscribedAt = s.SubscribedAt
                }),
                PageIndex = paginatedList.PageIndex,
                PageSize = pageSize,
                TotalCount = paginatedList.TotalCount,
                SearchTerm = query
            };
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



        public async Task DeleteSubscriberAsync(Guid id)
        {
            var subscriber = await _subscriberRepo.GetByIdAsync(id);
            if (subscriber == null)
                throw new Exception("Subscriber not found");

            await _subscriberRepo.DeleteInDB(id);
        }

    }
}