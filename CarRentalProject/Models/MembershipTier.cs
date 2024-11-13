using System;
using System.Collections.Generic;

namespace CarRentalProject.Models;

public partial class MembershipTier
{
    public int TierId { get; set; }

    public string TierName { get; set; } = null!;

    public decimal DiscountRate { get; set; }

    public virtual ICollection<Membership> Memberships { get; set; } = new List<Membership>();
}
