using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBookingSystem.Domain.CommonNew
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
