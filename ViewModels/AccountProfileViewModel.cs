using PustokTemplate.Models;

namespace PustokTemplate.ViewModels
{
    public class AccountProfileViewModel
    {
        public ProfileEditViewModel Profile { get; set; }
        public List<Order> Orders { get; set; }
    }
}
