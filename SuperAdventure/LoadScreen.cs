using Engine;
using SuperAdventure.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//Diese Klasse ist für den Ladebildschirm des Spiels zuständig. Sie zeigt die letzten drei gespeicherten Spielstände an.

namespace SuperAdventure
{
    public partial class LoadScreen : Form
    {
        private readonly List<Player> _saveGames;
        private MongoConnector _mongoConnector;

        public LoadScreen()
        {
            InitializeComponent();
            // Lade die letzten drei Speicherdateien
            _mongoConnector = new MongoConnector();
            _mongoConnector.Connect();

            // Lade die letzten drei Speicherstände
            _saveGames = _mongoConnector.GetLatestSaveGames(3);


            if (_saveGames.Count > 0) btnSave1.Text = _saveGames[0].LastSaveTime.ToString();
            if (_saveGames.Count > 1) btnSave2.Text = _saveGames[1].LastSaveTime.ToString();
            if (_saveGames.Count > 2) btnSave3.Text = _saveGames[2].LastSaveTime.ToString();
        }
        private void btnSave1_Click(object sender, EventArgs e)
        {
            if (_saveGames.Count > 0)
            {
                StartGame(_saveGames[0]);
            }
        }

        private void btnSave2_Click(object sender, EventArgs e)
        {
            if (_saveGames.Count > 1)
            {
                StartGame(_saveGames[1]);
            }
        }

        private void btnSave3_Click(object sender, EventArgs e)
        {
            if (_saveGames.Count > 2)
            {
                StartGame(_saveGames[2]);
            }
        }

        private void StartGame(Player player)
        {
            SuperAdventure game = new SuperAdventure(player);  // Übergabe des Player-Objekts
            game.StartPosition = FormStartPosition.CenterScreen;
            game.FormClosed += (s, args) => this.Close();
            game.Show();
            this.Hide();
        }
    }
}
