using System.ComponentModel.DataAnnotations;

namespace Eccomerce_Web.Common.Dtos.Responses
{
    public class ApiResponse<T>
    {

        [Required]
        public T Data { get; set; }
        public int Status { get; set; }
        public string? Message { get; set; }



        public static ApiResponse<T> Created(T data, string? message = "Created")
        {
            return new ApiResponse<T>
            {
                Data = data,
                Status = 201,
                Message = message
            };
        }

        public static ApiResponse<T> BadRequest(string? message = "Bad Request")
        {
            return new ApiResponse<T>
            {
                Data = default,
                Status = 400,
                Message = message
            };
        }

        public static ApiResponse<T> Unauthorized(string? message = "Unauthorized")
        {
            return new ApiResponse<T>
            {
                Data = default,
                Status = 401,
                Message = message
            };
        }

        public static ApiResponse<T> Forbidden(string? message = "Forbidden")
        {
            return new ApiResponse<T>
            {
                Data = default,
                Status = 403,
                Message = message
            };
        }

        public static ApiResponse<T> NotFound(string? message = "Not Found")
        {
            return new ApiResponse<T>
            {
                Data = default,
                Status = 404,
                Message = message
            };
        }
    }
}