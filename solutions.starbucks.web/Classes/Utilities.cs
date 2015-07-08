using solutions.starbucks.Interfaces;
using solutions.starbucks.Model.Pocos;
using System;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace solutions.starbucks.web.Classes
{

    public class Utilities { 

        public class Training
        {
         
            public static string AccessLink(Invites invite) {

                var request = HttpContext.Current.Request;
                var url = request.Url.Scheme + "://" + request.Url.Authority + request.ApplicationPath.TrimEnd('/');
                url += "/resources/training/My-Training/Login?" + AccessToken(invite);
                return url;
            }

            public static string AccessToken(Invites invite) {
                EncryptedQueryString args = new EncryptedQueryString();
                args["user"] = invite.TraineeEmail;
                args["token"] = invite.AccessToken.ToString();
                return "token=" + args.ToString();
            }

            public static Invites InviteFromQueryStringToken(string token, IInvitesRepository invitesRepository) {
                Invites invite = null;
                if (token != null)
                {
                    EncryptedQueryString args = new EncryptedQueryString(token);
                    token = args["token"] != null ? args["token"] : null;
                    // Lookup the invite
                    invite = invitesRepository.GetByAccessToken(token);                    
                }
                return invite;
            }

            public class CustomAuthorization
            {
                public const int SessionLengthMinutes = 240; // TODO: 4 hours, is this a good number?

                public class CustomAuthorizeAttribute : AuthorizeAttribute
                {
                    
                    // TODO: Remove this when we've decided for sure that it is not necessary
                    // If you want to call this method with specific roles place this above method:
                    // [CustomAuthorization.Training.CustomAuthorize("Administrator", "Moderator")]

                    private readonly string[] allowedroles;
                    public CustomAuthorizeAttribute(params string[] roles)
                    {
                        this.allowedroles = roles;
                    }
                    protected override bool AuthorizeCore(HttpContextBase httpContext)
                    {
                        bool authorize = false;

                        EncryptedQueryString args = httpContext.Request.QueryString["token"] == null ? null : new EncryptedQueryString(httpContext.Request.QueryString["token"]);
                        var accessToken = args != null && args["token"] != null ? args["token"] : null;

                        if (accessToken != null)
                        {
                            if (GetAccessToken(accessToken) == null) {
                                HttpContext.Current.Response.Redirect("~/umbraco/Surface/CourseSurface/SessionExpired/", true);
                            }
                            else {
                                authorize = true;
                            }
                        }
                        else if (httpContext.User.Identity.IsAuthenticated)
                        {
                            authorize = true;
                        }
                        return authorize;
                    }
                    protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
                    {
                        filterContext.Result = new HttpUnauthorizedResult();
                    }
                }


                public static DateTime AccessExpiration() {
                    return DateTime.UtcNow.AddMinutes(SessionLengthMinutes);
                }

                public static HttpCookie GetAccessToken(string cookieKey) {
                    HttpCookie newCookie = HttpContext.Current.Request.Cookies[cookieKey];
                    if (newCookie != null) newCookie.Expires = AccessExpiration();
                    return newCookie;
                }

                public static HttpCookie SetAccessToken(string cookieKey) {
                    HttpCookie newCookie = HttpContext.Current.Request.Cookies[cookieKey];
                    if (newCookie == null) {
                        newCookie = new HttpCookie(cookieKey);
                        newCookie.Expires = AccessExpiration();
                        HttpContext.Current.Response.Cookies.Add(newCookie);
                    }
                    return newCookie;
                }

            }


            [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
            public sealed class AntiForgeryToken : System.Web.Http.AuthorizeAttribute
            {

                private void ValidateRequestHeader(string tokenValue)
                {
                    string cookieToken = String.Empty;
                    string formToken = String.Empty;
                     if (!String.IsNullOrEmpty(tokenValue))
                    {
                        string[] tokens = tokenValue.Split(':');
                        if (tokens.Length == 2)
                        {
                            cookieToken = tokens[0].Trim();
                            formToken = tokens[1].Trim();
                        }
                    }
                    AntiForgery.Validate(cookieToken, formToken);
                }

                public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
                {
                    try
                    {
                        // IsAjaxRequest?
                        if (actionContext.Request.Headers.Contains("X-Requested-With") && actionContext.Request.Headers.GetValues("X-Requested-With").FirstOrDefault() == "XMLHttpRequest") {
                            string requestToken = actionContext.Request.Headers.GetValues("RequestVerificationToken").FirstOrDefault();
                            ValidateRequestHeader(requestToken);
                        }
                        else
                        {
                            AntiForgery.Validate();
                        }
                    }
                    catch (HttpAntiForgeryException e)
                    {
                        throw new HttpAntiForgeryException("Anti forgery token cookie not found");
                    }


                }
            }


            

        }



    }
}
