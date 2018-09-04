using System.Data.Entity;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Stipendium.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Registered since")]
        public DateTime RegistrationDate { get { return DateTime.Now; } }
        public DateTimeOffset LastActivityDate { get; set; }



        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class PrivateUser : ApplicationUser
    {
        [Display(Name ="Förnamn")]
        public string FirstName { get; set; }
        [Display(Name = "Efternamn")]
        public string LastName { get; set; }
        public string Address { get; set; }
        [Display(Name ="Post Nr.")]
        public string PostNr { get; set; }
        [Display(Name = "Ort")]
        public string City { get; set; }
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Telefon Nr")]
        public override string PhoneNumber { get; set; }

    }

    public class CompanyUser : ApplicationUser
    {
        public virtual Stiftelse Stiftelse { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        object placeHolderVariable;

        public System.Data.Entity.DbSet<Stipendium.Models.Stiftelse> Stiftelses { get; set; }

        public System.Data.Entity.DbSet<Stipendium.Models.PrivateUser> ApplicationUsers { get; set; }

        public System.Data.Entity.DbSet<Stipendium.Models.Pageviews> Pageviews { get; set; }

        public System.Data.Entity.DbSet<Stipendium.Models.SearchTerm> SearchTerms { get; set; }
    }
}