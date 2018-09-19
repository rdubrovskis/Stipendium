using ClosedXML.Excel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SendGrid;
using SendGrid.Helpers.Mail;
using Stipendium.Models;
using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace Stipendium.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext db = new ApplicationDbContext();

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (User.IsInRole("Admin"))
            {
                returnUrl = "~/Admin/Index";
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user != null)
            {
                if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                {
                    string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, "Confirm your account-Resend");
                    ViewBag.errorMessage = "Du måste bekräfta din e-post adress innan du kan logga in";
                    return View("Error");
                }
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    model.LastActivityDate = DateTime.Now;
                    user = db.Users.Where(u => u.Email == model.Email).FirstOrDefault();
                    user.LastActivityDate = DateTime.Now;
                    db.SaveChanges();
                    if (user.Roles.Count() > 0 && user.Roles.Single().RoleId == db.Roles.Single(r => r.Name == "Admin").Id)
                    {
                        return RedirectToLocal("~/Admin/Index");
                    }
                    else
                    {
                        return RedirectToLocal(returnUrl);
                    }
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return PartialView();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SendConfirmationEmail(user);
                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult RegisterPrivate()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterPrivate(RegisterPrivateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new PrivateUser { UserName = model.Email, Email = model.Email, Address = model.Address, City = model.City, FirstName = model.FirstName, LastName = model.LastName, PhoneNumber = model.PhoneNumber, PostNr = model.PostNr, LastActivityDate = DateTime.Now };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SendConfirmationEmail(user);
                    ViewBag.Message = "Kontrollera din e-post (" + user.Email + ") och bekräfta ditt registrerade konto.";

                    return View("Info");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult RegisterCompany()
        {
            return PartialView();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterCompany(RegisterCompanyViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new CompanyUser { UserName = model.Email, Email = model.Email, LastActivityDate = DateTime.Now };
                user.Stiftelse = new Stiftelse
                {
                    Adress = model.Stiftelse.Adress,
                    Aktnr = model.Stiftelse.Aktnr,
                    CoAdress = model.Stiftelse.CoAdress,
                    Förmögenhet = model.Stiftelse.Förmögenhet,
                    Kommun = model.Stiftelse.Kommun,
                    Län = model.Stiftelse.Län,
                    Orgnr = model.Stiftelse.Orgnr,
                    Postadress = model.Stiftelse.Postadress,
                    Postnr = model.Stiftelse.Postnr,
                    Stiftelsenamn = model.Stiftelse.Stiftelsenamn,
                    Stiftelsenr = model.Stiftelse.Stiftelsenr,
                    Telefon = model.Stiftelse.Telefon,
                    Ändamål = model.Stiftelse.Ändamål,
                    År = model.Stiftelse.År,
                };

                db.SaveChanges();

                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SendConfirmationEmail(user);
                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "E-post bekräftad" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Återställ lösenord", "Klicka <a href=\"" + callbackUrl + "\">här</a> för att återställa ditt lösenord");
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            user.LastActivityDate = DateTime.Now;
            db.SaveChanges();
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        public async Task SendConfirmationEmail(ApplicationUser user)
        {
            //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

            string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);

            XmlDocument doc = new XmlDocument();
            string strAppPath = AppDomain.CurrentDomain.BaseDirectory;
            doc.Load(strAppPath + "\\ConfirmationEmail.xml");


            string msgSubject = "Välkommen till Stiftelseverket!";
            string msgBody = doc.SelectSingleNode("/Email/Body").InnerText;

            var callbackUrl = Url.Action("ConfirmEmail", "Account",
               new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
            await UserManager.SendEmailAsync(user.Id,
               msgSubject, "<p>Välkommen som ny medlem hos Stiftelseverket.</p>" + "<br />" + "<a href=\""
               + callbackUrl + "\">Klicka här för att bekräfta din e-postadress som du angav vid registering</a><br/><p>Lycka till med ditt sökande.</p><p>Stiftelseverket</p>");
        }

        private async Task<string> SendEmailConfirmationTokenAsync(string userID, string subject)
        {
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(userID);
            var callbackUrl = Url.Action("ConfirmEmail", "Account",
               new { userId = userID, code = code }, protocol: Request.Url.Scheme);
            await UserManager.SendEmailAsync(userID, subject,
               "Klicka <a href=\"" + callbackUrl + "\">här</a> för att bekräfta ditt e-post");

            return callbackUrl;
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                switch (error)
                {
                    case ("Passwords must have at least one digit ('0'-'9')."):
                        ModelState.AddModelError("", "Lösenord måste ha minst en siffra ('0' - '9').");
                        break;
                    case ("Passwords must have at least one uppercase ('A'-'Z')."):
                        ModelState.AddModelError("", "Lösenord måste ha minst en stor bokstav ('A'-'Z').");
                        break;
                    case ("Passwords must have at least one digit ('0'-'9'). Passwords must have at least one uppercase ('A'-'Z')."):
                        ModelState.AddModelError("", "Lösenord måste ha minst en siffra ('0'-'9'). Lösenord måste ha minst en stor bokstav ('A'-'Z').");
                        break;
                    default:
                        ModelState.AddModelError("", error);
                        break;
                }

            }
        }

        public ActionResult Resend(string userid)
        {
            if (userid != "")
            {
                string code = UserManager.GenerateEmailConfirmationToken(userid);
                var callbackUrl = Url.Action("ConfirmEmail", "Account",
                    new { userId = userid, code }, protocol: Request.Url.Scheme);
                UserManager.SendEmail(userid, "E-mail confirmation",
               "Klicka <a href=\"" + callbackUrl + "\">här</a> för att bekräfta ditt e-post");
                return new JsonResult { Data = "success", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            return View("Error");
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        [AllowAnonymous]
        public ActionResult StiftelseApplication()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> StiftelseApplicationAsync(StiftelseApplicationForm model, string other)
        {
            if (ModelState.IsValid)
            {
                string body = "<h3>New stiftelse application from: " + model.FirstName + " " + model.LastName + " (<a href=\"mailto: " + model.Email + "\">" + model.Email + "</a>)</h3>" +
                    "<p>Alternate contact: " + other + "</p>" +
                    "<br />" +
                    "<p>Org. nr: " + model.Orgnr + "</p>" +
                    "<p>Län: " + model.Län + "</p>" +
                    "<p>Stiftelsenamn: " + model.Stiftelsenamn + "</p>" +
                    "<p>Kommun: " + model.Kommun + "</p>" +
                    "<p>Adress: " + model.Adress + "</p>" +
                    "<p>C/o adress: " + model.CoAdress + "</p>" +
                    "<p>Postnummer: " + model.Postnr + "</p>" +
                    "<p>Postadress: " + model.Postadress + "</p>" +
                    "<p>Telefon: " + model.Telefon + "</p>" +
                    "<p>Stifteslsetyp: " + model.Stiftelsetyp + "</p>" +
                    "<p>År: " + model.År + "</p>" +
                    "<p>Förmögenhet: " + model.Förmögenhet + "</p>" +
                    "<p>Ändamål: " + model.Ändamål + "</p>";

                var administrators = db.Users.Where(u => u.Roles.FirstOrDefault().RoleId == db.Roles.FirstOrDefault(s => s.Name == "Admin").Id);
                var attachmentsource = GenerateApplication(model);

                foreach (var admin in administrators)
                {
                    var from = new EmailAddress("applications@stiftelseverket.se");
                    var to = new EmailAddress(admin.Email);


                    var msg = MailHelper.CreateSingleEmail(from, to, "Ny stiftelseapplikation", body, body);

                    byte[] bytes = System.IO.File.ReadAllBytes(attachmentsource);
                    string file = Convert.ToBase64String(bytes);
                    var attachment = new SendGrid.Helpers.Mail.Attachment { Filename = model.FirstName+"_"+model.LastName+"_"+DateTime.Now.ToShortDateString()+".xlsx", Content = file, Type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" };
                    msg.AddAttachment("applikation.xls", file);

                    var client = new SendGridClient(ConfigurationManager.AppSettings["SendGridAPIKey"]);
                    await client.SendEmailAsync(msg);

                    //await UserManager.SendEmailAsync(admin.Id, "Ny Stiftelseapplikation", body);

                }
                System.IO.File.Delete(attachmentsource);

                ViewBag.Title = "Tack för din ansökan";
                ViewBag.Message = "Vår administration kommer att kontakta dig så snart som möjligt för att verifiera din lämnade information.";
                return View("Info");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private string GenerateApplication(StiftelseApplicationForm form)
        {
            var folder = Server.MapPath("~/App_Data/");
            var oldPath = folder + "EntryTemplate.xlsx";
            var newPath = folder + form.FirstName + "_" + form.LastName + "_" + DateTime.Now.ToShortDateString() + ".xlsx";
            //System.IO.File.Copy(oldPath, newPath);



            //var result = reader.AsDataSet(new ExcelDataSetConfiguration() { ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration() { UseHeaderRow = true } });
            var workbook = new XLWorkbook(oldPath);
            var ws = workbook.Worksheet(1);
            var rngData = ws.Range("A1:P2");
            var xCeltable = rngData.AsTable();
            var u = ws.Columns().Count();

            xCeltable.Field("Stiftelsenamn").Column.Cell(2).Value = form.Stiftelsenamn;
            xCeltable.Field("Org.nr").Column.Cell(2).Value = form.Orgnr;
            xCeltable.Field("Kommun").Column.Cell(2).Value = form.Kommun;
            xCeltable.Field("Adress").Column.Cell(2).Value = form.Adress;
            xCeltable.Field("C/o adress").Column.Cell(2).Value = form.CoAdress;
            xCeltable.Field("Postnr").Column.Cell(2).Value = form.Postnr;
            xCeltable.Field("Postadress").Column.Cell(2).Value = form.Postadress;
            xCeltable.Field("Telefon").Column.Cell(2).Value = form.Telefon;
            xCeltable.Field("Stiftelsetyp").Column.Cell(2).Value = form.Stiftelsetyp;
            xCeltable.Field("År").Column.Cell(2).Value = form.År;
            xCeltable.Field("Förmögenhet").Column.Cell(2).Value = form.Förmögenhet;
            xCeltable.Field("Ändamål").Column.Cell(2).Value = form.Ändamål;
            xCeltable.Field("Län").Column.Cell(2).Value = form.Län;

            var t = ws.Columns().Count();
            ws.Column(100).Delete(); //File is saved with 1025 columns where there were 1024 for some reason, which throws a warning in some readers. This is to avoid that.
            workbook.SaveAs(newPath);

            return newPath;
        }
        #endregion
    }
}


//using (var reader = ExcelReaderFactory.CreateReader(stream))
//{
//    var result = reader.AsDataSet(new ExcelDataSetConfiguration() { ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration() { UseHeaderRow = true } });

//    var table = result.Tables[0];
//    var newRow = table.Rows[1];

//    newRow["Org.nr"] = form.Orgnr;

//    newRow["Län"] = form.Län;
//    newRow["Stiftelsenamn"] = form.Stiftelsenamn;
//    newRow["Kommun"] = form.Kommun;
//    newRow["Adress"] = form.Adress;
//    newRow["C/o adress"] = form.CoAdress;
//    newRow["Postnr"] = form.Postnr;
//    newRow["Postadress"] = form.Postadress;
//    newRow["Telefon"] = form.Telefon;
//    newRow["Stiftelsetyp"] = form.Stiftelsetyp;
//    newRow["År"] = form.År;
//    newRow["Förmögenhet"] = form.Förmögenhet;
//    newRow["Ändamål"] = form.Ändamål;

//    table.Rows[1].BeginEdit();
//    table.Rows[1]

//    table.AcceptChanges();
//    result.Tables[0].AcceptChanges();
//    result.AcceptChanges();

//}