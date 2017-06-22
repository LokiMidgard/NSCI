using NSCI;
using System;
using System.Text;
using NSCI.UI.Controls;
using System.Threading.Tasks;
using NSCI.UI;
using NSCI.UI.Controls.Layout;
using System.Reflection;
using System.Linq;

namespace TestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //Console.OutputEncoding = Encoding.UTF8;

            //Console.ForegroundColor = ConsoleColor.White;
            //Console.BackgroundColor = ConsoleColor.Black;
            //var t = typeof(NSCI.Characters.Box);
            //var fileds = t.GetFields().Where(x => x.FieldType == typeof(char)).OrderBy(x => x.Name).ToArray();
            //Console.BufferHeight = fileds.Length + 1;
            //int i=0;
            //foreach (var field in fileds)
            //{
            //    i++;
            //    var c = (char)field.GetRawConstantValue();

            //    Console.WriteLine($"{field.Name,-80}{c}");
            //    //System.Threading.Thread.Sleep(15);
            //    Console.BackgroundColor = i % 2 == 0 ? ConsoleColor.Black : ConsoleColor.DarkBlue;

            //}
            //Console.ReadKey(true);

            //foreach (SpecialChars item in Enum.GetValues(typeof(SpecialChars)))
            //{
            //    Console.WriteLine($"{Enum.GetName(typeof(SpecialChars), item),-40}: {(char)item}");
            //    System.Threading.Thread.Sleep(300);
            //}
            //Console.ReadKey(true);


            var root = new NSCI.UI.RootWindow();

            var text = new NSCI.UI.Controls.TextBlock() { Text = "Hallo Welt!", Height = 3 };
            var border = new Border() { Style = NSCI.UI.Controls.BorderStyle.Block, Background = ConsoleColor.Green, Foreground = ConsoleColor.Black };
            border.Content = text;
            var text2 = new NSCI.UI.Controls.TextBlock() { Text = "Hello World!" };

            var button1 = new Button() { Width = 40, HorizontalAlignment = HorizontalAlignment.Left };
            var button2 = new Button() { HorizontalAlignment = HorizontalAlignment.Center };
            var button3 = new Button() { HorizontalAlignment = HorizontalAlignment.Center };

            button1.Text = "Button 1";
            button2.Text = "Button 2";
            button3.Text = "Button 3";

            var scroll = new ScrollView();
            var text3 = new NSCI.UI.Controls.TextBlock() { Text = "Hello World! Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.\n\nDuis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat,\nvel illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesent luptatum zzril delenit augue duis dolore te feugait nulla facilisi.Lorem ipsum dolor sit amet,\nconsectetuer adipiscing elit,\nsed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat.\n\nUt wisi enim ad minim veniam,\nquis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat.Duis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat,\nvel illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesent luptatum zzril delenit augue duis dolore te feugait nulla facilisi.\n\nNam liber tempor cum soluta nobis eleifend option congue nihil imperdiet doming id quod mazim placerat facer" };
            scroll.Content = text3;

            var grid = new Grid();

            grid.ColumnDefinitions.Add(new RelativSizeDefinition() { Size = 1 });
            grid.ColumnDefinitions.Add(new FixSizeDefinition() { Size = 20 });
            grid.ColumnDefinitions.Add(new AutoSizeDefinition());
            grid.ColumnDefinitions.Add(new RelativSizeDefinition() { Size = 1 });

            grid.RowDefinitions.Add(new FixSizeDefinition() { Size = 5 });
            grid.RowDefinitions.Add(new AutoSizeDefinition());
            grid.RowDefinitions.Add(new AutoSizeDefinition());

            grid.Items.Add(button1);
            grid.Items.Add(button2);
            grid.Items.Add(button3);
            grid.Items.Add(scroll);

            Grid.Column[button1].Value = 0;
            Grid.Column[button2].Value = 1;

            Grid.Column[button3].Value = 0;
            Grid.ColumnSpan[button3].Value = 2;
            Grid.Row[button3].Value = 2;
            Grid.Row[button1].Value = 1;
            Grid.Row[button2].Value = 1;

            Grid.ColumnSpan[scroll].Value = 2;
            Grid.Row[scroll].Value = 0;

            var stack = new StackPanel();

            stack.Items.Add(border);
            stack.Items.Add(grid);
            //stack.Items.Add(/*grid*/);
            //stack.Items.Add(button1);
            //stack.Items.Add(button2);



            stack.Items.Add(text2);

            root.Content = stack;

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