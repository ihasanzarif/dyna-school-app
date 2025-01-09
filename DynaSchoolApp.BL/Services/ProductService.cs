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
        Task<List<Product>> GetProducts();
    }

    public class ProductService(IProductRepository productRepository) : IProductService
    {
        public Task<List<Product>> GetProducts()
        {
            return productRepository.GetProducts();
        }
        

    }
}
