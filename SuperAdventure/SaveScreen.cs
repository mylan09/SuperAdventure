using Engine;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using SuperAdventure.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//Diese Klasse ist für den Speicherbildschirm verantwortlich. Hier wird der aktuelle Spielstand des Spielers gespeichert.

namespace SuperAdventure
{
    public partial class SaveScreen : Form
    {
        private Player _player;
        public SaveScreen(Player player)
        {
            InitializeComponent();
            _player = player;

        }
        public void UpdatePlayerSave(Player player)
        {

            var mongoDB = new MongoConnector();
            mongoDB.Connect();

            var collection = mongoDB.Database.GetCollection<Player>("savegames");

            // Sichert den aktuellen Zustand des Spielers für den neuen Speicherstand.
            // Generiert eine neue ID für die eindeutige Identifikation des Spielstandes.

            var newPlayer = new Player
            {
                Id = ObjectId.GenerateNewId(), 
                Gold = player.Gold,
                ExperiencePoints = player.ExperiencePoints,
                CurrentLocation = player.CurrentLocation,
                CurrentWeapon = player.CurrentWeapon,
                LocationsVisited = player.LocationsVisited,
                Chest = player.Chest,
                Inventory = player.Inventory,
                Quests = player.Quests,
                MaximumHitPoints = player.MaximumHitPoints,
                CurrentHitPoints = player.CurrentHitPoints,
                Level = player.Level,
                LastSaveTime = DateTime.UtcNow
            };

            // Speichern des neuen Spielstands
            collection.InsertOne(newPlayer);

            MessageBox.Show("Game progress successfully saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void btnYes_Click(object sender, EventArgs e)
        {
            UpdatePlayerSave(_player); // Update des aktuellen Spielstands
            Close();
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
