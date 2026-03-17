using HotelBookingSystem.Domain.CommonNew;
using HotelBookingSystem.Domain.EnumsNew;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBookingSystem.Domain.EntitiesNew
{
    public class Tenant : BaseEntity
    {
        public string Name { get; set; } = default!;
        public TenantType Type { get; set; }
        public PropertyStatus Status { get; set; } = PropertyStatus.Pending;

        public string? Subdomain { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string CurrencyCode { get; set; } = "UZS";
        public string TimeZone { get; set; } = "Asia/Tashkent";

        public ICollection<AppUser> Users { get; set; } = new List<AppUser>();
        public ICollection<Property> Properties { get; set; } = new List<Property>();
    }
}
