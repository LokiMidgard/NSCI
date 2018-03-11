using System;
using System.Collections.Generic;
using System.Text;
using NDProperty.Providers.Binding;
using NSCI.UI.Controls.Layout;

namespace NSCI.UI.Controls
{
    public class ListItem : ContentControl
    {

        static ListItem()
        {
            Template.SetDefaultControlTemplate(Template.CreateControlTemplate((ListItem item) => {
                var grid = new Grid();

                grid.ColumnDefinitions.Add(new AutoSizeDefinition());
                grid.ColumnDefinitions.Add(new RelativSizeDefinition() { Size = 1 });

                var selection = new TextBlock();


                TextBlock.TextProperty.Bind(selection, ItemsControl.IsSelectedReadOnlyProperty.Of(item).ConvertOneWay(state =>
                {
                    if (state)
                        return NSCI.Characters.Arrows.SINGLE_RIGHT_POINTING_ANGLEK.ToString();
                    else
                        return " ";
                }));

                var content = new ContentPresenter();

                Grid.Column[selection].Value = 0;
                Grid.Column[content].Value = 1;

                grid.Children.Add(selection);
                grid.Children.Add(content);

                return grid;
            }));
        }
                
    }
}
