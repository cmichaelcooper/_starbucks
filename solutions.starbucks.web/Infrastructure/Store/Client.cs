using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth2;
using System;
using System.Collections.Generic;

namespace solutions.starbucks.web.Infrastructure.Store
{
    public class Client : IClientDescription
    {
        public string ClientIdentifier { get; set; }

        public string ClientSecret { get; set; }

        public Uri DefaultCallback { get; set; }

        public string Name { get; set; }

        public ClientType ClientType { get; set; }

        public IList<ClientAuthorization> ClientAuthorizations { get; set; }

        public Client()
        {
            this.ClientAuthorizations = new List<ClientAuthorization>();
        }

        public bool HasNonEmptySecret
        {
            get { return !string.IsNullOrEmpty(this.ClientSecret); }
        }

        public bool IsCallbackAllowed(Uri callback)
        {
            return callback.Scheme == this.DefaultCallback.Scheme &&
                        callback.Host == this.DefaultCallback.Host &&
                            callback.Port == this.DefaultCallback.Port;
        }

        public bool IsValidClientSecret(string secret)
        {
            return MessagingUtilities.EqualsConstantTime(secret, this.ClientSecret);
        }
    }
}