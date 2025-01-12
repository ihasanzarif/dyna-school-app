using DynaSchoolApp.BL.Repositories;
using DynaSchoolApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynaSchoolApp.BL.Services
{
    public interface IProductService
    {
        Task<List<ProductModel>> GetProducts();
    }

    public class ProductService(IProductRepository productRepository) : IProductService
    {
        public Task<List<ProductModel>> GetProducts()
        {
            return productRepository.GetProducts();
        }
        

    }
}
