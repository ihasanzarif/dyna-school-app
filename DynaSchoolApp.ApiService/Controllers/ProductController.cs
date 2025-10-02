using DynaSchoolApp.BL.Services;
using DynaSchoolApp.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DynaSchoolApp.ApiService.Controllers
{
    [Route("school/[controller]")]
    [ApiController]
    [Authorize(Roles = "Staff", Policy = "GatewayOnly")]
    public class ProductController(IProductService productService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<BaseResponseModel>> GetProducts()
        {
            var products = await productService.GetProducts();
            return Ok(new BaseResponseModel { Success = true, Data = products});
        }
    }
}
