using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationInsightsFullDemoApi.DTOs
{
    public class ProductModelDTO
    {
        public ProductModelDTO()
        {
            Product = new List<ProductDTO>();
            ProductModelProductDescription = new List<ProductModelProductDescriptionDTO>();
        }

        public int ProductModelId { get; set; }
        public string Name { get; set; }
        public string CatalogDescription { get; set; }
        public Guid Rowguid { get; set; }
        public DateTime ModifiedDate { get; set; }

        public virtual List<ProductDTO> Product { get; set; }
        public virtual List<ProductModelProductDescriptionDTO> ProductModelProductDescription { get; set; }
    }
}
