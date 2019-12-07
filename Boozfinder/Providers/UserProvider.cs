using Boozfinder.Helpers;
using Boozfinder.Models.Data;
using Boozfinder.Providers.Interfaces;
using LiteDB;
using System;

namespace Boozfinder.Providers
{
    public class UserProvider : IUserProvider
    {
        public UserProvider(ICacheProvider cacheProvider)
        {
            CacheProvider = cacheProvider;
        }

        public ICacheProvider CacheProvider { get; }

        public bool CreateUser(User user)
        {
            var result = false;
            using (var db = new LiteDatabase(@"Boozfinder.db"))
            {
                var userCollection = db.GetCollection<User>("user");
                try
                {
                    var userExists = userCollection.FindOne(x => x.Email.Equals(user.Email));
                    if (userExists == null)
                    {
                        userCollection.Insert(user);
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    // TODO: Log error
                }
                return result;
            }
        }

        public bool HaveAdminRole(string email)
        {
            var result = false;
            using (var db = new LiteDatabase(@"Boozfinder.db"))
            {
                var userCollection = db.GetCollection<User>("user");
                try
                {
                    var user = userCollection.FindOne(x => x.Email.Equals(email));
                    if (user != null)
                    {
                        result = user.Role.ToLower() == Enums.Role.Admin.ToString().ToLower();
                    }
                }
                catch (Exception ex)
                {
                    // TODO: Log error
                }
                return result;
            }
        }

        // TODO: Implement possibility for admins to list users and update password
        // TODO: Implement salting and hashing of passwords before saving user to database
    }
}
