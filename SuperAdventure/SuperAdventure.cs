using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Media;
using System.IO;
using Newtonsoft.Json;
using Engine;
using System.Numerics;
using Newtonsoft.Json.Linq;
using Engine.Converter;
using static System.Windows.Forms.AxHost;
using System.Security.Cryptography;

namespace SuperAdventure
{

    public partial class SuperAdventure : Form
    {

        private const string PLAYER_DATA_FILE_NAME = "SaveGames";

        private Player _player;

        public SuperAdventure(string selectedSaveFile)
        {
            InitializeComponent();
            
            if (!string.IsNullOrEmpty(selectedSaveFile))
                {
                    if (File.Exists(selectedSaveFile))
                    {
                        var playerJsonStr = File.ReadAllText(selectedSaveFile);

                        var options = new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.All,
                            PreserveReferencesHandling = PreserveReferencesHandling.All,
                            NullValueHandling = NullValueHandling.Ignore,
                            MissingMemberHandling = MissingMemberHandling.Ignore,
                            DefaultValueHandling = DefaultValueHandling.Ignore,
                        };

                        World.Initialize();

                        _player = JsonConvert.DeserializeObject<Player>(playerJsonStr, options);
                        _player.MoveTo(World.LocationByID(_player.CurrentLocation.ID));
                    }
                }
                else
                {
                    // Standard-Spieler initialisieren
                    _player = Player.CreateDefaultPlayer();
                    _player.MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
                }

            lblHitPoints.DataBindings.Add("Text", _player, "CurrentHitPoints");
            lblGold.DataBindings.Add("Text", _player, "Gold");
            lblExperience.DataBindings.Add("Text", _player, "ExperiencePoints");
            lblLevel.DataBindings.Add("Text", _player, "Level");

            dgvInventory.ScrollBars = ScrollBars.Both;
            dgvInventory.RowHeadersVisible = false;
            dgvInventory.AutoGenerateColumns = false;

            dgvInventory.DataSource = _player.Inventory;

            dgvInventory.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Name",
                Width = 197,
                DataPropertyName = "Description"
            });

            dgvInventory.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Quantity",
                DataPropertyName = "Quantity"
            });

            dgvInventory.ScrollBars = ScrollBars.Vertical;
            
            dgvQuests.ScrollBars = ScrollBars.Both;
            dgvQuests.RowHeadersVisible = false;
            dgvQuests.AutoGenerateColumns = false;

            dgvQuests.DataSource = _player.Quests;

            dgvQuests.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Name",
                Width = 197,
                DataPropertyName = "Name"
            });

            dgvQuests.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Done?",
                DataPropertyName = "IsCompleted",
                Name = "IsCompletedColumn"
            });

            cboWeapons.DataSource = _player.Weapons;
            cboWeapons.DisplayMember = "Name";
            cboWeapons.ValueMember = "Id";

            if (_player.CurrentWeapon != null)
            {
                cboWeapons.SelectedItem = _player.CurrentWeapon;
            }

            cboWeapons.SelectedIndexChanged += cboWeapons_SelectedIndexChanged;

            cboPotions.DataSource = _player.Potions;
            cboPotions.DisplayMember = "Name";
            cboPotions.ValueMember = "Id";

            _player.PropertyChanged += PlayerOnPropertyChanged;
            _player.OnMessage += DisplayMessage;

            _player.MoveTo(_player.CurrentLocation);
        }
       
        private void DisplayMessage(object sender, MessageEventArgs messageEventArgs)
        {
            rtbMessages.SelectionStart = rtbMessages.Text.Length;
            rtbMessages.SelectionLength = 0;

            rtbMessages.SelectionColor = messageEventArgs.TextColor;
            rtbMessages.AppendText(messageEventArgs.Message + Environment.NewLine);

            if (messageEventArgs.AddExtraNewLine)
            {
                rtbMessages.AppendText(Environment.NewLine);
            }

            rtbMessages.SelectionColor = System.Drawing.Color.Black;
            rtbMessages.ScrollToCaret();
        }

        private void PlayerOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "Weapons")
            {
                Weapon previouslySelectedWeapon = _player.CurrentWeapon;

                cboWeapons.DataSource = _player.Weapons;

                if (previouslySelectedWeapon != null &&
                    _player.Weapons.Exists(w => w.ID == previouslySelectedWeapon.ID))
                {
                    cboWeapons.SelectedItem = previouslySelectedWeapon;
                }

                if (!_player.Weapons.Any())
                {
                    cboWeapons.Visible = false;
                    btnUseWeapon.Visible = false;
                }
            }

            if (propertyChangedEventArgs.PropertyName == "Potions")
            {
                cboPotions.DataSource = _player.Potions;

                if (!_player.Potions.Any())
                {
                    cboPotions.Visible = false;
                    btnUsePotion.Visible = false;
                }
            }

            if (propertyChangedEventArgs.PropertyName == "CurrentLocation")
            {
                // Show/hide available movement buttons
                btnNorth.Visible = (_player.CurrentLocation.LocationToNorth != null);
                btnEast.Visible = (_player.CurrentLocation.LocationToEast != null);
                btnSouth.Visible = (_player.CurrentLocation.LocationToSouth != null);
                btnWest.Visible = (_player.CurrentLocation.LocationToWest != null);
                btnTrade.Visible = (_player.CurrentLocation.VendorWorkingHere != null);

               
                // Display current location name and description
                rtbLocation.Text = _player.CurrentLocation.Name + Environment.NewLine;
                rtbLocation.Text += _player.CurrentLocation.Description + Environment.NewLine;

                if (!_player.CurrentLocation.HasAMonster)
                {
                    cboWeapons.Visible = false;
                    cboPotions.Visible = false;
                    btnUseWeapon.Visible = false;
                    btnUsePotion.Visible = false;
                    lblSelectAction.Visible = false;

                }
                else
                {
                    cboWeapons.Visible = _player.Weapons.Any();
                    cboPotions.Visible = _player.Potions.Any();
                    btnUseWeapon.Visible = _player.Weapons.Any();
                    btnUsePotion.Visible = _player.Potions.Any();
                    lblSelectAction.Visible = true;
                }
            }

            if (propertyChangedEventArgs.PropertyName == "CloseOffLocation")
            {
                var location = _player.CurrentLocation;
                if (location.CloseOffLocation)
                {
                  
                    btnWest.Visible = false;
                    rtbLocation.Text = "This location is closed off.";
                }
                else
                {
                 
                    btnWest.Visible = true;
                    
                }
            }
        }

        private void btnNorth_Click(object sender, EventArgs e)
        {
            _player.MoveNorth();
            //if(_player.CurrentLocation == World.LocationByID(World.LOCATION_ID_HOME))
            //{
            //    pctLocation.Image = iLLocations.Images[]
            //}
        }

        private void btnEast_Click(object sender, EventArgs e)
        {
            _player.MoveEast();
        }

        private void btnSouth_Click(object sender, EventArgs e)
        {
            _player.MoveSouth();
        }

        private void btnWest_Click(object sender, EventArgs e)
        {
            _player.MoveWest();
        }

        private void btnUseWeapon_Click(object sender, EventArgs e)
        {
            // Hole die aktuell ausgewählte Waffe
            Weapon currentWeapon = (Weapon)cboWeapons.SelectedItem;

            if (currentWeapon != null)
            {
                // Spieler benutzt die ausgewählte Waffe
                _player.UseWeapon(currentWeapon);

            }

        }


        private void btnUsePotion_Click(object sender, EventArgs e)
        {

            {
                // Get the currently selected potion from the combobox
                HealingPotion potion = (HealingPotion)cboPotions.SelectedItem;

                _player.UsePotion(potion);
                
            }
        }

        private void btnTrade_Click(object sender, EventArgs e)
        {
            TradingScreen tradingScreen = new TradingScreen(_player);
            tradingScreen.StartPosition = FormStartPosition.CenterScreen;
            tradingScreen.ShowDialog(this);
        }

        private void btnWorldMap_Click(object sender, EventArgs e)
        {
            WorldMap mapScreen = new WorldMap(_player);
            mapScreen.StartPosition = FormStartPosition.CenterScreen;
            mapScreen.ShowDialog(this);
        }

        private void SuperAdventure_FormClosing(object sender, FormClosingEventArgs e)
        {
            _player.ToJsonString();
            _player.PropertyChanged -= PlayerOnPropertyChanged;
            _player.OnMessage -= DisplayMessage;
            lblHitPoints.DataBindings.Clear();
            lblGold.DataBindings.Clear();
            lblExperience.DataBindings.Clear();
            lblLevel.DataBindings.Clear();
            btnEast.Click -= btnEast_Click;
            btnNorth.Click -= btnNorth_Click;
            btnSouth.Click -= btnSouth_Click;
            btnWest.Click -= btnWest_Click;
            btnUseWeapon.Click -= btnUseWeapon_Click;
            btnUsePotion.Click -= btnUsePotion_Click;
            btnTrade.Click -= btnTrade_Click;
            btnWorldMap.Click -= btnWorldMap_Click;

            cboWeapons.SelectedIndexChanged -= cboWeapons_SelectedIndexChanged;
            cboPotions.SelectedIndexChanged -= btnUsePotion_Click;

            dgvInventory.DataSource = null;
            dgvQuests.DataSource = null;
            cboWeapons.DataSource = null;
            cboPotions.DataSource = null;

            // Dispose other controls, if necessary
            dgvInventory.Dispose();
            dgvQuests.Dispose();
            cboWeapons.Dispose();
            cboPotions.Dispose();

            this.Dispose();
            this.Close();

        }

        private void cboWeapons_SelectedIndexChanged(object sender, EventArgs e)
        {

            _player.CurrentWeapon = (Weapon)cboWeapons.SelectedItem;
        
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            vScrollBar1.Minimum = 0;
            vScrollBar1.Maximum = dgvQuests.RowCount - 1;
            vScrollBar1.LargeChange = 1;
            vScrollBar1.SmallChange = 1;

            //vScrollBar1.Scroll += new ScrollEventHandler(vScrollBar1_Scroll);

            if (e.NewValue >= 0 && e.NewValue < dgvQuests.RowCount)
            {
                // Die erste angezeigte Zeile im DataGridView wird entsprechend der Scrollbar angepasst
                dgvQuests.FirstDisplayedScrollingRowIndex = e.NewValue;
            }
        }

        private void vScrollBar2_Scroll(object sender, ScrollEventArgs e)
        {
            vScrollBar1.Minimum = 0;
            vScrollBar1.Maximum = dgvInventory.RowCount - 1;
            vScrollBar1.LargeChange = 1;
            vScrollBar1.SmallChange = 1;

            //vScrollBar1.Scroll += new ScrollEventHandler(vScrollBar1_Scroll);

            if (e.NewValue >= 0 && e.NewValue < dgvInventory.RowCount)
            {
                // Die erste angezeigte Zeile im DataGridView wird entsprechend der Scrollbar angepasst
                dgvInventory.FirstDisplayedScrollingRowIndex = e.NewValue;
            }
        }

    }
}