using Boozfinder.Helpers;
using Boozfinder.Models.Data;
using Boozfinder.Providers.Interfaces;
using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;

namespace Boozfinder.Providers
{
    public class BoozeProvider : IBoozeProvider
    {
        public Booze Get(int id)
        {
            var item = new Booze();
            using (var db = new LiteDatabase(Globals.DatabaseName))
            {
                try
                {
                    var boozeCollection = db.GetCollection<Booze>(Globals.BoozeCollection);
                    item = boozeCollection.FindById(id);

                    if (item.HasImage)
                    {
                        var stream = db.FileStorage.OpenRead($"$/{id}.jpg");
                        using (var memoryStream = new MemoryStream())
                        {
                            stream.CopyTo(memoryStream);
                            var imageDataByteArray = memoryStream.ToArray();
                            var imageData = Convert.ToBase64String(imageDataByteArray);
                            item.ImageData = imageData;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // TODO: Log error
                }
                return item;
            }
        }

        public IEnumerable<Booze> Get()
        {
            IEnumerable<Booze> itemList = null;
            using (var db = new LiteDatabase(Globals.DatabaseName))
            {
                try
                {
                    var boozeCollection = db.GetCollection<Booze>(Globals.BoozeCollection);
                    itemList = boozeCollection.FindAll();

                    foreach (var item in itemList)
                    {
                        if (item.HasImage)
                        {
                            var stream = db.FileStorage.OpenRead($"$/{item.Id}.jpg");
                            using (var memoryStream = new MemoryStream())
                            {
                                stream.CopyTo(memoryStream);
                                var imageDataByteArray = memoryStream.ToArray();
                                var imageData = Convert.ToBase64String(imageDataByteArray);
                                item.ImageData = imageData;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // TODO: Log error
                }
                return itemList;
            }
        }

        public Booze Save(Booze booze)
        {
            Booze savedItem = null;
            using (var db = new LiteDatabase(Globals.DatabaseName))
            {
                try
                {
                    var boozeCollection = db.GetCollection<Booze>(Globals.BoozeCollection);
                    var id = boozeCollection.Insert(booze).AsInt32;
                    savedItem = Get(id);

                    // If there is an image attached, upload it with id and name of item as identifier
                    if (booze.ImageData != null)
                    {
                        booze.HasImage = true;

                        // Takes a Base64String from UI and saves it to LiteDB using FileStorage
                        // TEST: "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"
                        var imageDataByteArray = Convert.FromBase64String(booze.ImageData);

                        //When creating a stream, you need to reset the position, without it you will see that you always write files with a 0 byte length. 
                        var imageDataStream = new MemoryStream(imageDataByteArray)
                        {
                            Position = 0
                        };

                        // Upload a file from a Stream
                        db.FileStorage.Upload($"$/{savedItem.Id}.jpg", $"{savedItem.Id}.jpg", imageDataStream);
                    }
                }
                catch (Exception ex)
                {
                    // TODO: Log error
                }
                return savedItem;
            }
        }

        public Booze Update(Booze booze)
        {
            Booze updatedItem = null;
            using (var db = new LiteDatabase(Globals.DatabaseName))
            {
                try
                {
                    var boozeCollection = db.GetCollection<Booze>(Globals.BoozeCollection);
                    var updated = boozeCollection.Update(booze);
                    if (updated)
                    {
                        updatedItem = Get(booze.Id);
                    }
                }
                catch (Exception ex)
                {
                    // TODO: Log error
                }
                return updatedItem;
            }
        }

        public IEnumerable<Booze> Search(string searchTerm, string type = "")
        {
            IEnumerable<Booze> itemList = null;
            using (var db = new LiteDatabase(Globals.DatabaseName))
            {
                try
                {
                    var boozeCollection = db.GetCollection<Booze>(Globals.BoozeCollection);
                    if (!string.IsNullOrWhiteSpace(type)) // If a booze type has been provided, add that to the search together with the searchterm to make an exclusive search
                    {
                        itemList = boozeCollection.Find(x => x.Name.Contains(searchTerm) && x.Type.Equals(type));
                    }
                    else
                    {
                        itemList = boozeCollection.Find(x => x.Name.Contains(searchTerm));
                    }
                }
                catch (Exception ex)
                {
                    // TODO: Log error
                }
                return itemList;
            }
        }
    }
}
