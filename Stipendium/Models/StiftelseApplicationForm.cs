using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Stipendium.Models
{
    public class StiftelseApplicationForm
    {
        [Display(Name = "Förnamn")]
        [Required]
        public string FirstName { get; set; }
        [Display(Name = "Efternamn")]
        [Required]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "E-postadress")]
        public string Email { get; set; }
        [Display(Name = "Övriga kontaktuppgifter")]
        public string Other { get; set; }


        [Display(Name = "Organisations nummer")]
        public string Orgnr { get; set; }
        public string Län { get; set; }
        [Required]
        [Display(Name = "Stiftelsensnamn")]
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
        [Required]
        [Display(Name ="Förmögenhet")]
        public string Förmögenhet { get; set; }
        public string Ändamål { get; set; }
    }
}