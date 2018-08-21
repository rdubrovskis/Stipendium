using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Stipendium.Models
{
    public class Municipality
    {
        public string MunicipalityName { get; set; }
        public int PostIndexLow { get; set; }
        public int PostIndexHigh { get; set; }
    }
}