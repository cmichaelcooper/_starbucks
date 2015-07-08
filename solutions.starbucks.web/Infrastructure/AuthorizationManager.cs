
namespace solutions.starbucks.web.Infrastructure
{
    //public class AuthorizationManager : ClaimsAuthorizationManager
    //{
    //    public override bool CheckAccess(AuthorizationContext context)
    //    {
    //        string resource = context.Resource.First().Value;
    //        string action = context.Action.First().Value;

    //        if (action == "Get" && resource == "Contacts")
    //        {
    //            ClaimsIdentity id = (context.Principal.Identity as ClaimsIdentity);

    //            return (id.Claims.Any(c => c.Type == "http://mycontacts.development.tomrankin.net/contacts/OAuth20/claims/scope" &&
    //                                                    c.Value.Equals("Read.Contacts")));
    //        }

    //        return false;
    //    }

    //}
}