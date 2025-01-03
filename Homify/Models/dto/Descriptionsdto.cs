using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Homify.Models.dto
{
    public class Descriptionsdto
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