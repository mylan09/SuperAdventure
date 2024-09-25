using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SuperAdventure
{
    public partial class StartScreen : Form
    {
        public StartScreen()
        {
            InitializeComponent();
        }

        private void btnNewGame_Click(object sender, EventArgs e)
        {
            SuperAdventure newGame = new SuperAdventure(null); // Neues Spiel ohne Speicherdatei starten
            newGame.StartPosition = FormStartPosition.CenterScreen;
            newGame.Show();
            this.Hide();
        }

        private void btnLoadSave_Click(object sender, EventArgs e)
        {
            LoadScreen loadScreen = new LoadScreen();
            loadScreen.StartPosition = FormStartPosition.CenterScreen;
            loadScreen.Show();
            this.Hide();
        }
    }
}
