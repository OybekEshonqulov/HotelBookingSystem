using HotelBookingSystem.Domain.CommonNew;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBookingSystem.Domain.EntitiesNew
{
    public class RoomType : TenantEntity
    {
        public Guid PropertyId { get; set; }
        public Property Property { get; set; } = default!;

        public string Name { get; set; } = default!;
        public int Capacity { get; set; }
        public decimal BasePrice { get; set; }

        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
