using Engine;
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
    public partial class SaveScreen : Form
    {
        private Player _player;
        public SaveScreen(Player player)
        {
            InitializeComponent();
            _player = player;

        }

        private void btnYes_Click(object sender, EventArgs e)
        {
            _player.ToJsonString();
            MessageBox.Show("Game progress successfully saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
