using DotNetOpenAuth.OAuth2.Messages;
using System.Collections.Generic;

namespace solutions.starbucks.Model
{
    public class AuthorizationRequest
    {
        public string ClientApp { get; set; }

        public HashSet<string> Scope { get; set; }

        public EndUserAuthorizationRequest Request { get; set; }
    }
}