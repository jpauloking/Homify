//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Homify.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Description
    {
        public int description1 { get; set; }
        public int propertyId { get; set; }
        public string landType { get; set; }
        public string size { get; set; }
        public string houseType { get; set; }
        public Nullable<int> bedRooms { get; set; }
        public string parking { get; set; }
        public Nullable<int> bathRooms { get; set; }
        public Nullable<System.DateTime> YearBuilt { get; set; }
        public string Amentities { get; set; }
    
        public virtual Property Property { get; set; }
    }
}
