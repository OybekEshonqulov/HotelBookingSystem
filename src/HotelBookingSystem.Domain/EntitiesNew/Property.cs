using HotelBookingSystem.Domain.CommonNew;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBookingSystem.Domain.EntitiesNew
{
    public class Property : TenantEntity
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public bool IsPublished { get; set; }
        public Tenant Tenant { get; set; } = default!;
        public ICollection<RoomType> RoomTypes { get; set; } = new List<RoomType>();
        public ICollection<PropertyImage> Images { get; set; } = new List<PropertyImage>();
        public ICollection<PropertyReview> Reviews { get; set; } = new List<PropertyReview>();

    }
}
