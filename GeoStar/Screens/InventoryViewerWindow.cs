using System;

using Microsoft.Xna.Framework;

using SadConsole;
using SadConsole.Controls;

using GeoStar.Entities;
using GeoStar.Items;
using GeoStar.MapObjects;

namespace GeoStar.Screens
{
    class InventoryViewerWindow : Window
    {
        ListBox listBox_SelfInventory;
        ListBox listBox_GroundInventory;

        bool isPickup = false;
        bool isPickupAll = false;
        bool isDrop = false;
        bool isDropAll = false;

        public InventoryViewerWindow(string title = "Item") : base(40, 12)
        {
            Title = title;
            TitleAlignment = System.Windows.HorizontalAlignment.Center;


            listBox_SelfInventory = new SadConsole.Controls.ListBox(18, 10)
            {
                Position = new Microsoft.Xna.Framework.Point(0, 2)
            };
            Print(1, 1, "Self");

            listBox_GroundInventory = new SadConsole.Controls.ListBox(18, 10)
            {
                Position = new Microsoft.Xna.Framework.Point(22, 2)
            };
            Print(22, 1, "Gnd");

            NormalButton button_pickup = new NormalButton(4)
            {
                Text = " << ",
                Position = new Microsoft.Xna.Framework.Point(18, 3)
            };
            button_pickup.MouseButtonClicked += (o, e) =>
            {
                isPickup = true;
            };

            NormalButton button_pickup_all = new NormalButton(4)
            {
                Text = "<<<<",
                Position = new Microsoft.Xna.Framework.Point(18, 5)
            };
            button_pickup_all.MouseButtonClicked += (o, e) =>
            {
                isPickupAll = true;
            };

            NormalButton button_drop = new NormalButton(4)
            {
                Text = " >> ",
                Position = new Microsoft.Xna.Framework.Point(18, 7)
            };
            button_drop.MouseButtonClicked += (o, e) =>
            {
                isDrop = true;
            };

            NormalButton button_drop_all = new NormalButton(4)
            {
                Text = ">>>>",
                Position = new Microsoft.Xna.Framework.Point(18, 9)
            };
            button_drop_all.MouseButtonClicked += (o, e) =>
            {
                isDropAll = true;
            };

            Add(listBox_SelfInventory);
            Add(listBox_GroundInventory);
            Add(button_pickup);
            Add(button_pickup_all);
            Add(button_drop);
            Add(button_drop_all);
        }

        EntityBase sel = null;
        Floor gnd = null;

        public void Show(EntityBase self, Map map, bool modal = false)
        {
            sel = self;
            UpdateListBox(listBox_SelfInventory, self.Inventory);

            var pos = self.Position.Y * map.Width + self.Position.X;
            gnd = map.Tiles[pos] as Floor;
            UpdateListBox(listBox_GroundInventory, gnd.Inventory);

            gnd.Inventory.ItemAdded += (o, e) =>
            {
                gnd.Foreground = Color.White;
                gnd.Glyph = 15;
            };

            gnd.Inventory.ItemRemoved += (o, e) =>
            {
                if (gnd.Inventory.Count() == 0)
                {
                    gnd.Foreground = gnd.DefaultForeground;
                    gnd.Glyph = 46;
                }
            };

            base.Show(modal);
        }

        private void UpdateListBox(ListBox listBox, Inventory inv)
        {
            listBox.Items.Clear();

            foreach (var item in inv)
            {
                listBox.Items.Add(item.ToString(16));
            }
        }

        private void UpdateBothListBox()
        {
            UpdateListBox(listBox_SelfInventory, sel.Inventory);
            UpdateListBox(listBox_GroundInventory, gnd.Inventory);
        }

        public override void Hide()
        {
            sel?.Inventory.RemoveAllEventListener();
            gnd?.Inventory.RemoveAllEventListener();

            sel = null;
            gnd = null;

            base.Hide();
        }

        public override void Update(TimeSpan time)
        {
            if (IsVisible)
            {
                if (SadConsole.Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Enter))
                {
                    DialogResult = true;
                    Hide();
                }
                if (SadConsole.Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Back))
                {
                    DialogResult = false;
                    Hide();
                }

                if (isPickup)
                {
                    var selectedItem = listBox_GroundInventory.SelectedItem;
                    if (selectedItem != null)
                    {
                        gnd.Inventory.TransferItem(sel.Inventory, selectedItem.ToString().GetUntilOrEmpty(" "), 1);
                        UpdateBothListBox();
                    }
                    isPickup = false;
                }

                if (isPickupAll)
                {
                    foreach (var item in listBox_GroundInventory.Items)
                    {
                        int.TryParse(item.ToString().GetFromBackUntilOrEmpty(), out int amount);
                        gnd.Inventory.TransferItem(sel.Inventory, item.ToString().GetUntilOrEmpty(" "), amount);
                    }
                    UpdateBothListBox();
                    isPickupAll = false;
                }

                if (isDrop)
                {
                    var selectedItem = listBox_SelfInventory.SelectedItem;
                    if (selectedItem != null)
                    {
                        sel.Inventory.TransferItem(gnd.Inventory, selectedItem.ToString().GetUntilOrEmpty(" "), 1);
                        UpdateBothListBox();
                    }
                    isDrop = false;
                }

                if (isDropAll)
                {
                    foreach (var item in listBox_SelfInventory.Items)
                    {
                        int.TryParse(item.ToString().GetFromBackUntilOrEmpty(), out int amount);
                        sel.Inventory.TransferItem(gnd.Inventory, item.ToString().GetUntilOrEmpty(" "), amount);
                    }
                    UpdateBothListBox();
                    isDropAll = false;
                }
            }

            base.Update(time);
        }
    }
}
