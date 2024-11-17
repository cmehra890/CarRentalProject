using CarRentalProject.Data;
using CarRentalProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CarRentalProject.Controllers
{
    //[Authorize(Roles = "Administrator")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _applicationContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(ApplicationDbContext applicationContext,
                              UserManager<IdentityUser> userManager,
                              RoleManager<IdentityRole> roleManager)
        {
            _applicationContext = applicationContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public ActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            ViewData["Roles"] = new SelectList(_roleManager.Roles.ToList(), "Name", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Email,UserName,Role")] AuthorizedUserViewModel model)
        {

            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    Email = model.Email,
                    UserName = model.UserName
                };

                await _userManager.CreateAsync(user, "Password123!");

                await _userManager.AddToRoleAsync(user, model.Role ?? "Visitor");

                return RedirectToAction(nameof(Index));
            }
            ViewData["Roles"] = new SelectList(_roleManager.Roles.ToList(), "Name", "Name", model.Role);
            return View(model);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
                return RedirectToAction("Index");
            }
            return NotFound();
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            if (user != null)
            {
                var userSpecific = new AuthorizedUserViewModel
                {
                    Id = id,
                    Email = user.Email,
                    UserName = user.UserName,
                };
                ViewData["Roles"] = new SelectList(_roleManager.Roles.ToList(), "Name", "Name", userRole);
                return View(userSpecific);
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Email,UserName,Role")] AuthorizedUserViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.FindByIdAsync(model.Id);
                    if (user is not null)
                    {

                        var userRoles = (await _userManager.GetRolesAsync(user)).ToArray();

                        await _userManager.SetEmailAsync(user, model.Email);

                        await _userManager.RemoveFromRolesAsync(user, userRoles);

                        await _userManager.AddToRoleAsync(user, model.Role);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

    }
}
