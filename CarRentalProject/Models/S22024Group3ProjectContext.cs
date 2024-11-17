using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using CarRentalProject.Models;

namespace CarRentalProject.Models;

public partial class S22024Group3ProjectContext : DbContext
{
    public S22024Group3ProjectContext()
    {
    }

    public S22024Group3ProjectContext(DbContextOptions<S22024Group3ProjectContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Car> Cars { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<Membership> Memberships { get; set; }

    public virtual DbSet<MembershipTier> MembershipTiers { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<UserDetail> UserDetails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("data source=citizen.manukautech.info,6304;initial catalog=S22024_Group3_Project;User Id=S22024_Group3;password=fBit$66267;TrustServerCertificate=True;MultipleActiveResultSets=true;Integrated Security=False;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("AspNetUserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.ProviderKey).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.Name).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Bookings__73951AEDF182FC05");

            entity.Property(e => e.BookingStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Confirmed");
            entity.Property(e => e.DropoffLocation).HasMaxLength(100);
            entity.Property(e => e.PickupLocation).HasMaxLength(100);
            entity.Property(e => e.TotalCost).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Car).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.CarId)
                .HasConstraintName("FK__Bookings__CarId__4F47C5E3");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Bookings__UserId__4E53A1AA");
        });

        modelBuilder.Entity<Car>(entity =>
        {
            entity.HasKey(e => e.CarId).HasName("PK__Cars__68A0342E289D6E85");

            entity.HasIndex(e => e.LicensePlate, "UQ__Cars__026BC15CC9B4C43B").IsUnique();

            entity.Property(e => e.LicensePlate).HasMaxLength(20);
            entity.Property(e => e.Make).HasMaxLength(50);
            entity.Property(e => e.Model).HasMaxLength(50);
            entity.Property(e => e.PricePerDay).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Available");
            entity.Property(e => e.Type).HasMaxLength(20);
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__Invoices__D796AAB5E35FFA55");

            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CarConditionAtReturn).HasMaxLength(255);
            entity.Property(e => e.DamageCharges).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.DueAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.GeneratedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LateFees).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Booking).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__Invoices__Bookin__5AB9788F");
        });

        modelBuilder.Entity<Membership>(entity =>
        {
            entity.HasKey(e => e.UserMembershipId).HasName("PK__Membersh__5A4E736A6F25D4D9");

            entity.ToTable("Membership");

            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Tier).WithMany(p => p.Memberships)
                .HasForeignKey(d => d.TierId)
                .HasConstraintName("FK__Membershi__TierI__45BE5BA9");

            entity.HasOne(d => d.User).WithMany(p => p.Memberships)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Membershi__UserI__44CA3770");
        });

        modelBuilder.Entity<MembershipTier>(entity =>
        {
            entity.HasKey(e => e.TierId).HasName("PK__Membersh__362F561D2E771C0B");

            entity.Property(e => e.DiscountRate).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.TierName).HasMaxLength(20);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A3816D364D0");

            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PaymentMethod).HasMaxLength(20);
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.TransactionId).HasMaxLength(50);

            entity.HasOne(d => d.Booking).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__Payments__Bookin__540C7B00");
        });

        modelBuilder.Entity<UserDetail>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C2A0677F5");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.AspNetUserId).HasMaxLength(450);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Town).HasMaxLength(100);

            entity.HasOne(d => d.AspNetUser).WithMany(p => p.UserDetails)
                .HasForeignKey(d => d.AspNetUserId)
                .HasConstraintName("FK__Users__AspNetUse__40058253");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

public DbSet<CarRentalProject.Models.AuthorizedUserViewModel> AuthorizedUserViewModel { get; set; } = default!;
}
