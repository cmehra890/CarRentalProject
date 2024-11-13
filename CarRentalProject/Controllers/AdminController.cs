using CarRentalProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRentalProject.Controllers
{
    public class AdminController(S22024Group3ProjectContext dbContext, IHttpContextAccessor httpContextAccessor) : Controller
    {
        private readonly S22024Group3ProjectContext _dbContext = dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public IActionResult ViewAllBookings()
        {
            try
            {
                var bookings = _dbContext.Bookings.AsQueryable();

                return View(bookings);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region Manage Car

        public IActionResult AddCar(Car model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _dbContext.Cars.Add(model);
                    _dbContext.SaveChangesAsync();
                }

                return View(model);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IActionResult UpdateCarDetails(Car model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _dbContext.Cars.Attach(model);
                    _dbContext.Entry(model).State = EntityState.Modified;
                    _dbContext.SaveChangesAsync();
                }
                return View(model);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IActionResult RemoveCar(int carId)
        {
            try
            {
                var car = _dbContext.Cars.Where(x => x.CarId == carId).FirstOrDefault();

                if(car is not null)
                {
                    car.Status = CarStatus.Removed;

                    _dbContext.Cars.Attach(car);
                    _dbContext.Entry(car).State = EntityState.Modified;

                    _dbContext.SaveChangesAsync();

                }
                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IActionResult UpdateCarStatus(Car model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _dbContext.Cars.Attach(model);
                    _dbContext.Entry(model).State = EntityState.Modified;
                    _dbContext.SaveChangesAsync();
                }
                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
