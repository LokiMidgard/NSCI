using System;
using NDProperty.Providers.Binding;
using NSCI.Sample.Viewmodel;
using NSCI.UI;
using NSCI.UI.Controls;
using NSCI.UI.Controls.Layout;

namespace NSCI.Sample
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var model = Viewmodel.PersonList.Load(new System.IO.FileInfo("test.xml"));

            var root = new NSCI.UI.RootWindow();

            var grid = new Grid();
            root.Content = grid;

            grid.RowDefinitions.Add(new AutoSizeDefinition());
            grid.RowDefinitions.Add(new RelativSizeDefinition() { Size = 1 });
            grid.RowDefinitions.Add(new AutoSizeDefinition());
            grid.RowDefinitions.Add(new FixSizeDefinition() { Size = 1 });

            grid.ColumnDefinitions.Add(new AutoSizeDefinition());
            grid.ColumnDefinitions.Add(new RelativSizeDefinition() { Size = 1 });
            grid.ColumnDefinitions.Add(new AutoSizeDefinition());

            // Form
            {
                var contentPane = new Border() { BorderStyle = BorderStyle.DoubleLined, Background = ConsoleColor.DarkCyan };
                Grid.Row[contentPane].Value = 1;
                Grid.ColumnSpan[contentPane].Value = 3;
                grid.Children.Add(contentPane);


                var form = new Grid();
                contentPane.Child = form;

                form.RowDefinitions.Add(new AutoSizeDefinition());
                form.RowDefinitions.Add(new AutoSizeDefinition());
                form.RowDefinitions.Add(new AutoSizeDefinition());
                form.RowDefinitions.Add(new AutoSizeDefinition());
                form.RowDefinitions.Add(new AutoSizeDefinition());
                form.RowDefinitions.Add(new RelativSizeDefinition() { Size = 1 });

                form.ColumnDefinitions.Add(new AutoSizeDefinition());
                form.ColumnDefinitions.Add(new RelativSizeDefinition() { Size = 1 });


                var title = new TextBlock()
                {
                    Margin = new Thickness(1, 1, 0, 3)
                };
                TextBlock.TextProperty.Bind(title, PersonList.CurrentReadOnlyProperty.Of(model).Over(Person.NameReadOnlyProperty).OneWay());
                Grid.ColumnSpan[title].Value = 2;
                Grid.Row[title].Value = 0;
                form.Children.Add(title);


                var surNameLable = new TextBlock()
                {
                    Height = 1
                };
                Grid.Column[surNameLable].Value = 0;
                Grid.Row[surNameLable].Value = 1;
                surNameLable.Text = "Sure Name";
                form.Children.Add(surNameLable);

                var surNameEdit = new TextBox()
                {
                    Margin = new Thickness(2,1,0,1),
                    Height = 1
                };
                Grid.Column[surNameEdit].Value = 1;
                Grid.Row[surNameEdit].Value = 1;
                TextBox.TextProperty.Bind(surNameEdit, PersonList.CurrentReadOnlyProperty.Of(model).Over(Person.SureNameProperty).TwoWay());
                form.Children.Add(surNameEdit);



                var firstNameLable = new TextBlock()
                {
                    Height = 1
                };
                Grid.Column[firstNameLable].Value = 0;
                Grid.Row[firstNameLable].Value = 2;
                firstNameLable.Text = "First Name";
                form.Children.Add(firstNameLable);

                var firstNameEdit = new TextBox()
                {
                    Margin = new Thickness(2,1,0,1),
                    Height = 1
                };
                Grid.Column[firstNameEdit].Value = 1;
                Grid.Row[firstNameEdit].Value = 2;
                TextBox.TextProperty.Bind(firstNameEdit, PersonList.CurrentReadOnlyProperty.Of(model).Over(Person.FirstNameProperty).TwoWay());
                form.Children.Add(firstNameEdit);




                var streetLable = new TextBlock()
                {
                    Height = 1
                };
                Grid.Column[streetLable].Value = 0;
                Grid.Row[streetLable].Value = 3;
                streetLable.Text = "Street";
                form.Children.Add(streetLable);

                var streetEdit = new TextBox()
                {
                    Margin = new Thickness(2,1,0,1),
                    Height = 1
                };
                Grid.Column[streetEdit].Value = 1;
                Grid.Row[streetEdit].Value = 3;
                TextBox.TextProperty.Bind(streetEdit, PersonList.CurrentReadOnlyProperty.Of(model).Over(Person.StreetProperty).TwoWay());
                form.Children.Add(streetEdit);




                var cityLable = new TextBlock()
                {
                    Height = 1
                };
                Grid.Column[cityLable].Value = 0;
                Grid.Row[cityLable].Value = 4;
                cityLable.Text = "City";
                form.Children.Add(cityLable);

                var cityEdit = new TextBox()
                {
                    Margin = new Thickness(2,1,0,1),
                    Height = 1
                };
                Grid.Column[cityEdit].Value = 1;
                Grid.Row[cityEdit].Value = 4;
                TextBox.TextProperty.Bind(cityEdit, PersonList.CurrentReadOnlyProperty.Of(model).Over(Person.CityProperty).TwoWay());
                form.Children.Add(cityEdit);




            }


            // Previous Button
            {
                var previousButton = new Button()
                {
                    Content = "Prev.",
                    Margin = new Thickness(1)
                };
                Grid.Row[previousButton].Value = 0;
                Grid.Column[previousButton].Value = 0;
                grid.Children.Add(previousButton);
                FrameworkElement.IsEnabledProperty.Bind(previousButton as FrameworkElement, Viewmodel.PersonList.HasPreviousReadOnlyProperty.Of(model).OneWay());
                previousButton.ButtonPressed += (sender, e) => model.CurrentSelectedIndex--;
            }

            // next Button
            {
                var nextButton = new Button()
                {
                    Content = "next",
                    Margin = new Thickness(1)

                };
                Grid.Row[nextButton].Value = 0;
                Grid.Column[nextButton].Value = 2;
                grid.Children.Add(nextButton);
                FrameworkElement.IsEnabledProperty.Bind(nextButton as FrameworkElement, Viewmodel.PersonList.HasNextReadOnlyProperty.Of(model).OneWay());
                nextButton.ButtonPressed += (sender, e) => model.CurrentSelectedIndex++;
            }

            // new Button
            {
                var nextButton = new Button()
                {
                    Content = "new",
                    Margin = new Thickness(1)

                };
                Grid.Row[nextButton].Value = 0;
                Grid.Column[nextButton].Value = 1;
                grid.Children.Add(nextButton);

                nextButton.ButtonPressed += (sender, e) => model.NewPerson();
            }

            root.Run();


            //var text = new NSCI.UI.Controls.TextBlock() { Text = "Hallo Welt!1", Height = 3 };
            //var border = new Border() { BorderStyle = NSCI.UI.Controls.BorderStyle.Block, Foreground = ConsoleColor.Black };
            //border.Child = text;
            ////var text2 = new NSCI.UI.Controls.TextBlock() { Text = "Hello World!2" };

            //////TextBlock.TextProperty.Bind(text2,
            //////    ContentControl.ContentProperty
            //////    .Of(root)
            //////    .Over(UIElement.DepthReadOnlyProperty)
            //////    .ConvertOneWay(x => x.ToString())
            //////    );
            //////TextBlock.TextProperty.Bind(text,
            //////    ContentControl.ContentProperty
            //////    .Of(root)
            //////    .Over(UIElement.IsVisibleProperty)
            //////    .ConvertTwoWay(x => x.ToString(), x => bool.Parse(x))
            //////    );

            //var button1 = new Button() { Width = 40, HorizontalAlignment = HorizontalAlignment.Left };
            //var button2 = new Button() { HorizontalAlignment = HorizontalAlignment.Center };
            //var button3 = new Button() { HorizontalAlignment = HorizontalAlignment.Center };

            //button1.Content = "Button 1";
            //button2.Content = "Button 2";
            //button3.Content = "Button 3";

            //////var scroll = new ScrollView();
            //////var text3 = new NSCI.UI.Controls.TextBlock() { Text = "Hello World! Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.\n\nDuis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat,\nvel illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesent luptatum zzril delenit augue duis dolore te feugait nulla facilisi.Lorem ipsum dolor sit amet,\nconsectetuer adipiscing elit,\nsed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat.\n\nUt wisi enim ad minim veniam,\nquis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat.Duis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat,\nvel illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesent luptatum zzril delenit augue duis dolore te feugait nulla facilisi.\n\nNam liber tempor cum soluta nobis eleifend option congue nihil imperdiet doming id quod mazim placerat facer" };
            //////scroll.Content = text3;

            //var grid = new Grid();

            //grid.ColumnDefinitions.Add(new RelativSizeDefinition() { Size = 1 });
            //grid.ColumnDefinitions.Add(new FixSizeDefinition() { Size = 20 });
            //grid.ColumnDefinitions.Add(new AutoSizeDefinition());
            //grid.ColumnDefinitions.Add(new RelativSizeDefinition() { Size = 1 });

            //grid.RowDefinitions.Add(new FixSizeDefinition() { Size = 5 });
            //grid.RowDefinitions.Add(new AutoSizeDefinition());
            //grid.RowDefinitions.Add(new AutoSizeDefinition());

            //grid.Children.Add(button1);
            //grid.Children.Add(button2);
            //grid.Children.Add(button3);
            ////grid.Children.Add(scroll);

            //Grid.Column[button1].Value = 0;
            //Grid.Column[button2].Value = 1;

            //Grid.Column[button3].Value = 0;
            //Grid.ColumnSpan[button3].Value = 2;
            //Grid.Row[button3].Value = 2;
            //Grid.Row[button1].Value = 1;
            //Grid.Row[button2].Value = 1;

            ////Grid.ColumnSpan[scroll].Value = 2;
            ////Grid.Row[scroll].Value = 0;

            //var checkbox = new CheckBox();

            //checkbox.Content = new TextBlock() { Text = "Test Slection" };

            //var stack = new StackPanel();

            //stack.Children.Add(checkbox);
            //stack.Children.Add(border);
            //stack.Children.Add(grid);

            //var textblock = new TextBlock() { Background = ConsoleColor.DarkRed, Width = 40, Height = 1, Text = "HEY" };
            //var textbox = new TextBox() { Background = ConsoleColor.DarkRed, Width = 40, Height = 1 };

            //stack.Children.Add(textbox);
            //stack.Children.Add(textblock);
            ////stack.Items.Add(/*grid*/);
            ////stack.Items.Add(button1);
            ////stack.Items.Add(button2);




            //root.Content = stack;

            ////root.BeforeStart += async () =>
            ////  {
            ////      var styles = (NSCI.UI.Controls.BorderStyle[])Enum.GetValues(typeof(NSCI.UI.Controls.BorderStyle));
            ////      while (root.Running) // this endless loop would prevent the App from quitting :/ Need to handle on framework level.
            ////      {
            ////          foreach (var s in styles)
            ////          {
            ////              if (!root.Running)
            ////                  break;
            ////              await Task.Delay(1000);
            ////              border.Style = s;
            ////          }
            ////      }
            ////  };

            ////var dialog = new Dialog(root) { Text = "Hello World!", Width = 60, Height = 32, Top = 4, Left = 4, Border = BorderStyle.Thick };
            ////new Label(dialog) { Text = "This is a dialog!", Top = 2, Left = 2 };
            ////var button = new Button(dialog) { Text = "Oooooh", Top = 4, Left = 6 };
            ////var button2 = new Button(dialog) { Text = "Click", Top = 4, Left = 18 };
            ////var list = new ListBox(dialog) { Top = 10, Left = 4, Width = 32, Height = 6, Border = BorderStyle.Thin };
            ////var line = new VerticalLine(dialog) { Top = 4, Left = 40, Width = 1, Height = 6, Border = BorderStyle.Thick };

            ////var dialog2 = new Dialog(root) { Text = "ooooh", Width = 32, Height = 10, Top = 6, Left = 6, Border = BorderStyle.Thick, Visible = false };
            ////var button3 = new Button(dialog2) { Text = "Bye!", Width = 8, Height = 3, Top = 1, Left = 1 };

            ////var text = new NSCI.Widgets.SingleLineTextbox(dialog2) { Width = 28, Height = 3, Top = 5, Left = 1 };



            ////button3.Clicked += (s, e) => { dialog2.Hide(); dialog.Show(); };
            ////button2.Clicked += (s, e) => { dialog.Hide(); dialog2.Show(); };

            ////for (var i = 0; i < 25; i++)
            ////{
            ////    list.Items.Add("Item " + i.ToString());
            ////}

            ////button.Clicked += button_Clicked;




            //root.Run();
        }
        //static void button_Clicked(object sender, EventArgs e)
        //{
        //    (sender as Button).RootWindow.Detach();
        //}
    }
}