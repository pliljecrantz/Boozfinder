using Boozfinder.Models.Data;
using Boozfinder.Providers.Interfaces;
using LiteDB;
using System;
using System.Collections.Generic;

namespace Boozfinder.Providers
{
    public class BoozeProvider : IBoozeProvider
    {
        /// <summary>
        /// Gets an item by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A Booze object</returns>
        public Booze Get(int id)
        {
            var result = new Booze();
            using (var db = new LiteDatabase(@"Boozfinder.db"))
            {
                try
                {
                    var boozeCollection = db.GetCollection<Booze>("booze");
                    result = boozeCollection.FindById(id);
                }
                catch (Exception ex)
                {
                    // TODO: Log error
                }
                return result;
            }
        }

        /// <summary>
        /// Gets all items
        /// </summary>
        /// <returns>An IEnumerable of Booze objects</returns>
        public IEnumerable<Booze> Get()
        {
            IEnumerable<Booze> result = null;
            using (var db = new LiteDatabase(@"Boozfinder.db"))
            {
                try
                {
                    var boozeCollection = db.GetCollection<Booze>("booze");
                    result = boozeCollection.FindAll();
                }
                catch (Exception ex)
                {
                    // TODO: Log error
                }
                return result;
            }
        }

        /// <summary>
        /// Saves the item
        /// </summary>
        /// <param name="booze"></param>
        /// <returns>The saved item with the auto created id</returns>
        public Booze Save(Booze booze)
        {
            Booze item = null;
            using (var db = new LiteDatabase(@"Boozfinder.db"))
            {
                try
                {
                    var boozeCollection = db.GetCollection<Booze>("booze");
                    var id = boozeCollection.Insert(booze);
                    item = Get(id.AsInt32);
                }
                catch (Exception ex)
                {
                    // TODO: Log error
                }
                return item;
            }
        }

        /// <summary>
        /// Updates the item
        /// </summary>
        /// <param name="booze"></param>
        /// <returns>An updated version of the item</returns>
        public Booze Update(Booze booze)
        {
            Booze item = null;
            using (var db = new LiteDatabase(@"Boozfinder.db"))
            {
                try
                {
                    var boozeCollection = db.GetCollection<Booze>("booze");
                    var updated = boozeCollection.Update(booze);
                    if (updated)
                    {
                        item = Get(booze.Id);
                    }
                }
                catch (Exception ex)
                {
                    // TODO: Log error
                }
                return item;
            }
        }

        public IEnumerable<Booze> Search(string searchTerm, string type = "")
        {
            IEnumerable<Booze> result = null;
            using (var db = new LiteDatabase(@"Boozfinder.db"))
            {
                try
                {
                    var boozeCollection = db.GetCollection<Booze>("booze");
                    if (!string.IsNullOrWhiteSpace(type)) // If a type has been provided, add that to the search together with the searchterm to make an exclusive search
                    {
                        result = boozeCollection.Find(x => x.Name.Contains(searchTerm) && x.Type.Equals(type));
                    }
                    else
                    {
                        result = boozeCollection.Find(x => x.Name.Contains(searchTerm));
                    }
                }
                catch (Exception ex)
                {
                    // TODO: Log error
                }
                return result;
            }
        }
    }
}
