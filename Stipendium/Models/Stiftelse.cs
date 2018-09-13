using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stipendium.Models
{
    public class Stiftelse
    {
        public int Id { get; set; }
        [Display(Name ="Stiftelse nr.")]
        [Required]
        public string Stiftelsenr { get; set; }
        [Required]
        [Display(Name = "Aktnummer")]
        public string Aktnr { get; set; }
        [Display(Name = "Organisations nummer")]
        public string Orgnr { get; set; }
        public string Län { get; set; }
        [Required]
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
        [Required]
        public string Förmögenhet { get; set; }
        public string Ändamål { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DateCreated { get; set; }
        public DateTime? LastModified { get; set; }
    }
}