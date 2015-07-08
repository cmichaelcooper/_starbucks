using DotNetOpenAuth.Messaging.Bindings;
using DotNetOpenAuth.OAuth2;
using solutions.starbucks.Model.Pocos;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace solutions.starbucks.web.Infrastructure.Store
{
    public class DataStore
    {
        private static DataStore store = null;
        protected UmbracoDatabase _db = ApplicationContext.Current.DatabaseContext.Database;

        static DataStore()
        {
            store = new DataStore();
        }

        private DataStore()
        {
            this.Clients = new List<Client>();
            this.CryptoKeyStore = new CryptoKeyStore();
            this.NonceStore = new NonceStore();
            var queryText = Sql.Builder.Append("SELECT * FROM Clients");
            var a = _db.Fetch<Clients>(queryText);

            foreach (var item in a)
            {
                var client = new Client();
                client.ClientIdentifier = item.ClientId.ToString();
                client.ClientSecret = item.ClientSecret;
                client.ClientType = item.ClientType == "Confidential" ? ClientType.Confidential : ClientType.Public;
                client.DefaultCallback = new Uri(item.DefaultCallback);
                //var authQueryTexts = Sql.Builder.Append("SELECT Count(*) FROM ClientAuthorizations WHERE ClientId=@0", item.ClientId);
                //var qs = _db.Fetch<ClientAuthorizations>(queryText);

                //This is returning a value even though the table is empty

                //var authQueryText = Sql.Builder.Append("SELECT * FROM ClientAuthorizations WHERE ClientId=@0", item.ClientId);
                //var q = _db.Fetch<ClientAuthorizations>(queryText);
                //if (q != null)
                //{
                //    //do something
                //}
                //foreach (var auth in q)
                //{
                //    var tmpAuth = new ClientAuthorization();
                //    tmpAuth.ExpirationDateUtc = auth.ExpirationDateUtc;
                //    tmpAuth.IssueDate = auth.IssueDate;
                //    var authScopeQueryText = Sql.Builder.Append("SELECT * FROM ClientAuthorizationScopes WHERE ClientId=@0", item.ClientId);
                //    var clientAuthScopeQuery = _db.Fetch<ClientAuthorizationScopes>(queryText);
                //    tmpAuth.Scope = new HashSet<string>(clientAuthScopeQuery.Select(p => p.Scope).ToList());
                //    //tmpAuth.Scope = new HashSet<string>(auth.ClientAuthorizationScopes.Select(p => p.Scope).ToList());
                //    tmpAuth.UserId = auth.UserId.ToString();
                //    client.ClientAuthorizations.Add(tmpAuth);
                //}

                this.Clients.Add(client);
            }
            //}
        }

        public static DataStore Instance
        {
            get
            {
                return store;
            }
        }


        public IList<Client> Clients { get; set; }

        public ICryptoKeyStore CryptoKeyStore { get; set; }
        public INonceStore NonceStore { get; set; }

        public void AddAuth(string ClientIdentifier, ClientAuthorization auth)
        {
            var queryText = Sql.Builder.Append("SELECT TOP (1) * FROM ClientAuthorizations WHERE ClientId=@0 AND UserId=@1", new Guid(ClientIdentifier), auth.UserId);
            var existing = _db.SingleOrDefault<ClientAuthorizations>(queryText);
            if (existing == null)
            {
                existing = new ClientAuthorizations();
                existing.ClientId = new Guid(ClientIdentifier);
                existing.UserId = auth.UserId;
                existing.IssueDate = auth.IssueDate;
                existing.ExpirationDateUtc = auth.ExpirationDateUtc;

                foreach (var item in auth.Scope)
                {
                    var clientAuthScopes = new ClientAuthorizationScopes();
                    clientAuthScopes.ClientId = new Guid(ClientIdentifier);
                    clientAuthScopes.Scope = item;
                    clientAuthScopes.UserId = auth.UserId;
                    if (existing.ClientId != null)
                    {
                        _db.Insert(existing);
                    }

                    _db.Insert(clientAuthScopes);
                }


            }
            var existingLocal = this.Clients.Single(p => p.ClientIdentifier == ClientIdentifier).ClientAuthorizations.SingleOrDefault(f => f.UserId == auth.UserId);
            if (existingLocal == null)
            {
                this.Clients.Single(p => p.ClientIdentifier == ClientIdentifier).ClientAuthorizations.Add(auth);
            }
            else
            {
                existingLocal.Scope = auth.Scope;
                existingLocal.IssueDate = auth.IssueDate;
                existingLocal.ExpirationDateUtc = auth.ExpirationDateUtc;
            }
            //}


        }
       
    }

}