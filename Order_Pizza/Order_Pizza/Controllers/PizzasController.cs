using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Order_Pizza.Data;
using Order_Pizza.Models;
using Order_Pizza.Tools;
using Order_Pizza.ViewModels;

namespace Order_Pizza.Controllers
{
    [Authorize(Roles = Static_Details.ADMIN_ROLE)]
    public class PizzasController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PizzasController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Pizzas
        public async Task<IActionResult> Index()
        {
            return _context.Pizzas != null ?
                        View(await _context.Pizzas.ToListAsync()) :
                        Problem("Entity set 'AppDbContext.Pizzas'  is null.");
        }

        // GET: Pizzas/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null || _context.Pizzas == null)
        //    {
        //        return NotFound();
        //    }

        //    var pizza = await _context.Pizzas
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (pizza == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(pizza);
        //}

        // GET: Pizzas/Create
        public IActionResult Create()
        {
            PizzaVM pizzaVM = new PizzaVM()
            {
                Pizza = new()
            };
            return View(pizzaVM);
        }

        // POST: Pizzas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PizzaVM pizzavm)
        {
            if (ModelState.IsValid)
            {
                pizzavm.Pizza.Image_Url = Upload_Image(pizzavm);
                if (pizzavm.Pizza.Image_Url == null)
                {
                    ModelState.AddModelError("Image_Pizza", "Please upload an image");
                    return View(pizzavm);
                }
                _context.Add(pizzavm.Pizza);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(pizzavm);
        }

        private string Upload_Image(PizzaVM pizzaVM)
        {
            string? file_Name = null;
            if (pizzaVM.Image_Pizza != null && pizzaVM.Image_Pizza.Length > 0)
            {
                try
                {
                    file_Name = Guid.NewGuid().ToString() + Path.GetExtension(pizzaVM.Image_Pizza.FileName);
                    var uploads = Path.Combine(_webHostEnvironment.WebRootPath, @"upload/images");
                    using (var fileStrems = new FileStream(Path.Combine(uploads, file_Name), FileMode.Create))
                    {
                        pizzaVM.Image_Pizza.CopyTo(fileStrems);
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }
            return file_Name;
        }

        // GET: Pizzas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Pizzas == null)
            {
                return NotFound();
            }

            var pizza = await _context.Pizzas.FindAsync(id);
            if (pizza == null)
            {
                return NotFound();
            }
            PizzaVM pizzaVM = new PizzaVM()
            {
                Pizza = pizza
            };
            return View(pizzaVM);
        }

        // POST: Pizzas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PizzaVM pizzavm)
        {
            //if (id != pizza.Id)
            //{
            //    return NotFound();
            //}

            if (ModelState.IsValid)
            {
                try
                {
                    string file_Name = Upload_Image(pizzavm);
                    if (file_Name != null)
                    {
                        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"upload/images/" + pizzavm.Pizza.Image_Url);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                        pizzavm.Pizza.Image_Url = file_Name;
                    }
                    _context.Update(pizzavm.Pizza);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PizzaExists(pizzavm.Pizza.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(pizzavm);
        }

        // GET: Pizzas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Pizzas == null)
            {
                return NotFound();
            }

            var pizza = await _context.Pizzas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pizza == null)
            {
                return NotFound();
            }

            return View(pizza);
        }

        // POST: Pizzas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Pizzas == null)
            {
                return Problem("Entity set 'AppDbContext.Pizzas'  is null.");
            }
            var pizza = await _context.Pizzas.FindAsync(id);
            if (pizza != null)
            {
                _context.Pizzas.Remove(pizza);
                try
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"upload/images/" + pizza.Image_Url);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PizzaExists(int id)
        {
            return (_context.Pizzas?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
