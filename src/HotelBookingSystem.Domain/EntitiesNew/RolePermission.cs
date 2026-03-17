using HotelBookingSystem.Domain.CommonNew;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBookingSystem.Domain.EntitiesNew
{
    public class RolePermission : BaseEntity
    {
        public Guid RoleId { get; set; }
        public AppRole Role { get; set; } = default!;

        public Guid PermissionId { get; set; }
        public Permission Permission { get; set; } = default!;
    }
}
