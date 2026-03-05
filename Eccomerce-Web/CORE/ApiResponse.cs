using System.ComponentModel.DataAnnotations;

namespace Eccomerce_Web.CORE
{
    public class ApiResponse<T>
    {

        [Required]
        public T Data { get; set; }
    
        public int Status { get; set; }
        public string? Message { get; set; }
        
    }
}