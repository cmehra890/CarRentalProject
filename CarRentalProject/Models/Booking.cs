using System;
using System.Collections.Generic;

namespace CarRentalProject.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public string? UserId { get; set; }

    public int? CarId { get; set; }

    public DateOnly PickupDate { get; set; }

    public TimeOnly PickupTime { get; set; }

    public DateOnly ReturnDate { get; set; }

    public string? PickupLocation { get; set; }

    public string? DropoffLocation { get; set; }

    public decimal? TotalCost { get; set; }

    public string BookingStatus { get; set; } = null!;

    public virtual Car? Car { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual UserDetail? User { get; set; }
}
