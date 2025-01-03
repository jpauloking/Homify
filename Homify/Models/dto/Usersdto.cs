using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Homify.Models.dto
{
    public class Usersdto
    {
        public int userId { get; set; }
        public string userName { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string passWord { get; set; }
        public string emailContact { get; set; }
        public List<Propertydto> Properties { get; set; }
    }
}