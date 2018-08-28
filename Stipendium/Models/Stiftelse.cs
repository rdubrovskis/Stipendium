using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Stipendium.Models
{
    public class Stiftelse
    {
        public int Id { get; set; }
        public string Stiftelsenr { get; set; }
        public string Aktnr { get; set; }
        public string Orgnr { get; set; }
        public string Län { get; set; }
        public string Stiftelsenamn { get; set; }
        public string Kommun { get; set; }
        public string Adress { get; set; }
        public string CoAdress { get; set; }
        public string Postnr { get; set; }
        public string Postadress { get; set; }
        public string Telefon { get; set; }
        public string Stiftelsetyp { get; set; }
        public string Status { get; set; }
        public string År { get; set; }
        public string Förmögenhet { get; set; }
        public string Ändamål { get; set; }
    }
}