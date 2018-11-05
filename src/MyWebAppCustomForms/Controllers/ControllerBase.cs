namespace MyWebAppCustomForms.Controllers
{
    #region using directives

    using System;
    using Models;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.Security;
    using Newtonsoft.Json;

    #endregion using directives

    public abstract class ControllerBase : Controller
    {
        public UserModel CurrentUser { get; set; }

        protected override IAsyncResult BeginExecute(RequestContext requestContext, AsyncCallback callback, object state)
        {
            if (requestContext.HttpContext.User.Identity is FormsIdentity identity)
            {
                var userData = identity.Ticket.UserData;
                if (!string.IsNullOrWhiteSpace(userData))
                {
                    this.CurrentUser = JsonConvert.DeserializeObject<UserModel>(userData);
                    this.ViewBag.CurrentUser = this.CurrentUser;
                }
            }
            return base.BeginExecute(requestContext, callback, state);
        }
    }
}