using HotelBookingSystem.Domain.CommonNew;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBookingSystem.Domain.EntitiesNew
{
    public class AppUser : TenantEntity
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public bool IsActive { get; set; } = true;
        public DateTime? LastLoginAtUtc { get; set; }

        public Tenant Tenant { get; set; } = default!;
        public ICollection<AppUserRole> UserRoles { get; set; } = new List<AppUserRole>();
    }
}
