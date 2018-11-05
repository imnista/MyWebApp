namespace MyWebAppCustomForms.Controllers
{
    #region using directives

    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;
    using Models;
    using Newtonsoft.Json;

    #endregion

    [AllowAnonymous]
    public class LoginController : ControllerBase
    {
        public ActionResult Index()
        {
            return this.View();
        }

        [ValidateAntiForgeryToken]
        public ActionResult Login(UserModel model, string returnUrl)
        {
            if (this.ModelState.IsValid)
            {
                model.Name = model.Name.Trim();

                if (model.Name != "nobody" /* 检查密码等，各种逻辑 */)
                {
                    //FormsAuthentication.SetAuthCookie(model.Name, model.IsRememberMe);
                    SetAuthCookie(model.Name, model.IsRememberMe, model);

                    return this.CheckReturnUrl(returnUrl)
                        ? this.Redirect(returnUrl)
                        : this.RedirectToAction("Index", "Home") as ActionResult;
                }
                this.ModelState.AddModelError("", "请检查你的用户名或密码");
            }
            return this.View("Index", model);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return this.RedirectToAction("Index");
        }

        private static void SetAuthCookie(
            string userName,
            bool createPersistentCookie,
            object userData)
        {
            if (!System.Web.HttpContext.Current.Request.IsSecureConnection && FormsAuthentication.RequireSSL)
            {
                throw new HttpException("Connection not secure creating secure cookie");
            }

            // <!> In this way, we will lose the function of cookieless
            //var flag = UseCookieless(
            //    System.Web.HttpContext.Current,
            //    false,
            //    FormsAuthentication.CookieMode);

            FormsAuthentication.Initialize();
            if (userName == null)
            {
                userName = string.Empty;
            }
            var cookiePath = FormsAuthentication.FormsCookiePath;
            var utcNow = DateTime.UtcNow;
            var expirationUtc = utcNow + FormsAuthentication.Timeout;
            var authenticationTicket = new FormsAuthenticationTicket(
                2,
                userName,
                utcNow.ToLocalTime(),
                expirationUtc.ToLocalTime(),
                createPersistentCookie,
                JsonConvert.SerializeObject(userData),
                cookiePath
            );

            var encryptedTicket = FormsAuthentication.Encrypt(authenticationTicket);
            if (string.IsNullOrEmpty(encryptedTicket))
            {
                throw new HttpException("Unable to encrypt cookie ticket");
            }
            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
            {
                HttpOnly = true,
                Path = cookiePath,
                Secure = FormsAuthentication.RequireSSL
            };

            if (FormsAuthentication.CookieDomain != null)
            {
                authCookie.Domain = FormsAuthentication.CookieDomain;
            }
            if (authenticationTicket.IsPersistent)
            {
                authCookie.Expires = authenticationTicket.Expiration;
            }

            System.Web.HttpContext.Current.Response.Cookies.Add(authCookie);
        }

        private bool CheckReturnUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return false;
            }
            // Make Sure the return url was not redirect to an external site.
            if (Uri.TryCreate(url, UriKind.Absolute, out var absoluteUri))
            {
                return string.Equals(
                    this.Request.Url.Host,
                    absoluteUri.Host, StringComparison.OrdinalIgnoreCase);
            }
            return url[0] == '/' && (url.Length == 1
                                     || url[1] != '/' && url[1] != '\\')
                   || url.Length > 1 && url[0] == '~' && url[1] == '/';
        }
    }
}