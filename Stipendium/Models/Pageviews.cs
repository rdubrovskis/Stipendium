using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Stipendium.Models
{
    public class Pageviews
    {
        public int Id { get; set; }
        public virtual Stiftelse Stiftelse { get; set; }
        public int ViewCount { get; set; } = 0;
    }
}