using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Homify.Models.dto
{
    public class PropertyImagesdto
    {
        public int imageId { get; set; }
        public int propertyId { get; set; }
        public string imagePath { get; set; }
    }
}