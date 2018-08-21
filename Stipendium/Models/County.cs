using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Stipendium.Models
{
    public class County
    {
        public string CountyName { get; set; }
        public IEnumerable<Municipality> Municipalities { get; set; }
    }
}