
using DynaSchoolApp.Models.Entities;
using DynaSchoolApp.Models.Models;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace DynaSchoolApp.Web.Components.Pages.Product
{
    public partial class ProductIndex
    {
        [Inject]
        public ApiClient ApiClient { get; set; }
        public List<ProductModel> ProductModels { get; set; }
        protected async override Task OnInitializedAsync()
        {
            var res = await ApiClient.GetFromJsonAsync<BaseResponseModel>("/api/Product");
            if (res != null && res.Success) {
                ProductModels = JsonConvert.DeserializeObject<List<ProductModel>>(res.Data.ToString());
            }

            await base.OnInitializedAsync();
        }
    }
}
