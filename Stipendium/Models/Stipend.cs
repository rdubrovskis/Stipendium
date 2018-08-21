using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Stipendium.Models
{
    public class Stipend
    {
        public string ID { get; set; }

        public string Title { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public int PostNr { get; set; }
        public string County { get; set; }
        public string ContactInfo { get; set; }
        public string OrgNr { get; set; }
        public string Description { get; set; }
        public decimal Capital { get; set; }
        public bool AcceptsApplications { get; set; }
    }
}