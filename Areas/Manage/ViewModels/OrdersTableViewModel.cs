using PustokTemplate.Models;
using PustokTemplate.ViewModels;

namespace PustokTemplate.Areas.Manage.ViewModels
{
    public class OrdersTableViewModel
    {
        public PaginatedList<Order> PaginatedListOrder { get; set; }
        public Order Order { get; set; }

    }
}
