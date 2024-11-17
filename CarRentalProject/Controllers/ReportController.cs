using CarRentalProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRentalProject.Controllers
{
    [Authorize(Roles ="Administrator")]
    public class ReportController : Controller
    {
        private S22024Group3ProjectContext _context;
        private UserManager<IdentityUser> _userManager;

        public ReportController(S22024Group3ProjectContext context,UserManager<IdentityUser> userManager) 
        { 
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Bookings()
        {
            var bookings = await _context.Bookings.ToListAsync();

            return View(bookings);
        }

        public async Task<IActionResult> Customers()
        {
            var customers = await _context.UserDetails.ToListAsync();

            return View(customers);
        }

        public async Task<IActionResult> Users()
        {
            var customerUsers = await _userManager.GetUsersInRoleAsync("Customer");

            var allUsers = await _userManager.Users.ToListAsync();

            var roledUsers = allUsers.Except(customerUsers).ToList();

            return View(roledUsers);
        }

        public async Task<IActionResult> Cars()
        {
            var cars = await _context.Cars.ToListAsync();

            return View(cars);
        }

    }
}
