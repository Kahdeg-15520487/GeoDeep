using GeoStar.Entities;
using SadConsole;
using SadConsole.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoStar.Screens
{
    class InventoryViewerWindow : Window
    {
        SadConsole.Controls.ListBox listBox_SelfInventory;
        SadConsole.Controls.ListBox listBox_GroundInventory;

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
                Text = " >> ",
                Position = new Microsoft.Xna.Framework.Point(18, 3)
            };

            NormalButton button_drop = new NormalButton(4)
            {
                Text = " << ",
                Position = new Microsoft.Xna.Framework.Point(18, 6)
            };

            Add(listBox_SelfInventory);
            Add(listBox_GroundInventory);
            Add(button_pickup);
            Add(button_drop);
        }

        public void Show(Inventory selfInv, Inventory gndInv, bool modal = false)
        {
            listBox_SelfInventory.Items.Clear();
            listBox_GroundInventory.Items.Clear();

            foreach (var item in selfInv)
            {
                listBox_SelfInventory.Items.Add(item.ToString(16));
            }

            foreach (var item in gndInv)
            {
                listBox_GroundInventory.Items.Add(item.ToString(16));
            }

            base.Show(modal);
        }

        public override void Update(TimeSpan time)
        {
            if (SadConsole.Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Enter))
            {
                Hide();
            }

            base.Update(time);
        }
    }
}
