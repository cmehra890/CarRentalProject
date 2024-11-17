using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarRentalProject.Models;

namespace CarRentalProject.Controllers
{
    public class InvoicesController : Controller
    {
        private readonly S22024Group3ProjectContext _context;

        public InvoicesController(S22024Group3ProjectContext context)
        {
            _context = context;
        }

        // GET: Invoices
        public async Task<IActionResult> Index()
        {
            var s22024Group3ProjectContext = _context.Invoices.Include(i => i.Booking);
            return View(await s22024Group3ProjectContext.ToListAsync());
        }

        // GET: Invoices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices
                .Include(i => i.Booking)
                .FirstOrDefaultAsync(m => m.InvoiceId == id);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // GET: Invoices/Create
        //public IActionResult Create()
        //{
        //    ViewData["BookingId"] = new SelectList(_context.Bookings, "BookingId", "BookingId");
        //    return View();
        //}

        // POST: Invoices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("InvoiceId,BookingId,GeneratedDate,Amount,DueAmount,LateFees,DamageCharges,CarConditionAtReturn")] Invoice invoice)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(invoice);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["BookingId"] = new SelectList(_context.Bookings, "BookingId", "BookingId", invoice.BookingId);
        //    return View(invoice);
        //}

        // GET: Invoices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }

            var currentBooking = _context.Bookings.Where(b => b.BookingId == invoice.BookingId);

            ViewData["BookingId"] = new SelectList(currentBooking, "BookingId", "BookingId", invoice.BookingId);
            return View(invoice);
        }

        // POST: Invoices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InvoiceId,BookingId,GeneratedDate,Amount,DueAmount,LateFees,DamageCharges,CarConditionAtReturn")] Invoice invoice)
        {
            if (id != invoice.InvoiceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(invoice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InvoiceExists(invoice.InvoiceId))
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
            ViewData["BookingId"] = new SelectList(_context.Bookings, "BookingId", "BookingId", invoice.BookingId);
            return View(invoice);
        }

        // GET: Invoices/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var invoice = await _context.Invoices
        //        .Include(i => i.Booking)
        //        .FirstOrDefaultAsync(m => m.InvoiceId == id);
        //    if (invoice == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(invoice);
        //}

        //// POST: Invoices/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var invoice = await _context.Invoices.FindAsync(id);
        //    if (invoice != null)
        //    {
        //        _context.Invoices.Remove(invoice);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool InvoiceExists(int id)
        {
            return _context.Invoices.Any(e => e.InvoiceId == id);
        }
    }
}
