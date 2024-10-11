using Engine;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Diese Klasse ist für die Verbindung zur MongoDB-Datenbank zuständig und das Registrieren der Klassen, die in der Datenbank gespeichert werden.

namespace SuperAdventure.Database
{
    public class MongoConnector
    {
        public IMongoDatabase Database { get; private set; }

        public void Connect()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            this.Database = client.GetDatabase("SuperAdventure");
            RegisterBsonClassMaps(); 
        }

        private void RegisterBsonClassMaps()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(Player)))
            {
                BsonClassMap.RegisterClassMap<Player>(cm =>
                {
                    cm.AutoMap();
                    cm.SetDiscriminator("Player");
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(InventoryItem)))
            {
                BsonClassMap.RegisterClassMap<InventoryItem>(cm =>
                {
                    cm.AutoMap();
                    cm.SetDiscriminator("InventoryItem");
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(HealingPotion)))
            {
                BsonClassMap.RegisterClassMap<HealingPotion>(cm =>
                {
                    cm.AutoMap();
                    cm.SetDiscriminator("HealingPotion");
                });
            }

            // Registriere die Quest-Klasse
            if (!BsonClassMap.IsClassMapRegistered(typeof(Quest)))
            {
                BsonClassMap.RegisterClassMap<Quest>(cm =>
                {
                    cm.AutoMap();
                    cm.SetDiscriminator("Quest");
                });
            }

        }

        public List<Player> GetLatestSaveGames(int limit)
        {
            var collection = Database.GetCollection<Player>("savegames");

            // Sortiere nach dem LastSaveTime-Feld und nehme die neuesten Einträge
            var saveGames = collection.Find(Builders<Player>.Filter.Empty)
                                      .SortByDescending(p => p.LastSaveTime)  // Sortiere nach LastSaveTime
                                      .Limit(limit)
                                      .ToList();

            return saveGames;
        }
    }
}
