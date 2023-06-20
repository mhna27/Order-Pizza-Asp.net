using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Order_Pizza.Data;
using Order_Pizza.Models;
using Order_Pizza.ViewModels;
using System.Security.Claims;

namespace Order_Pizza.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public OrderController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //[AllowAnonymous]
        public IActionResult Index()
        {
            //IEnumerable<Pizza> pizzas = _context.Pizzas;
            //return View(pizzas);
            var pizzas = _context.Pizzas.ToList();
            var orders = _context.Orders.ToList();
            var viewModel = new PizzaOrderVM
            {
                Pizzas = pizzas.ToPizzaViewModels(orders, _userManager.GetUserId(User))
            };
            return View(viewModel);
        }

        //Get
        public async Task<IActionResult> Upsert(int? pizza_Id, int? order_Id)
        {
            if (pizza_Id == null)
            {
                return NotFound();
            }
            var pizza = await _context.Pizzas.FindAsync(pizza_Id);
            if (pizza == null)
            {
                return NotFound();
            }

            if (order_Id == null || order_Id == 0)
            {
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                return View(new OrderVM()
                {
                    Pizza = pizza,
                    Order = new() { User_Id = claim.Value }
                });
            }
            else
            {
                var order = await _context.Orders.FindAsync(order_Id);
                if (order == null)
                {
                    return NotFound();
                }
                return View(new OrderVM()
                {
                    Pizza = pizza,
                    Order = order
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(OrderVM orderVM)
        {
            if (ModelState.IsValid)
            {
                if (orderVM.Order.Id == 0)
                {
                    //await orderVM.Order.User_Id = _userManager.GetUserIdAsync(User.Identity);
                    _context.Add(orderVM.Order);
                    TempData["success"] = "Your order has been successfully placed.";
                }
                else
                {
                    _context.Update(orderVM.Order);
                    TempData["success"] = "Your order has been edited successfully.";
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(orderVM);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }
            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }
            var pizza = await _context.Pizzas
                .FirstOrDefaultAsync(m => m.Id == order.Pizza_Id);
            if (pizza == null)
            {
                return NotFound();
            }
            return View(new OrderVM()
            {
                Pizza = pizza,
                Order = order
            });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'AppDbContext.Orders'  is null.");
            }
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }
            await _context.SaveChangesAsync();
            TempData["success"] = "Your order has been successfully cancelled.";
            return RedirectToAction(nameof(Index));
        }
    }
}
