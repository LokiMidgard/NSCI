﻿using NSCI;
using NSCI.Widgets;
using System;
using System.Text;
using NSCI.UI.Controls.Layput;

namespace TestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            var root = new RootWindow();

            var text = new NSCI.UI.Controls.TextBlock() { Text = "Hallo Welt!"  , Background = ConsoleColor.DarkYellow};
            var text2 = new NSCI.UI.Controls.TextBlock() { Text = "Hello World!" , Background= ConsoleColor.Green};

            var stack = new StackPanel();

            stack.Items.Add(text);
            stack.Items.Add(text2);

            root.Content = stack;

            //var dialog = new Dialog(root) { Text = "Hello World!", Width = 60, Height = 32, Top = 4, Left = 4, Border = BorderStyle.Thick };
            //new Label(dialog) { Text = "This is a dialog!", Top = 2, Left = 2 };
            //var button = new Button(dialog) { Text = "Oooooh", Top = 4, Left = 6 };
            //var button2 = new Button(dialog) { Text = "Click", Top = 4, Left = 18 };
            //var list = new ListBox(dialog) { Top = 10, Left = 4, Width = 32, Height = 6, Border = BorderStyle.Thin };
            //var line = new VerticalLine(dialog) { Top = 4, Left = 40, Width = 1, Height = 6, Border = BorderStyle.Thick };

            //var dialog2 = new Dialog(root) { Text = "ooooh", Width = 32, Height = 10, Top = 6, Left = 6, Border = BorderStyle.Thick, Visible = false };
            //var button3 = new Button(dialog2) { Text = "Bye!", Width = 8, Height = 3, Top = 1, Left = 1 };

            //var text = new NSCI.Widgets.SingleLineTextbox(dialog2) { Width = 28, Height = 3, Top = 5, Left = 1 };



            //button3.Clicked += (s, e) => { dialog2.Hide(); dialog.Show(); };
            //button2.Clicked += (s, e) => { dialog.Hide(); dialog2.Show(); };

            //for (var i = 0; i < 25; i++)
            //{
            //    list.Items.Add("Item " + i.ToString());
            //}

            //button.Clicked += button_Clicked;




            root.Run();
        }
        static void button_Clicked(object sender, EventArgs e)
        {
            (sender as Button).RootWindow.Detach();
        }
    }
}