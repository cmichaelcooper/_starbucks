using DotNetOpenAuth.Messaging.Bindings;
using solutions.starbucks.Model.Pocos;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Persistence;

namespace solutions.starbucks.web.Infrastructure.Store
{
    public class CryptoKeyStore : ICryptoKeyStore
    {
        //private IList<SymmetricCryptoKey> cryptoKeys = new List<SymmetricCryptoKey>();
        protected UmbracoDatabase _db = ApplicationContext.Current.DatabaseContext.Database;

        public CryptoKey GetKey(string bucket, string handle)
        {
            //var item = _db.Fetch<SymmetricCryptoKeys>(
            //    string queryText = "SELECT * FROM SymmetricCryptoKeys WHERE Bucket=@0 AND Handle=@1";
            //return _db.Query<SymmetricCryptoKeys>(queryText, bucket, handle);

                        var queryText = Sql.Builder.Append("SELECT TOP (1) * FROM SymmetricCryptoKeys WHERE Bucket=@0 AND Handle=@1", bucket, handle);
            var a = _db.SingleOrDefault<SymmetricCryptoKeys>(queryText);
            if (a == null) return null;
            var cryptoKey = new CryptoKey(a.Key.Take(a.DataLength).ToArray(), DateTime.SpecifyKind(new DateTime(a.Ticks), DateTimeKind.Utc));
           

            return cryptoKey;
        }

        public IEnumerable<KeyValuePair<string, CryptoKey>> GetKeys(string bucket)
        {
            var queryText = Sql.Builder.Append("SELECT * FROM SymmetricCryptoKeys WHERE Bucket=@0", bucket);
            var query = _db.Query<SymmetricCryptoKeys>(queryText);



            return query.ToList().Select(p => new KeyValuePair<string, CryptoKey>(p.Handle, new CryptoKey(p.Key.Take(p.DataLength).ToArray(), DateTime.SpecifyKind(new DateTime(p.Ticks), DateTimeKind.Utc)))).ToList();
            //return _db.Query<SymmetricCryptoKeys>(queryText); 
            //using (var db = new pgpro_oauthEntities())
            //{
            //    return db.SymmetricCryptoKeys.Where(p => p.Bucket == bucket).ToList().Select(p => new KeyValuePair<string, CryptoKey>(p.Handle, new CryptoKey(p.Key.Take(p.DataLength).ToArray(), DateTime.SpecifyKind(new DateTime(p.Ticks), DateTimeKind.Utc)))).ToList();
            //}
        }

        public void RemoveKey(string bucket, string handle)
        {
            //db.Execute("DELETE FROM articles WHERE draft<>0");
            var queryText = Sql.Builder.Append("DELETE FROM SymmetricCryptoKeys WHERE Bucket=@0 AND Handle=@1", bucket, handle);
            var a = _db.Execute(queryText);
            
            
            //using (var db = new pgpro_oauthEntities())
            //{
            //    var item = db.SymmetricCryptoKeys.SingleOrDefault(p => p.Bucket == bucket && p.Handle == handle);
            //    if (item == null) return;
            //    db.SymmetricCryptoKeys.Remove(item);
            //    db.SaveChanges();
            //}
        }

        public void StoreKey(string bucket, string handle, CryptoKey key)
        {

            var a = _db.SingleOrDefault<SymmetricCryptoKeys>("SELECT * FROM SymmetricCryptoKeys WHERE Bucket=@0 AND Handle=@1", bucket, handle);

            if (a == null)
            {
                SymmetricCryptoKeys keyToSave = new SymmetricCryptoKeys();
                keyToSave.Bucket = bucket;
                keyToSave.Handle = handle;
                keyToSave.ExpiresUTC = key.ExpiresUtc;
                keyToSave.Key = key.Key; 
                keyToSave.Ticks = key.ExpiresUtc.Ticks;
                keyToSave.DataLength = key.Key.Length ;
                _db.Insert(keyToSave);
            }
            else
            {
                a.Key = key.Key;
                a.ExpiresUTC = key.ExpiresUtc;
                a.Ticks = key.ExpiresUtc.Ticks;
                a.DataLength = key.Key.Length;
                _db.Save(a);
            }
            // Save it
            

           
                //db.SaveChanges();
            //}
        }
    }
}