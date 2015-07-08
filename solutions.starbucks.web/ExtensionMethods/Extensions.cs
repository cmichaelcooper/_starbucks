using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Umbraco.Core;


namespace solutions.starbucks.web.ExtensionMethods
{
    public static class Extensions
    {
        public static bool AccountNumberVerification(this ApplicationContext applicationContext,string accountNumber, string zip)
        {
            var db = applicationContext.DatabaseContext.Database;
            //Get the current Umbraco Node ID
            string queryText = "SELECT * FROM CustomerAttributes WHERE AccountNumber=@0 AND PostalCode LIKE @1";

            int userAccountCount = db.Query<string>(queryText, accountNumber, "%" + zip + "%").Count();
            bool userAccount = db.Query<string>(queryText, accountNumber, "%" + zip + "%").Any();
            //Account/zip combination exists, ready to add to profile
            if (userAccountCount > 0 && userAccount)
            {
                return true;
            }

            return false;
        }

        public static string ToMD5HashString(this string input)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] bytesToHash = System.Text.Encoding.ASCII.GetBytes(input);
            bytesToHash = md5.ComputeHash(bytesToHash);
            StringBuilder sb = new StringBuilder();

            foreach (byte b in bytesToHash)
            {
                sb.Append(b.ToString("x2", CultureInfo.InvariantCulture));
            }

            return sb.ToString();
        }

        public static X509Certificate2 ToCertificate(this string subjectName,
                                                        StoreName name = StoreName.My,
                                                            StoreLocation location = StoreLocation.LocalMachine)
        {
            X509Store store = new X509Store(name, location);
            store.Open(OpenFlags.ReadOnly);

            try
            {
                var cert = store.Certificates.OfType<X509Certificate2>()
                            .FirstOrDefault(c => c.SubjectName.Name.Equals(subjectName,
                                StringComparison.OrdinalIgnoreCase));

                return (cert != null) ? new X509Certificate2(cert) : null;
            }
            finally
            {
                store.Certificates.OfType<X509Certificate2>().ToList().ForEach(c => c.Reset());
                store.Close();
            }
        }


      
    }
}