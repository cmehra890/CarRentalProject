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
    public class CustomerController : Controller
    {
        private readonly S22024Group3ProjectContext _context;

        public CustomerController(S22024Group3ProjectContext context)
        {
            _context = context;
        }

        // GET: Customer
        public async Task<IActionResult> Index()
        {
            var s22024Group3ProjectContext = _context.UserDetails.Include(u => u.AspNetUser);
            return View(await s22024Group3ProjectContext.ToListAsync());
        }

        // GET: Customer/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userDetail = await _context.UserDetails
                .Include(u => u.AspNetUser)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (userDetail == null)
            {
                return NotFound();
            }

            return View(userDetail);
        }

        // GET: Customer/Create
        public IActionResult Create()
        {
            ViewData["AspNetUserId"] = new SelectList(_context.AspNetUsers, "Id", "Id");
            return View();
        }

        // POST: Customer/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,AspNetUserId,FirstName,LastName,Address,MembershipId,DateOfBirth,CreatedDate,Town,Country")] UserDetail userDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AspNetUserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", userDetail.AspNetUserId);
            return View(userDetail);
        }

        // GET: Customer/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userDetail = await _context.UserDetails.FindAsync(id);
            if (userDetail == null)
            {
                return NotFound();
            }
            ViewData["AspNetUserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", userDetail.AspNetUserId);
            return View(userDetail);
        }

        // POST: Customer/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("UserId,AspNetUserId,FirstName,LastName,Address,MembershipId,DateOfBirth,CreatedDate,Town,Country")] UserDetail userDetail)
        {
            if (id != userDetail.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserDetailExists(userDetail.UserId))
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
            ViewData["AspNetUserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", userDetail.AspNetUserId);
            return View(userDetail);
        }

        // GET: Customer/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userDetail = await _context.UserDetails
                .Include(u => u.AspNetUser)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (userDetail == null)
            {
                return NotFound();
            }

            return View(userDetail);
        }

        // POST: Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var userDetail = await _context.UserDetails.FindAsync(id);
            if (userDetail != null)
            {
                _context.UserDetails.Remove(userDetail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserDetailExists(string id)
        {
            return _context.UserDetails.Any(e => e.UserId == id);
        }
    }
}
