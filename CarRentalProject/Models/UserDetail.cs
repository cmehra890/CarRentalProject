using System;
using System.Collections.Generic;

namespace CarRentalProject.Models;

public partial class UserDetail
{
    public string? UserId { get; set; }

    public string? AspNetUserId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Address { get; set; }

    public int? MembershipId { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? Town { get; set; }

    public string? Country { get; set; }

    public virtual AspNetUser? AspNetUser { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Membership> Memberships { get; set; } = new List<Membership>();
}
