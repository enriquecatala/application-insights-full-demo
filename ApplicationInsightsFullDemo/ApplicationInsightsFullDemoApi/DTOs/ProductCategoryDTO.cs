using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationInsightsFullDemoApi.DTOs
{
    public class ProductCategoryDTO
    {
        public ProductCategoryDTO()
        {
            InverseParentProductCategory = new List<ProductCategoryDTO>();
            Product = new List<ProductDTO>();
        }

        public int ProductCategoryId { get; set; }
        public int? ParentProductCategoryId { get; set; }
        public string Name { get; set; }
        public Guid Rowguid { get; set; }
        public DateTime ModifiedDate { get; set; }

        public virtual ProductCategoryDTO ParentProductCategory { get; set; }
        public virtual List<ProductCategoryDTO> InverseParentProductCategory { get; set; }
        public virtual List<ProductDTO> Product { get; set; }
    }
}
