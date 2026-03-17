using HotelBookingSystem.Domain.CommonNew;
using HotelBookingSystem.Domain.EnumsNew;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBookingSystem.Domain.EntitiesNew
{
    public class Room : TenantEntity
    {
        public Guid PropertyId { get; set; }
        public Property Property { get; set; } = default!;

        public Guid RoomTypeId { get; set; }
        public RoomType RoomType { get; set; } = default!;

        public string Number { get; set; } = default!;
        public int Floor { get; set; }
        public RoomStatus Status { get; set; } = RoomStatus.Available;

        public ICollection<Bed> Beds { get; set; } = new List<Bed>();

        public ICollection<RoomImage> Images { get; set; } = new List<RoomImage>();

    }
}
