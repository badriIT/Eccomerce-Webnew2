using Eccomerce_Web.Common.Dtos.Responses;

namespace Eccomerce_Web.Modules.Products.Dtos.Response
{
    public class ProductsResponse <T> : ApiResponse <T>
    {
       

        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
    }
}
