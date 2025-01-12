using DynaSchoolApp.Database.Data;
using DynaSchoolApp.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynaSchoolApp.BL.Repositories
{
    public interface IProductRepository
    {
        Task<List<ProductModel>> GetProducts();
    }

    public class ProductRepository(AppDbContext dbContext) : IProductRepository
    {
        Task<List<ProductModel>> IProductRepository.GetProducts()
        {
            return dbContext.Products.ToListAsync();
        }
    }
}
