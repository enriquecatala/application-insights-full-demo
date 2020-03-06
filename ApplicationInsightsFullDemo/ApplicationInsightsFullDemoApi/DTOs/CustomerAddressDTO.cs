using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationInsightsFullDemoApi.DTOs
{
    public class CustomerAddressDTO
    {
        public int CustomerId { get; set; }
        public int AddressId { get; set; }
        public string AddressType { get; set; }
        public Guid Rowguid { get; set; }
        public DateTime ModifiedDate { get; set; }

        public virtual AddressDTO Address { get; set; }
        public virtual CustomerDTO Customer { get; set; }
    }
}
