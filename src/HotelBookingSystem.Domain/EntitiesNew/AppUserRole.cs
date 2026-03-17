using HotelBookingSystem.Domain.CommonNew;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBookingSystem.Domain.EntitiesNew
{
    public class AppUserRole : BaseEntity
    {
        public Guid UserId { get; set; }
        public AppUser User { get; set; } = default!;

        public Guid RoleId { get; set; }
        public AppRole Role { get; set; } = default!;
    }
}
