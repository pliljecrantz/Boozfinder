using Boozfinder.Models.Data;
using Boozfinder.Providers.Interfaces;
using LiteDB;
using System;
using System.Linq;

namespace Boozfinder.Providers
{
    public class AuthenticationProvider : IAuthenticationProvider
    {
        public AuthenticationProvider(ICacheProvider cacheProvider)
        {
            CacheProvider = cacheProvider;
        }

        public ICacheProvider CacheProvider { get; }

        public bool AuthenticateUser(string email, string password)
        {
            var result = false;
            using (var db = new LiteDatabase(@"Boozfinder.db"))
            {
                try
                {
                    var userCollection = db.GetCollection<User>("user");
                    var user = userCollection.Find(x => x.Email.Equals(email)).First();
                    if (user != null && user.Password.Equals(password))
                    {
                        result = true;
                    }
                }
                catch (Exception)
                {
                    // TODO: Log error
                }
                return result;
            }
        }
    }
}
