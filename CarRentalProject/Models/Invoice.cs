using System;
using System.Collections.Generic;

namespace CarRentalProject.Models;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public int? BookingId { get; set; }

    public DateTime? GeneratedDate { get; set; }

    public decimal? Amount { get; set; }

    public decimal? DueAmount { get; set; }

    public decimal? LateFees { get; set; }

    public decimal? DamageCharges { get; set; }

    public string? CarConditionAtReturn { get; set; }

    public virtual Booking? Booking { get; set; }
}
