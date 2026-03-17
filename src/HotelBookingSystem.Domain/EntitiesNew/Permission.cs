using HotelBookingSystem.Domain.CommonNew;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBookingSystem.Domain.EntitiesNew
{
    public class Permission : BaseEntity
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? Description { get; set; }

        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
