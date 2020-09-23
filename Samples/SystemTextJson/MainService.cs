using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SystemTextJson
{
    public class MainService : IHostedService
    {
        readonly ILogger logger;
        readonly JsonSerializerOptions options;

        public MainService(ILogger<MainService> logger, JsonSerializerOptions options)
        {
            this.logger = logger;
            this.options = options;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var customer = new Customer
            {
                Id = CustomerId.FromInt32(123),
                Category = Category.FromString("Gold"),
                Reference = ExternalReference.FromGuid(Guid.NewGuid()),
                LastSeen = Timestamp.Now
            };
            var json = JsonSerializer.Serialize(customer, options);
            logger.LogInformation(json);
            customer = JsonSerializer.Deserialize<Customer>(json, options);
            logger.LogInformation("Id = {Id}", customer.Id);
            logger.LogInformation("Category= {Category}", customer.Category);
            logger.LogInformation("Reference = {Reference}", customer.Reference);
            logger.LogInformation("Last seen = {LastSeen}", customer.LastSeen.ToLocalTime());
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
