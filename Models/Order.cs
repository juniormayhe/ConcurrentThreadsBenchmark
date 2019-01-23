using System;
using System.Collections.Generic;
using System.Text;

namespace ConcurrentThreadsBenchmark.Models
{
    public class Order
    {
        public int Id { get; set; }
        public ShippingAddress ShippingAddress { get; set; }
        public DateTime Created { get; set; }
        public IEnumerable<OrderItem> OrderItems { get; set; }
}
}
