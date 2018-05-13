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
            //  You need to include this in .csproj if you want source code generation
            // <ItemGroup>
            //  <DotNetCliToolReference Include="dotnet-codegen" Version="0.4.42" />
            //  <PackageReference Include="CodeGeneration.Roslyn.BuildTime" Version="0.4.42" />
            // </ItemGroup>


            // the Root window is the root for our drawing. It will later start the ui thread.
            var root = new NSCI.UI.RootWindow();

            // The root window can only have one child, the Content. So we need another control that will hold more elements.
            var grid = new Grid();
            root.Content = grid;

            // Like in WPF grid has Rows and Colums. 
            // There are 3 diferent row/column-definitions:
            // + AutoSizeDefinition coresponds to WPF "Auto"
            // + FixSizeDefinition corespondens to WPF "3"
            // + RelativSizeDefinition coresponds to WPF "3*"
            grid.RowDefinitions.Add(new AutoSizeDefinition());
            grid.RowDefinitions.Add(new RelativSizeDefinition() { Size = 1 });
            grid.RowDefinitions.Add(new AutoSizeDefinition());
            grid.RowDefinitions.Add(new FixSizeDefinition() { Size = 1 });

            grid.ColumnDefinitions.Add(new AutoSizeDefinition());
            grid.ColumnDefinitions.Add(new RelativSizeDefinition() { Size = 1 });
            grid.ColumnDefinitions.Add(new AutoSizeDefinition());


            // You should not forget to set the Size Property on RelativeSizeDefinition.
            // otherwise the row or column will have always a size of zero

            // This Application will let you show edit and store Addresses in an XML.
            // You will be able to add new entrys browse existing ones and persist changes
            // to the XML.

            // Here we store our data.
            var model = Viewmodel.PersonList.Load(new System.IO.FileInfo("test.xml"));
            // This initialy loads the XML or creates it if not existent.
            // It will allow us to add new entrys get data from already existing
            // and persisting changes.
            // The Model PersonList will contains a short explination how to create
            // your own models that are compatiple with this FrameWork.


            // In the nex block creates the parts of the View that will show the information to one address
            // and alowes us to change it.
            {
                // We will show the Address in a box that is decorated with a double lined Border.
                var contentPane = new Border()
                {
                    BorderStyle = BorderStyle.DoubleLined,
                    TabIndex = 4,
                    Background = ConsoleColor.DarkCyan
                };
                // We will add this contentPane to our grid.
                // Like in WPF we need to set the Row and Column 
                // Property for the contentPane in order to draw
                // it at the correct Position.
                // We use Static Propertys on the Grid to dor this.
                Grid.Row[contentPane].Value = 1;
                Grid.ColumnSpan[contentPane].Value = 3;
                // We also need to actually add the contentPane to our grid.
                grid.Children.Add(contentPane);

                // initially there won't be any Address so we want to disable all address related fields.
                // There are two propertys on FrameworkElements that control the enabled/disabled state.
                // IsEnabled and IsDisabled. IsEnabled is a writable property that tells if this control 
                // was explicitly enabled or disabled. IsDisabled however tells us if a control is actual
                // disabled. 
                // If we set the IsEnabled property on contentPane to false, the IsDisabled property will be true.
                // For all children of contentPane the IsEnabled property is still true, because they were not 
                // explicitly disabled. However the IsDisabled property on all childrean is also true, because
                // this property is enhireted from its parent. The Naming is far from good and will change in
                // the future.
                // The following line will initiate the binding of the IsEnabled property to the Current property
                // of our model.
                FrameworkElement.IsEnabledProperty.Bind(contentPane as FrameworkElement, PersonList.CurrentReadOnlyProperty.Of(model).ConvertOneWay(x => x != null));
                // This Framework uses the NDProperty framework. This allows for Binding, inheritance
                // and some other neat features. For every Property there is an static field with "<PropertyName>Property" 
                // or "<PropertyName>ReadonlyProperty".  On this object we can call the Extension Method Bind(...).
                // The first parameter will be the Object on which the property exists we Bind. The seccond Parameter is 
                // a BindingConfiguration. This is simular to the Path property in WPF Binding. We use the
                // Of(...) extneions method on another NDProperty. 
                // In our case this is the CurrentReadOnlyProperty of model. Because we try to Bind an object of type 
                // Person to bool we need a Converter. If the types would be compatible we could just call the 
                // mehtod OneWay(...).

                // Because of generics, we need to cast our contentPane to the Type where IsEnabledProperty is defined.
                // Thats why we need the "as FrameworkElement".


                // Agian Border can only have one child, so we need a grid to put in multiple elements.
                // We call it form:
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
                    Height = 1,
                    Margin = new Thickness(1, 1, 0, 2)
                };
                // The following will Bind to the Name property of a Person stored in our modl.
                // The Path for binding starts always with the Of(...) method and ends with:
                // + OneWay()
                // + OneWayConvert(...)
                // + TwoWay()
                // + TwoWayConvert(...)
                // To change different Propertys together we can use the Over method. This Binding
                // would be model.Current.Name
                TextBlock.TextProperty.Bind(title, PersonList.CurrentReadOnlyProperty.Of(model).Over(Person.NameReadOnlyProperty).OneWay());
                Grid.ColumnSpan[title].Value = 2;
                Grid.Row[title].Value = 0;
                form.Children.Add(title);


                var surNameLable = new TextBlock()
                {
                    Height = 1,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.Column[surNameLable].Value = 0;
                Grid.Row[surNameLable].Value = 1;
                surNameLable.Text = "Sure Name";
                form.Children.Add(surNameLable);

                var surNameEdit = new TextBox()
                {
                    TabIndex = 3,
                    Margin = new Thickness(2, 1, 0, 1),
                    Height = 1
                };
                Grid.Column[surNameEdit].Value = 1;
                Grid.Row[surNameEdit].Value = 1;
                // This will bind the Property TwoWay. When changes are applied to the TextProperty
                // those will be set on the SureNameProperty as Local Values. Changes in SureNameProeprty
                // however will not result in the change of the local Value of TextProperty but the Binding Value
                TextBox.TextProperty.Bind(surNameEdit, PersonList.CurrentReadOnlyProperty.Of(model).Over(Person.SureNameProperty).TwoWay());
                form.Children.Add(surNameEdit);



                var firstNameLable = new TextBlock()
                {
                    Height = 1,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.Column[firstNameLable].Value = 0;
                Grid.Row[firstNameLable].Value = 2;
                firstNameLable.Text = "First Name";
                form.Children.Add(firstNameLable);

                var firstNameEdit = new TextBox()
                {
                    TabIndex = 4,
                    Margin = new Thickness(2, 1, 0, 1),
                    Height = 1
                };
                Grid.Column[firstNameEdit].Value = 1;
                Grid.Row[firstNameEdit].Value = 2;
                TextBox.TextProperty.Bind(firstNameEdit, PersonList.CurrentReadOnlyProperty.Of(model).Over(Person.FirstNameProperty).TwoWay());
                form.Children.Add(firstNameEdit);




                var streetLable = new TextBlock()
                {
                    Height = 1,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.Column[streetLable].Value = 0;
                Grid.Row[streetLable].Value = 3;
                streetLable.Text = "Street";
                form.Children.Add(streetLable);

                var streetEdit = new TextBox()
                {
                    TabIndex = 5,
                    Margin = new Thickness(2, 1, 0, 1),
                    Height = 1
                };
                Grid.Column[streetEdit].Value = 1;
                Grid.Row[streetEdit].Value = 3;
                TextBox.TextProperty.Bind(streetEdit, PersonList.CurrentReadOnlyProperty.Of(model).Over(Person.StreetProperty).TwoWay());
                form.Children.Add(streetEdit);




                var cityLable = new TextBlock()
                {
                    Height = 1,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.Column[cityLable].Value = 0;
                Grid.Row[cityLable].Value = 4;
                cityLable.Text = "City";
                form.Children.Add(cityLable);

                var cityEdit = new TextBox()
                {
                    TabIndex = 6,
                    Margin = new Thickness(2, 1, 0, 1),
                    Height = 1
                };
                Grid.Column[cityEdit].Value = 1;
                Grid.Row[cityEdit].Value = 4;
                TextBox.TextProperty.Bind(cityEdit, PersonList.CurrentReadOnlyProperty.Of(model).Over(Person.CityProperty).TwoWay());
                form.Children.Add(cityEdit);
            }


            // Previous Button
            var previousButton = new Button()
            {
                TabIndex = 0,
                Content = "Prev.",
                Margin = new Thickness(1)
            };
            Grid.Row[previousButton].Value = 0;
            Grid.Column[previousButton].Value = 0;
            grid.Children.Add(previousButton);

            // Currently there is no command Pattern. So we need to enable the Button via binding
            FrameworkElement.IsEnabledProperty.Bind(previousButton as FrameworkElement, Viewmodel.PersonList.HasPreviousReadOnlyProperty.Of(model).OneWay());
            // and subscripe to events the old way
            previousButton.ButtonPressed += (sender, e) => model.CurrentSelectedIndex--;

            // next Button
            var nextButton = new Button()
            {
                TabIndex = 2,
                Content = "next",
                Margin = new Thickness(1)

            };
            Grid.Row[nextButton].Value = 0;
            Grid.Column[nextButton].Value = 2;
            grid.Children.Add(nextButton);
            FrameworkElement.IsEnabledProperty.Bind(nextButton as FrameworkElement, Viewmodel.PersonList.HasNextReadOnlyProperty.Of(model).OneWay());
            nextButton.ButtonPressed += (sender, e) => model.CurrentSelectedIndex++;

            // new Button
            var newButton = new Button()
            {
                TabIndex = 1,
                Content = "new",
                Margin = new Thickness(1)

            };
            Grid.Row[newButton].Value = 0;
            Grid.Column[newButton].Value = 1;
            grid.Children.Add(newButton);

            newButton.ButtonPressed += (sender, e) => model.NewPerson();

            // new Persist
            var persistButton = new Button()
            {
                TabIndex = 7,
                Content = "Save",
                Margin = new Thickness(1)

            };
            Grid.Row[persistButton].Value = 2;
            Grid.ColumnSpan[persistButton].Value = 3;
            grid.Children.Add(persistButton);
            persistButton.ButtonPressed += (sender, e) => model.Persist();

            // This is the last thing we'll do. This starts the event loop and blocks untill the UI is terminated
            // You should no longer call any Consol methods because these could corupt your UI.
            root.Run();

        }
    }
}