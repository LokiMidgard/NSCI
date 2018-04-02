using System;
using System.Collections.Generic;
using System.Text;
using NDProperty.Providers.Binding;
using NSCI.UI.Controls.Layout;

namespace NSCI.UI.Controls
{
    public partial class ScrollControl : ContentControl
    {


        [NDProperty.NDP]
        protected virtual void OnVerticalScrollEnabledChanging(global::NDProperty.Propertys.OnChangingArg<Propertys.NDPConfiguration, bool> arg)
        {
        }

        [NDProperty.NDP]
        protected virtual void OnHorizontalScrollEnabledChanging(global::NDProperty.Propertys.OnChangingArg<Propertys.NDPConfiguration, bool> arg)
        {
        }

        static ScrollControl()
        {
            Template.SetDefaultControlTemplate(Template.CreateControlTemplate((ScrollControl b) =>
            {
                var grid = new Grid();

                grid.ColumnDefinitions.Add(new RelativSizeDefinition() { Size = 1 });
                grid.ColumnDefinitions.Add(new AutoSizeDefinition());

                grid.RowDefinitions.Add(new RelativSizeDefinition() { Size = 1 });
                grid.RowDefinitions.Add(new AutoSizeDefinition());

                var verticalScrollbar = new Scrollbar() { Orientation = Orientation.Vertical };
                var horizontalScrollbar = new Scrollbar() { Orientation = Orientation.Horizontal };

                var scroller = new ScrollPane() { VerticalScrollEnabled = true };

                var contentPresenter = new ContentPresenter() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                scroller.Content = contentPresenter;


                Scrollbar.MaximumProperty.Bind(verticalScrollbar, ScrollPane.VerticalScrollMaximumReadOnlyProperty.Of(scroller).OneWay());
                Scrollbar.ValueProperty.Bind(verticalScrollbar, ScrollPane.VerticalScrollPositionProperty.Of(scroller).OneWay());

                grid.Children.Add(scroller);
                grid.Children.Add(verticalScrollbar);
                grid.Children.Add(horizontalScrollbar);

                Grid.Column[verticalScrollbar].Value = 1;
                Grid.Row[horizontalScrollbar].Value = 1;

                UIElement.IsVisibleProperty.Bind(verticalScrollbar, ScrollControl.VerticalScrollEnabledProperty.Of(b).OneWay());
                UIElement.IsVisibleProperty.Bind(horizontalScrollbar, ScrollControl.HorizontalScrollEnabledProperty.Of(b).OneWay());

                ScrollPane.HorizontalScrollEnabledProperty.Bind(scroller, ScrollControl.HorizontalScrollEnabledProperty.Of(b).OneWay());
                ScrollPane.VerticalScrollEnabledProperty.Bind(scroller, ScrollControl.VerticalScrollEnabledProperty.Of(b).OneWay());

                return grid;
            }));
        }

    }
}
