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

namespace SuperAdventure
{
    public partial class LoadScreen : Form
    {
        private readonly List<string> _saveFiles;
        public LoadScreen()
        {
            InitializeComponent();
            // Lade die letzten drei Speicherdateien
            if (Directory.Exists("SaveGames"))
            {
                _saveFiles = Directory.GetFiles("SaveGames")
                                      .OrderByDescending(f => new FileInfo(f).CreationTime)
                                      .Take(3)
                                      .ToList();

                // Dateinamen in die Buttons einfügen
                if (_saveFiles.Count > 0) btnSave1.Text = Path.GetFileName(_saveFiles[0]);
                if (_saveFiles.Count > 1) btnSave2.Text = Path.GetFileName(_saveFiles[1]);
                if (_saveFiles.Count > 2) btnSave3.Text = Path.GetFileName(_saveFiles[2]);
            }
        }
        private void btnSave1_Click(object sender, EventArgs e)
        {
            if (_saveFiles.Count > 0)
            {
                StartGame(_saveFiles[0]);
            }
        }

        private void btnSave2_Click(object sender, EventArgs e)
        {
            if (_saveFiles.Count > 1)
            {
                StartGame(_saveFiles[1]);
            }
        }

        private void btnSave3_Click(object sender, EventArgs e)
        {
            if (_saveFiles.Count > 2)
            {
                StartGame(_saveFiles[2]);
            }
        }

        private void StartGame(string saveFile)
        {
            // Starte das Spiel mit der ausgewählten Speicherdatei
            SuperAdventure game = new SuperAdventure(saveFile);
            game.StartPosition = FormStartPosition.CenterScreen;
            game.FormClosed += (s, args) => this.Close();
            game.Show();
            this.Hide();
        }
    }
}
