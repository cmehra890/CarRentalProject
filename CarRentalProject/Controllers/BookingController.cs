using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarRentalProject.Models;
using System.Security.Claims;

namespace CarRentalProject.Controllers
{
    public class BookingController : Controller
    {
        private readonly S22024Group3ProjectContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public BookingController(S22024Group3ProjectContext context,IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        // GET: Booking
        public async Task<IActionResult> Index()
        {
            var s22024Group3ProjectContext = _context.Bookings.Include(b => b.Car).Include(b => b.User);
            return View(await s22024Group3ProjectContext.ToListAsync());
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,CarId,PickupDate,PickupTime,ReturnDate,PickupLocation,DropoffLocation,TotalCost,BookingStatus")] Booking booking)
        {
            booking.BookingStatus = BookingStatus.NotConfirmed;
            if (ModelState.IsValid)
            {
                booking.UserId = _context.UserDetails.Where(x => x.AspNetUserId == _contextAccessor.HttpContext!
                                                                                                   .User
                                                                                                   .Claims
                                                                                                   .FirstOrDefault(y => y.Type == ClaimTypes.NameIdentifier)!.Value)
                                                     .Select(x => x.UserId).FirstOrDefault();

                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarId"] = new SelectList(_context.Cars, "CarId", "CarId", booking.CarId);
            ViewData["UserId"] = new SelectList(_context.UserDetails, "UserId", "UserId", booking.UserId);
            return View(booking);
        }

        // GET: Booking/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);
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
        public async Task<IActionResult> Delete(int? id)
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
    }
}
