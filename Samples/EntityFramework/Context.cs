using Microsoft.EntityFrameworkCore;

namespace EntityFramework
{
    public class Context : DbContext
    {
        public Context(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>(builder =>
            {
                builder.HasKey(Customer.IdFieldName);
                builder.Property(Customer.IdFieldName).ValueGeneratedOnAdd();
                builder.Ignore(customer => customer.Id);

                builder.Property(customer => customer.Category)
                    .HasConversion(category => category.ToString(), category => Category.FromString(category));

                builder.Property(customer => customer.Reference)
                    .HasConversion(reference => reference.ToGuid(), reference => ExternalReference.FromGuid(reference));

                builder.Property(customer => customer.LastSeen)
                    .HasConversion(timestamp => timestamp.ToDateTimeOffset(), timestamp => Timestamp.FromDateTimeOffset(timestamp));
            });
        }
    }
}
