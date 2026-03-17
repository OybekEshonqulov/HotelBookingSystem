using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBookingSystem.Domain.CommonNew
{
    public abstract class TenantEntity : BaseEntity
    {
        public Guid TenantId { get; set; }
    }
}
