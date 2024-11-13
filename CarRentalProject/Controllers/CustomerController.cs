using CarRentalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CarRentalProject.Controllers
{
    public class CustomerController(S22024Group3ProjectContext dbContext, IHttpContextAccessor httpContextAccessor) : Controller
    {
        private readonly S22024Group3ProjectContext _dbContext = dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        #region Booking section

        public IActionResult AddBooking(Booking model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(model);
                }

                var aspUserId = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                if (aspUserId is not null)
                {
                    var userId = _dbContext.UserDetails.FirstOrDefault(x => x.AspNetUserId == aspUserId)?.UserId;

                    if (userId is not null)
                    {
                        using (var transaction = _dbContext.Database.BeginTransaction())
                        {
                            model.BookingStatus = BookingStatus.UpComming;

                            _dbContext.Bookings.Add(model);
                            _dbContext.SaveChangesAsync();

                            var paymentRequestObject = new Payment()
                            {
                                BookingId = model.BookingId,
                                Amount = Convert.ToDecimal(model.TotalCost),
                                PaymentStatus = PayementStatus.NotPaid
                            };

                            _dbContext.Payments.Add(paymentRequestObject);
                            _dbContext.SaveChangesAsync();

                            transaction.Commit();

                            return RedirectToAction("/*Message_panel_link*/", new Message() { MessageText = "Booked Successfully!", PreviousPage = "Booking" });
                        }
                    }
                }

                return View(model);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IActionResult CancelBooking(int bookingId)
        {
            try
            {
                if (bookingId is not 0)
                {
                    using (var transaction = _dbContext.Database.BeginTransaction())
                    {
                        var requestedBookingObject = _dbContext.Bookings.Where(x => x.BookingId == bookingId).FirstOrDefault();

                        if (requestedBookingObject is not null)
                        {
                            requestedBookingObject.BookingStatus = BookingStatus.Cancelled;
                            _dbContext.SaveChangesAsync();
                        }

                        var requestedPaymentObject = _dbContext.Payments.Where(x => x.BookingId == bookingId).FirstOrDefault();

                        if (requestedPaymentObject is not null)
                        {
                            requestedPaymentObject.PaymentStatus = PayementStatus.Cancelled;
                            _dbContext.SaveChangesAsync();
                        }

                        transaction.Commit();

                        return RedirectToAction("/*message_panel*/", new Message { MessageText = "Booking cancelled successfuly!", PreviousPage = "booking" });
                    }

                }

                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IActionResult ViewCustomerAllBooking()
        {
            IQueryable<Booking>? allBookingList = null;
            try
            {
                var currentLoggedUserId = _httpContextAccessor.HttpContext?
                                                              .User
                                                              .Claims
                                                              .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?
                                                              .Value;

                if (currentLoggedUserId is not null)
                {
                    var currentUserId = _dbContext.UserDetails.Where(x => x.AspNetUserId == currentLoggedUserId)
                                                              .Select(x => x.UserId)
                                                              .FirstOrDefault();

                    if (currentUserId is not null)
                    {
                        var allBookings = _dbContext.Bookings.Where(x => x.UserId == currentUserId);

                        if (allBookings is not null)
                        {
                            allBookingList = allBookings;
                        }
                    }
                }
                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Personal details section
        public IActionResult ViewPersonalDetails()
        {
            UserDetail userDetail = new();
            try
            {
                var currentLoggedUserId = _httpContextAccessor.HttpContext?.User
                                                                           .Claims
                                                                           .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?
                                                                           .Value;

                if (currentLoggedUserId is not null)
                {
                    var currentUserDetails = _dbContext.UserDetails.Where(x => x.AspNetUserId == currentLoggedUserId)
                                                                   .FirstOrDefault();

                    if (currentUserDetails is not null)
                    {
                        userDetail = currentUserDetails;
                    }
                }
                return View(userDetail);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IActionResult UpdatePersonalDetails(UserDetail model)
        {
            try
            {
                if (model is not null)
                {
                    _dbContext.UserDetails.Attach(model);

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
        #endregion

        #region MemberShip Section

        public IActionResult GetAllAvailableMemberships()
        {
            IQueryable<MembershipTier>? membershipTiers = null;
            try
            {
                var availableMembershipTiers = _dbContext.MembershipTiers.AsQueryable<MembershipTier>();

                if (availableMembershipTiers is not null)
                {
                    membershipTiers = availableMembershipTiers;
                }

                return View(membershipTiers);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IActionResult GetCustomerCurrentMembership()
        {
            Membership? activeMemberShip = null;
            try
            {

                var currentLoggedUserId = _httpContextAccessor.HttpContext?.User
                                                                          .Claims
                                                                          .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?
                                                                          .Value;

                if (currentLoggedUserId is not null)
                {
                    var membership = _dbContext.UserDetails.Where(x => x.AspNetUserId == currentLoggedUserId)
                                                       .Join(_dbContext.Memberships, x1 => x1.UserId, x2 => x2.UserId, (x1, x2) => new { x1, x2 })
                                                       .Where(t => Convert.ToBoolean(t.x2.IsActive))
                                                       .Select(t => t.x2)
                                                       .FirstOrDefault();

                    if (membership is not null)
                    {
                        activeMemberShip = membership;
                    }

                }

                return View(activeMemberShip);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IActionResult PurchaseMembership(Membership model)
        {
            try
            {
                if (model is not null)
                {
                    var currentLoggedUserId = _httpContextAccessor.HttpContext?.User
                                                                           .Claims
                                                                           .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?
                                                                           .Value;

                    if (currentLoggedUserId is not null)
                    {
                        var currentUserId = _dbContext.UserDetails.Where(x => x.AspNetUserId == currentLoggedUserId)
                                                                  .Select(x => x.UserId)
                                                                  .FirstOrDefault();

                        if (currentUserId is not null)
                        {
                            using (var transaction = _dbContext.Database.BeginTransaction())
                            {
                                var existingActiveMemberShip = _dbContext.Memberships.Where(x => x.UserId == currentUserId && x.IsActive == true)
                                                                                     .Select(x => new Membership
                                                                                     {
                                                                                         Tier = x.Tier,
                                                                                         TierId = x.TierId,
                                                                                         UserMembershipId = x.UserMembershipId,
                                                                                         UserId = x.UserId,
                                                                                         StartDate = x.StartDate,
                                                                                         ExpiryDate = x.ExpiryDate,
                                                                                         IsActive = false
                                                                                     });
                                _dbContext.Memberships.AttachRange(existingActiveMemberShip);

                                _dbContext.Entry(existingActiveMemberShip).State = EntityState.Modified;

                                _dbContext.SaveChangesAsync();

                                model.UserId = currentUserId;

                                _dbContext.Memberships.Add(model);

                                _dbContext.SaveChangesAsync();

                                transaction.Commit();
                            }
                        }
                    }
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
