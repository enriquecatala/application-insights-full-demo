using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationInsightsFullDemoApi.DTOs
{
    public class ProductDescriptionDTO
    {
        public ProductDescriptionDTO()
        {
            ProductModelProductDescription = new List<ProductModelProductDescriptionDTO>();
        }

        public int ProductDescriptionId { get; set; }
        public string Description { get; set; }
        public Guid Rowguid { get; set; }
        public DateTime ModifiedDate { get; set; }

        public virtual List<ProductModelProductDescriptionDTO> ProductModelProductDescription { get; set; }
    }
}
