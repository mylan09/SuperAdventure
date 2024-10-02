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
    public partial class ChestScreen : Form
    {
        private InventoryChest _chest;
        private Player _currentPlayer;

        public ChestScreen(Player player, InventoryChest chest)
        {
            _currentPlayer = player;
            _chest = chest;

            InitializeComponent();
            InitializeDataGridViews();
        }

        private void InitializeDataGridViews()
        {
            // Style for numeric column values
            DataGridViewCellStyle rightAlignedCellStyle = new DataGridViewCellStyle();
            rightAlignedCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            // Populate the DataGridView for the player's inventory
            dgvMyItems.RowHeadersVisible = false;
            dgvMyItems.AutoGenerateColumns = false;

            // This hidden column holds the item ID
            dgvMyItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ItemID",
                Visible = false
            });

            dgvMyItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Name",
                Width = 100,
                DataPropertyName = "Description"
            });

            dgvMyItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Qty",
                Width = 30,
                DefaultCellStyle = rightAlignedCellStyle,
                DataPropertyName = "Quantity"
            });

            dgvMyItems.Columns.Add(new DataGridViewButtonColumn
            {
                Text = "Store 1",
                UseColumnTextForButtonValue = true,
                Width = 60,
                DataPropertyName = "ItemID"
            });

            // Bind the player's inventory to the DataGridView
            dgvMyItems.DataSource = _currentPlayer.Inventory;

            // Handle the cell click for storing items
            dgvMyItems.CellClick += dgvMyItems_CellClick;

            // Populate the DataGridView for the chest's inventory
            dgvChestItems.RowHeadersVisible = false;
            dgvChestItems.AutoGenerateColumns = false;

            // Hidden column holds the item ID
            dgvChestItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ItemID",
                Visible = false
            });

            dgvChestItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Name",
                Width = 100,
                DataPropertyName = "Description"
            });

            dgvChestItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Qty",
                Width = 30,
                DefaultCellStyle = rightAlignedCellStyle,
                DataPropertyName = "Quantity"
            });

            dgvChestItems.Columns.Add(new DataGridViewButtonColumn
            {
                Text = "Take 1",
                UseColumnTextForButtonValue = true,
                Width = 60,
                DataPropertyName = "ItemID"
            });

            // Bind the chest's inventory to the DataGridView
            dgvChestItems.DataSource = _currentPlayer.Chest.Inventory; // Bind the chest inventory here

            // Handle the cell click for taking items
            dgvChestItems.CellClick += dgvChestItems_CellClick;
        }
        private void dgvMyItems_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // The 4th column (column index 3) has the "Store 1" button
            if (e.ColumnIndex == 3)
            {
                var itemID = dgvMyItems.Rows[e.RowIndex].Cells[0].Value;

                // Get the item being stored
                Item itemBeingStored = World.ItemByID(Convert.ToInt32(itemID));

                // Remove the item from the player's inventory
                _currentPlayer.RemoveItemFromInventory(itemBeingStored);

                // Add the item to the chest's inventory
                _chest.AddItemToInventoryChest(itemBeingStored);
            }
        }

        private void dgvChestItems_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // The 4th column (column index 3) has the "Take 1" button
            if (e.ColumnIndex == 3)
            {
                var itemID = dgvChestItems.Rows[e.RowIndex].Cells[0].Value;

                // Get the item being taken
                Item itemBeingTaken = World.ItemByID(Convert.ToInt32(itemID));

                // Remove the item from the chest's inventory
                _chest.RemoveItemFromInventoryChest(itemBeingTaken);

                // Add the item to the player's inventory
                _currentPlayer.AddItemToInventory(itemBeingTaken);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
