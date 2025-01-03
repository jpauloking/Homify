using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Homify.Models.dto
{
    public class Propertydto
    {
        public int propertyId { get; set; }
        public int userId { get; set; }
        public decimal price { get; set; }
        public string location { get; set; }
        public string status { get; set; }
        public string plotNumber { get; set; }

        public List<Descriptionsdto> Descriptions { get; set; }
        public List<PropertyImagesdto> PropertyImages { get; set; }

    }
}