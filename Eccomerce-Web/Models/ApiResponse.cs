namespace Eccomerce_Web.Models
{
    public class ApiResponse<T>
    {
        public T Data { get; set; }

        public int Status { get; set; }
        public string? Message { get; set; }
        
    }
}