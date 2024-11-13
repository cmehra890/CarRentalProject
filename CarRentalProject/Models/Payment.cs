using System;
using System.Collections.Generic;

namespace CarRentalProject.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int? BookingId { get; set; }

    public decimal Amount { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string? PaymentMethod { get; set; }

    public string? TransactionId { get; set; }

    public string PaymentStatus { get; set; } = null!;

    public virtual Booking? Booking { get; set; }
}
