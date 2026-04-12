using HotelBookingSystem.Domain.CommonNew;
using HotelBookingSystem.Domain.EnumsNew;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBookingSystem.Domain.EntitiesNew
{
    public class Bed : TenantEntity
    {
        public Guid RoomId { get; set; }
        public Room Room { get; set; } = default!;
        public bool IsPublished { get; set; }
        public string BedCode { get; set; } = default!;
        public decimal? BedPrice { get; set; }

        public BedStatus Status { get; set; } = BedStatus.Available;

    }
}
