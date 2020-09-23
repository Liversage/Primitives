namespace SystemTextJson
{
    public class Customer
    {
        public CustomerId Id { get; set; }
        public Category Category { get; set; }
        public ExternalReference Reference { get; set; }
        public Timestamp LastSeen { get; set; }
    }
}
