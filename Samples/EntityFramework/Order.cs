namespace EntityFramework
{
    public class Order
    {
        internal const string IdFieldName = nameof(id);
        internal const string CustomerIdFieldName = nameof(customerId);

        int id;

        public OrderId Id
        {
            get => OrderId.FromInt32(id);
            set => id = value.ToInt32();
        }

        int customerId;

        public CustomerId CustomerId
        {
            get => CustomerId.FromInt32(id);
            set => customerId = value.ToInt32();
        }

        public Customer? Customer { get; set; }

        public string? Description { get; set;}
    }
}
