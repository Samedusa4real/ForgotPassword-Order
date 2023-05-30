using Microsoft.AspNetCore.Identity;

namespace PustokTemplate.Models
{
    public class AppUser:IdentityUser
    {
        public string FulName { get; set; }
        public bool IsAdmin { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

    }
}
