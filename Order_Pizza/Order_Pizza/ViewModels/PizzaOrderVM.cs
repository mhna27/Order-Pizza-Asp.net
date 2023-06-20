using Order_Pizza.Models;

namespace Order_Pizza.ViewModels
{
    public class PizzaOrderVM
    {
        public IEnumerable<PizzaViewModel> Pizzas { get; set; }
    }
    public class PizzaViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image_Url { get; set; }
        public bool IsOrdered { get; set; }
        public int? OrderId { get; set; }
    }

    public static class PizzaViewModelExtensions
    {
        public static IEnumerable<PizzaViewModel> ToPizzaViewModels(this IEnumerable<Pizza> pizzas, IEnumerable<Order> orders, string user_Id)
        {
            return pizzas.Select(p => new PizzaViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Image_Url = p.Image_Url,
                IsOrdered = orders.Any(o => o.Pizza_Id == p.Id && o.User_Id == user_Id),
                OrderId = orders.FirstOrDefault(o => o.Pizza_Id == p.Id && o.User_Id == user_Id)?.Id,
            });
        }
    }
}
