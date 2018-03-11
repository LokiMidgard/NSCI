using NSCI;
using System;
using System.Text;
using NSCI.UI.Controls;
using System.Threading.Tasks;
using NSCI.UI;
using NSCI.UI.Controls.Layout;
using System.Reflection;
using System.Linq;
using NDProperty.Providers.Binding;

namespace TestHarness
{
    partial class MyClass
    {

        static MyClass()
        {
            Template.SetDefaultDataTemplate(Template.CreateDataTemplate((MyClass obj) =>
            {
                var txt = new TextBlock();

                TextBlock.TextProperty.Bind(txt, MyClass.TextProperty.Of(obj).ConvertOneWay(x => x));

                return txt;
            }));
        }

        [NDProperty.NDP]
        private void OnTextChanging(global::NDProperty.Propertys.OnChangingArg<NSCI.Propertys.NDPConfiguration, string> arg)
        {
        }

    }
    class Program
    {


        static void Main(string[] args)
        {



            var root = new NSCI.UI.RootWindow();


            var list = new SingelSelectionItemsControl<MyClass>();


            list.Items = new MyClass[] {
                new MyClass(){Text="Test 1" },
                new MyClass(){Text="Test 2"}
            };

            var border2 = new Border() { BorderStyle = NSCI.UI.Controls.BorderStyle.DoubleLined, Foreground = ConsoleColor.Yellow };
            border2.Child = list;

            var text = new NSCI.UI.Controls.TextBox() { Height = 3 };
            var border1 = new Border() { BorderStyle = NSCI.UI.Controls.BorderStyle.DoubleLined, Foreground = ConsoleColor.DarkYellow };
            border1.Child = text;

            TextBox.TextProperty.Bind(text, SingelSelectionItemsControl<MyClass>.SelectedItemProperty.Of(list).Over(MyClass.TextProperty).TwoWay());

            var grid = new Grid();

            grid.ColumnDefinitions.Add(new RelativSizeDefinition() { Size = 1 });
            //grid.ColumnDefinitions.Add(new FixSizeDefinition() { Size = 20 });
            //grid.ColumnDefinitions.Add(new AutoSizeDefinition());
            grid.ColumnDefinitions.Add(new RelativSizeDefinition() { Size = 1 });

            //grid.RowDefinitions.Add(new FixSizeDefinition() { Size = 5 });
            //grid.RowDefinitions.Add(new AutoSizeDefinition());
            //grid.RowDefinitions.Add(new AutoSizeDefinition());

            //grid.Children.Add(button1);
            //grid.Children.Add(button2);
            //grid.Children.Add(button3);
            grid.Children.Add(border1);
            grid.Children.Add(border2);

            Grid.Column[border2].Value = 0;
            Grid.Column[border1].Value = 1;

            //Grid.Column[button3].Value = 0;
            //Grid.ColumnSpan[button3].Value = 2;
            //Grid.Row[button3].Value = 2;
            //Grid.Row[button1].Value = 1;
            //Grid.Row[button2].Value = 1;

            //Grid.ColumnSpan[scroll].Value = 2;
            //Grid.Row[scroll].Value = 0;







            root.Content = grid;

            //root.BeforeStart += async () =>
            //  {
            //      var styles = (NSCI.UI.Controls.BorderStyle[])Enum.GetValues(typeof(NSCI.UI.Controls.BorderStyle));
            //      while (root.Running) // this endless loop would prevent the App from quitting :/ Need to handle on framework level.
            //      {
            //          foreach (var s in styles)
            //          {
            //              if (!root.Running)
            //                  break;
            //              await Task.Delay(1000);
            //              border.Style = s;
            //          }
            //      }
            //  };

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
        //static void button_Clicked(object sender, EventArgs e)
        //{
        //    (sender as Button).RootWindow.Detach();
        //}
    }
}