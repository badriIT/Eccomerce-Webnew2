namespace Eccomerce_Web.Models.Admin
{
    public class Admin
    {
        public int id { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string Role { get; set; } = "Admin";
             
    }
}
