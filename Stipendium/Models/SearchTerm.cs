using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Stipendium.Models
{
    public class SearchTerm
    {
        public int Id { get; set; }
        public string Term { get; set; }
        public int TimesSearched { get; set; }
        public DateTime LastSearched { get; set; }
    }
}