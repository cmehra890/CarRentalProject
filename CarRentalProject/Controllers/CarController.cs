using CarRentalProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarRentalProject.Controllers
{
    public class CarController(S22024Group3ProjectContext dbContext, IHttpContextAccessor httpContextAccessor) : Controller
    {
        private readonly S22024Group3ProjectContext _dbContext = dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public IActionResult ViewCarDetails(int carId)
        {
            Car? car = new();
            try
            {
                if(carId is not 0)
                {
                    car = _dbContext.Cars.Where(x => x.CarId == carId).FirstOrDefault();
                }
                return View(car);
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public IActionResult ViewBookingDetails(int bookingId)
        {
            Tuple<Booking, Payment>? responseObject = null;
            try
            {
                if (bookingId is not 0)
                {
                    var requestedBookingObject = _dbContext.Bookings.Where(x => x.BookingId == bookingId).FirstOrDefault();
                    var requestedPaymentObject = _dbContext.Payments.Where(x => x.BookingId == bookingId).FirstOrDefault();

                    responseObject = Tuple.Create<Booking, Payment>(requestedBookingObject ?? new Booking(), requestedPaymentObject ?? new Payment());

                }
                return View(responseObject);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
