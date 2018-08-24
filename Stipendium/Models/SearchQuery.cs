using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;

namespace Stipendium.Models
{
    public class SearchQuery
    {
        public string SearchTerm { get; set; }
        public string[] SelectedCounties { get; set; }
        public string SearchMunicipality { get; set; }
        public int ItemsPerPage { get; set; }
        public int Page { get; set; }
        public IPagedList<Stipend> SearchResults { get; set; }
    }
}