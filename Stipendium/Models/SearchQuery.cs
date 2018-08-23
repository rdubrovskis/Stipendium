using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Stipendium.Models
{
    public class SearchQuery
    {
        public string SearchTerm { get; set; }
        public string[] SelectedCounties { get; set; }
        public string SearchCommune { get; set; }
        public int ItemsPerPage { get; set; }
    }
}