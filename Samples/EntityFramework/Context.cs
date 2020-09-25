using Microsoft.EntityFrameworkCore;

namespace EntityFramework
{
    public class Context : DbContext
    {
        public Context(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; } = null!;

        public DbSet<Order> Orders { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // This ain't pretty...

            modelBuilder.Entity<Customer>(builder =>
            {
                builder.UseFieldInsteadOfPropertyAsKey(Customer.IdFieldName, nameof(Customer.Id));

                builder.Property(customer => customer.Category)
                    .HasConversion(category => category.ToString(), category => Category.FromString(category));

                builder.Property(customer => customer.Reference)
                    .HasConversion(reference => reference.ToGuid(), reference => ExternalReference.FromGuid(reference));

                builder.Property(customer => customer.LastSeen)
                    .HasConversion(timestamp => timestamp.ToDateTimeOffset(), timestamp => Timestamp.FromDateTimeOffset(timestamp));

                builder.HasMany(customer => customer.Orders)
                    .WithOne()
                    .HasForeignKey(Order.CustomerIdFieldName);
            });

            modelBuilder.Entity<Order>(builder =>
            {
                builder.UseFieldInsteadOfPropertyAsKey(Order.IdFieldName, nameof(Order.Id));

                builder.UseFieldInsteadOfProperty(Order.CustomerIdFieldName, nameof(Order.CustomerId));

                builder.HasOne(order => order.Customer!)
                    .WithMany(customer => customer.Orders)
                    .HasForeignKey(Order.CustomerIdFieldName);
            });
        }
    }
}
