using CarRentalProject.Data;
using CarRentalProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace CarRentalProject.Controllers
{
    public class HomeController(ILogger<HomeController> logger, S22024Group3ProjectContext dbContext, IHttpContextAccessor httpContextAccessor) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly S22024Group3ProjectContext _dbContext = dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public IActionResult Index()
        {
            var cars = _dbContext.Cars.AsQueryable<Car>();

            return View(cars);
        }

        public IActionResult FirstLogin(UserDetail model)
        {
            // Action to set users personal details
            try
            {
                /*
                 required parameters
                --------------------------------
                firstname
                lastname -optional
                address
                membershipid
                data of birth
                town
                country
                 */

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                model.AspNetUserId = _httpContextAccessor.HttpContext?
                                                         .User?
                                                         .Claims?
                                                         .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?
                                                         .Value;

                model.CreatedDate = DateTime.Now;

                _dbContext.UserDetails.Add(model);
                _dbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
