using System;
using System.Collections.Generic;
using System.Text;

namespace ConcurrentThreadsBenchmark.Models
{
    public class ShippingAddress
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public AddressType AddressType { get; set; }
        public bool IsBillingAddress { get; set; }
    }
}
