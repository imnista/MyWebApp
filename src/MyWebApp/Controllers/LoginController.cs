namespace MyWebApp.Controllers
{
    #region using directives

    using Models;
    using System;
    using System.Web.Mvc;
    using System.Web.Security;

    #endregion using directives

    [AllowAnonymous]
    public class LoginController : Controller
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
                    FormsAuthentication.SetAuthCookie(model.Name, model.IsRememberMe);

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