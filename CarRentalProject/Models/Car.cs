using System;
using System.Collections.Generic;

namespace CarRentalProject.Models;

public partial class Car
{
    public int CarId { get; set; }

    public string Make { get; set; } = null!;

    public string Model { get; set; } = null!;

    public int Year { get; set; }

    public string? Type { get; set; }

    public string LicensePlate { get; set; } = null!;

    public decimal PricePerDay { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
