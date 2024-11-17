using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarRentalProject.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CarRentalProject.Controllers
{
    public class BookingController : Controller
    {
        private readonly S22024Group3ProjectContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public BookingController(S22024Group3ProjectContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        // GET: Booking
        //[Authorize(Roles ="Customer")]
        public async Task<IActionResult> Index()
        {
            var aspUserId = User.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            if (aspUserId == null)
            {
                return Unauthorized();
            }

            var userId = _context.UserDetails?.Where(x => x.AspNetUserId == aspUserId).Select(x => Convert.ToString(x.UserId)).FirstOrDefault();

            if (userId == null)
            {
                return NotFound();
            }

            var customerBookingList = _context.Bookings.Include(b => b.Car).Include(b => b.User).Where(x => x.UserId == userId);
            return View(await customerBookingList.ToListAsync());
        }

        // GET: Booking/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Car)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Booking/Create
        public IActionResult Create()
        {
            ViewData["CarId"] = new SelectList(_context.Cars, "CarId", "CarId");
            ViewData["UserId"] = new SelectList(_context.UserDetails, "UserId", "UserId");
            ViewData["BookingStatus_NotConfirmed"] = BookingStatus.NotConfirmed;
            return View();
        }

        // POST: Booking/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        //[Authorize(Roles = "Customer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,CarId,PickupDate,PickupTime,ReturnDate,PickupLocation,DropoffLocation,TotalCost,BookingStatus")] Booking booking)
        {
            booking.BookingStatus = BookingStatus.NotConfirmed;
            if (ModelState.IsValid)
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    var aspUserid = _contextAccessor.HttpContext!.User.Claims.FirstOrDefault(y => y.Type == ClaimTypes.NameIdentifier)!.Value;

                    booking.UserId = _context.UserDetails.Where(x => x.AspNetUserId == aspUserid).Select(x => x.UserId).FirstOrDefault();

                    _context.Add(booking);
                    await _context.SaveChangesAsync();

                    var paymentRequestObject = new Payment()
                    {
                        BookingId = booking.BookingId,
                        Amount = Convert.ToDecimal(booking.TotalCost),
                        PaymentStatus = PayementStatus.NotPaid
                    };

                    _context.Payments.Add(paymentRequestObject);
                    await _context.SaveChangesAsync();

                    transaction.Commit();

                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["CarId"] = new SelectList(_context.Cars, "CarId", "CarId", booking.CarId);
            ViewData["UserId"] = new SelectList(_context.UserDetails, "UserId", "UserId", booking.UserId);
            return View(booking);
        }

        // GET: Booking/Edit/5
        
        //[Authorize(Roles ="Customer")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aspUserId = User.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            if(aspUserId == null)
            {
                return Unauthorized();
            }

            var userId = _context.UserDetails?.FirstOrDefault(x => x.AspNetUserId == aspUserId).UserId;

            if(userId == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.Where(x => x.UserId == userId && x.BookingId == id).FirstOrDefaultAsync();
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["CarId"] = new SelectList(_context.Cars, "CarId", "CarId", booking.CarId);
            ViewData["UserId"] = new SelectList(_context.UserDetails, "UserId", "UserId", booking.UserId);
            return View(booking);
        }

        // POST: Booking/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        //[Authorize(Roles ="Customer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,UserId,CarId,PickupDate,PickupTime,ReturnDate,PickupLocation,DropoffLocation,TotalCost,BookingStatus")] Booking booking)
        {
            if (id != booking.BookingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingId))
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
            ViewData["CarId"] = new SelectList(_context.Cars, "CarId", "CarId", booking.CarId);
            ViewData["UserId"] = new SelectList(_context.UserDetails, "UserId", "UserId", booking.UserId);
            return View(booking);
        }

        // GET: Booking/Delete/5
        //[Authorize(Roles ="Customer")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aspUserId = User.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            if (aspUserId == null)
            {
                return Unauthorized();
            }

            var userId = _context.UserDetails?.FirstOrDefault(x => x.AspNetUserId == aspUserId).UserId;

            if (userId == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Car)
                .Include(b => b.User)
                .Where(x => x.UserId == userId)
                .FirstOrDefaultAsync(m => m.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Booking/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);
        }

        public IActionResult UpdatePaymentStatus(int id, [Bind("PaymentId,PaymentId,Amount,PaymentMethod,TransactionId")] Payment payment)
        {
            if(id != payment.BookingId)
            {
                return NotFound();
            }

            string paymentStatus = string.Empty;

            decimal actualAmount = _context.Bookings.Where(x => x.BookingId == payment.BookingId).Select(x => Convert.ToDecimal(x.TotalCost)).FirstOrDefault();

            decimal payedAmount = payment.Amount;

            if(actualAmount != 0)
            {

                if(actualAmount - payedAmount > 0)
                {
                    paymentStatus = PayementStatus.PartiallyPaid;

                    ////currently partially payment is not available
                    //return View();
                }
                else if(actualAmount - payedAmount == 0)
                {
                    paymentStatus = PayementStatus.Paid;
                }
                else
                {
                    paymentStatus = PayementStatus.NotPaid;
                }

                var paymentEntered = _context.Payments.Where(x => x.PaymentId == payment.PaymentId).FirstOrDefault();

                if(paymentEntered != null) 
                { 
                    paymentEntered.PaymentMethod = payment.PaymentMethod;
                    paymentEntered.Amount = payment.Amount;
                    paymentEntered.TransactionId = payment.TransactionId;

                    using (var transaction = _context.Database.BeginTransaction())
                    {
                        _context.Payments.Attach(paymentEntered);

                        _context.Entry(paymentEntered).State = EntityState.Modified;

                        _context.SaveChangesAsync();

                        //Invoice objInvoice = new Invoice()
                        //{

                        //};

                        transaction.Commit();
                    }

                }

            }

            return RedirectToAction("Index", "Home");
        }
    }
}
