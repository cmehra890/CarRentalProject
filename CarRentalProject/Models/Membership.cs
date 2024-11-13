using System;
using System.Collections.Generic;

namespace CarRentalProject.Models;

public partial class Membership
{
    public int UserMembershipId { get; set; }

    public string? UserId { get; set; }

    public int? TierId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly ExpiryDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual MembershipTier? Tier { get; set; }

    public virtual UserDetail? User { get; set; }
}
