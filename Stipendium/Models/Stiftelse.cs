using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Stipendium.Models
{
    public class Stiftelse
    {
        public int Id { get; set; }
        [Display(Name ="Stiftelse nr.")]
        public string Stiftelsenr { get; set; }
        [Display(Name = "Aktnummer")]
        public string Aktnr { get; set; }
        [Display(Name = "Organisations nummer")]
        public string Orgnr { get; set; }
        public string Län { get; set; }
        [Display(Name = "Stiftelse Namn")]
        public string Stiftelsenamn { get; set; }
        public string Kommun { get; set; }
        public string Adress { get; set; }
        [Display(Name = "C/o Adress")]
        public string CoAdress { get; set; }
        [Display(Name = "Postnummer")]
        public string Postnr { get; set; }
        [Display(Name = "Ort")]
        public string Postadress { get; set; }
        public string Telefon { get; set; }
        public string Stiftelsetyp { get; set; }
        public string Status { get; set; }
        public string År { get; set; }
        [DataType(DataType.Currency)]
        public string Förmögenhet { get; set; }
        public string Ändamål { get; set; }

        public DateTime DateAdded { get; set; }
    }
}