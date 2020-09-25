using System.Collections.Generic;

namespace EntityFramework
{
    public class Customer
    {
        internal const string IdFieldName = nameof(id);

        int id;

        public CustomerId Id
        {
            get => CustomerId.FromInt32(id);
            set => id = value.ToInt32();
        }

        public Category Category { get; set; }
        public ExternalReference Reference { get; set; }
        public Timestamp LastSeen { get; set; }
        public List<Order>? Orders { get; set; }
    }
}
