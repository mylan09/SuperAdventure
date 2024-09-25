using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Engine;

namespace SuperAdventure
{
    public partial class WorldMap : Form
    {
        readonly Assembly _thisAssembly = Assembly.GetExecutingAssembly();
        private readonly Player _player;
        private readonly Bitmap _fogImage;

        public WorldMap(Player player)
        {
            InitializeComponent();

            _player = player;
            _fogImage = FilenameToBitmap("FogLocation");

            DisplayImage(pic_0_2, 5, "HerbalistsGarden");
            DisplayImage(pic_0_4, 13, "Dungeon");
            DisplayImage(pic_0_3, 14, "DungeonEntrance");
            DisplayImage(pic_0_5, 11, "Swamp");
            DisplayImage(pic_1_2, 4, "HerbalistsHut");
            DisplayImage(pic_1_3, 15, "SkeletonCave");
            DisplayImage(pic_1_4, 12, "DungeonGuardPost");
            DisplayImage(pic_1_5, 10, "pond");
            DisplayImage(pic_2_0, 7, "FarmFields");
            DisplayImage(pic_2_1, 6, "Farmhouse");
            DisplayImage(pic_2_2, 2, "TownSquare");
            DisplayImage(pic_2_3, 3, "TownGate");
            DisplayImage(pic_2_4, 8, "Bridge");
            DisplayImage(pic_2_5, 9, "SpiderForest");
            DisplayImage(pic_3_2, 1, "Home");
            DisplayImage(pic_3_1, 16, "TrainingsRoom");
            
        }

        private void DisplayImage(PictureBox pictureBox, int locationID, string imageName)
        {
            if (_player.LocationsVisited.Contains(locationID))
            {
                pictureBox.Image = FilenameToBitmap(imageName);
                
                if(_player.CurrentLocation.ID == locationID)
                {
                   using(Graphics g = Graphics.FromImage(pictureBox.Image))
                   {
                        int borderWidth = 3;

                       g.DrawRectangle(new Pen(Color.Red, borderWidth), new Rectangle(0, 0, pictureBox.Image.Width - borderWidth,
                                                      pictureBox.Image.Height - borderWidth));
                    }
                }
            }
            else
            {
                pictureBox.Image = _fogImage;
            }
        }

        private Bitmap FilenameToBitmap(string imageFileName)
        {
            string fullFileName =
                $"{_thisAssembly.GetName().Name}.Images.{imageFileName}.png";

            using (Stream resourceStream =
                _thisAssembly.GetManifestResourceStream(fullFileName))
            {
                if (resourceStream != null)
                {
                    return new Bitmap(resourceStream);
                }
            }

            return null;
        }

    }
}
