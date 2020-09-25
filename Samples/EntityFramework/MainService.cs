using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFramework
{
    public class MainService : IHostedService
    {
        readonly ILogger logger;
        readonly IServiceScopeFactory serviceScopeFactory;

        public MainService(ILogger<MainService> logger, IServiceScopeFactory serviceScopeFactory)
        {
            this.logger = logger;
            this.serviceScopeFactory=serviceScopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await ExecuteScopeAsync(CreateDatabaseAsync);
            var id = await ExecuteScopeAsync(context => CreateCustomerAsync(context, "Gold", "Alpha", "Beta"));
            await ExecuteScopeAsync(context => CreateCustomerAsync(context, "Silver"));
            await ExecuteScopeAsync(GetAllCustomersAsync);
            await ExecuteScopeAsync(context => GetCustomerByIdAsync(context, id));
            await ExecuteScopeAsync(context => GetCustomerByCategoryAsync(context, "Gold"));
            await ExecuteScopeAsync(context => GetCustomerWithEagerLoadedOrdersAsync(context, id));
            await ExecuteScopeAsync(context => GetCustomerWithExplicitlyLoadedOrdersAsync(context, id));
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        async Task CreateDatabaseAsync(Context context)
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }

        async Task<CustomerId> CreateCustomerAsync(Context context, Category category, params string[] orderDescriptions)
        {
            var customer = new Customer
            {
                Category = category,
                Reference = Guid.NewGuid(),
                LastSeen = DateTimeOffset.Now
            };
            context.Customers.Add(customer);
            logger.LogInformation("Creating customer: category = {Category}, reference = {Reference}, last seen = {LastSeen}", customer.Category, customer.Reference, customer.LastSeen);
            foreach (var orderDescription in orderDescriptions)
            {
                var order = new Order
                {
                    Customer = customer,
                    Description = orderDescription
                };
                context.Orders.Add(order);
            }
            await context.SaveChangesAsync();
            return customer.Id;
        }

        async Task GetAllCustomersAsync(Context context)
        {
            logger.LogInformation("Retrieving all customers");
            var customers = await context.Customers.ToListAsync();
            foreach (var customer in customers)
                logger.LogInformation("Customer: id = {Id}, category = {Category}, reference = {Reference}, last seen = {LastSeen}", customer.Id, customer.Category, customer.Reference, customer.LastSeen);
        }

        async Task GetCustomerByCategoryAsync(Context context, Category category)
        {
            logger.LogInformation("Retrieving customers in category {Category}", category);
            var customers = await context.Customers.Where(customer => customer.Category == category).ToListAsync();
            foreach (var customer in customers)
                logger.LogInformation("Customer: id = {Id}, category = {Category}, reference = {Reference}, last seen = {LastSeen}", customer.Id, customer.Category, customer.Reference, customer.LastSeen);
        }

        async Task GetCustomerByIdAsync(Context context, CustomerId id)
        {
            logger.LogInformation("Retrieving customers with id {CustomerId}", id);
            var providerId = id.ToInt32();
            var customers = await context.Customers.Where(customer => EF.Property<int>(customer, Customer.IdFieldName) == providerId).ToListAsync();
            foreach (var customer in customers)
                logger.LogInformation("Customer: id = {Id}, category = {Category}, reference = {Reference}, last seen = {LastSeen}", customer.Id, customer.Category, customer.Reference, customer.LastSeen);
        }

        async Task GetCustomerWithEagerLoadedOrdersAsync(Context context, CustomerId id)
        {
            logger.LogInformation("Retrieving customer with id {CustomerId} and related orders eagerly", id);
            var providerId = id.ToInt32();
            var customer = await context.Customers.Include(customer => customer.Orders).Where(customer => EF.Property<int>(customer, Customer.IdFieldName) == providerId).FirstAsync();
            logger.LogInformation("Customer: id = {Id}, category = {Category}, reference = {Reference}, last seen = {LastSeen}", customer.Id, customer.Category, customer.Reference, customer.LastSeen);
            foreach (var order in customer.Orders!)
                logger.LogInformation("Order: id = {Id}, description = {Description}", order.Id, order.Description);
        }

        async Task GetCustomerWithExplicitlyLoadedOrdersAsync(Context context, CustomerId id)
        {
            logger.LogInformation("Retrieving customer with id {CustomerId} and related orders explicitly", id);
            var providerId = id.ToInt32();
            var customer = await context.Customers.Where(customer => EF.Property<int>(customer, Customer.IdFieldName) == providerId).FirstAsync();
            logger.LogInformation("Customer: id = {Id}, category = {Category}, reference = {Reference}, last seen = {LastSeen}", customer.Id, customer.Category, customer.Reference, customer.LastSeen);
            await context.Entry(customer).Collection(customer => customer.Orders).LoadAsync();
            foreach (var order in customer.Orders!)
                logger.LogInformation("Order: id = {Id}, description = {Description}", order.Id, order.Description);
        }

        async Task ExecuteScopeAsync(Func<Context, Task> action)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<Context>();
            await action(context);
        }

        async Task<T> ExecuteScopeAsync<T>(Func<Context, Task<T>> action)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<Context>();
            return await action(context);
        }
    }
}
