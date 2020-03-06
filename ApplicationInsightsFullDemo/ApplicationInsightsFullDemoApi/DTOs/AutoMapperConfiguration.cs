using ApplicationInsightsFullDemoApi.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationInsightsFullDemoApi.DTOs
{
    public class AutoMapperConfiguration: Profile
    {
        public AutoMapperConfiguration()
        {

            // To avoid circular references, i´m ignoring navegation contexts
            CreateMap<Address, AddressDTO>()
                   .ForMember(x => x.CustomerAddress, o => o.Ignore())
                   .ForMember(x => x.SalesOrderHeaderBillToAddress, o => o.Ignore())
                   .ForMember(x => x.SalesOrderHeaderShipToAddress, o => o.Ignore())
                   .ReverseMap();

            CreateMap<BuildVersion,BuildVersionDTO>().ReverseMap();
            CreateMap<CustomerAddress, CustomerAddressDTO>().ReverseMap();
            CreateMap<Customer, CustomerDTO>()
                .ForMember(x => x.CustomerAddress, o => o.Ignore())
                .ForMember(x => x.SalesOrderHeader, o => o.Ignore())
                .ReverseMap();
            CreateMap<ErrorLog, ErrorLogDTO>().ReverseMap();
            CreateMap<ProductCategory, ProductCategoryDTO>()
                .ForMember(x => x.ParentProductCategory, o => o.Ignore())
                .ForMember(x => x.InverseParentProductCategory, o => o.Ignore())
                .ForMember(x => x.Product, o => o.Ignore())
                .ReverseMap();
            CreateMap<ProductDescription, ProductDescriptionDTO>()
                .ForMember(x => x.ProductModelProductDescription, o => o.Ignore())
                .ReverseMap();
            CreateMap<Product, ProductDTO>()
                .ForMember(x => x.ProductCategory, o => o.Ignore())
                .ForMember(x => x.ProductModel, o => o.Ignore())
                .ForMember(x => x.SalesOrderDetail, o => o.Ignore())
                .ReverseMap();
            CreateMap<ProductModel, ProductModelDTO>()
                .ForMember(x => x.Product, o => o.Ignore())
                .ForMember(x => x.ProductModelProductDescription, o => o.Ignore())
                .ReverseMap();
            CreateMap<ProductModelProductDescription, ProductModelProductDescriptionDTO>().ReverseMap();
            CreateMap<SalesOrderDetail, SalesOrderDetailDTO>().ReverseMap();
            CreateMap<SalesOrderHeader, SalesOrderHeaderDTO>()
                .ForMember(x => x.SalesOrderDetail, o => o.Ignore())
                .ReverseMap();
            CreateMap<VGetAllCategories, VGetAllCategoriesDTO>().ReverseMap();
            CreateMap<VProductAndDescription, VProductAndDescriptionDTO>().ReverseMap();
            CreateMap<VProductModelCatalogDescription, VProductModelCatalogDescriptionDTO>().ReverseMap();

        }
    }
}
