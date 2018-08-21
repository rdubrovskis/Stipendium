using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Stipendium.Models
{
    public class County
    {
        [StringLength(50, MinimumLength = 5)]
        public string CountyName { get; set; }
        public IEnumerable<Municipality> Municipalities { get; set; }
    }
}