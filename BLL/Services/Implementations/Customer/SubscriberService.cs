using BLL.Services.Interfaces.Customer;
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
    public class SubscriberService : ISubscriberService
    {
        private readonly IGenericRepository<Subscriber> _subscriberRepo;
        private readonly IGenericRepository<SentEmail> _sentEmailRepo;
        private readonly IEmailService _emailService;
        private readonly ILogger<SubscriberService> _logger;
        public SubscriberService(IGenericRepository<Subscriber> subscriberRepo,
                                    ILogger<SubscriberService> logger,
                                        IGenericRepository<SentEmail> sentEmailRepo,
                                            IEmailService emailService)
        {
            _subscriberRepo = subscriberRepo;
            _emailService = emailService;
            _sentEmailRepo = sentEmailRepo;
            _logger = logger;
        }



        public async Task<(bool Success, string Message)> SubscribeAsync(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return (false, "Please enter a valid email address.");
                }

                email = email.Trim().ToLower();

                var existingSubscriber = await _subscriberRepo.GetOneAsync(
                    s => s.Email == email
                );

                if (existingSubscriber != null)
                {
                    return (false, "You are already subscribed!");
                }

                var newSubscriber = new Subscriber { Email = email };
                await _subscriberRepo.Add(newSubscriber);

                _logger.LogInformation("New subscriber: {Email}", email);
                return (true, "Thank you for subscribing!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error subscribing email: {Email}", email);
                return (false, "An error occurred. Please try again.");
            }
        }

    }
}
