using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Homify.Models.dto
{
    public class Rentaldto
    {
        public int rentalId { get; set; }
        public int userId { get; set; }
        public string clientName { get; set; }
        public string clientEmail { get; set; }
        public string clientContact { get; set; }
        public string clientLocation { get; set; }
        public decimal rent { get; set; }
        public string billingPeriod { get; set; }
        public string due { get; set; }

        public virtual User User { get; set; }
    }
}