using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace NSCI.UI.Controls.Layout
{
    public class Panel :FrameworkElement
    {

        public ObservableCollection<UIElement> Children { get; } = new ObservableCollection<UIElement>();


        public Panel()
        {
            this.Children.CollectionChanged += (s, e) =>
            {
                this.InvalidateArrange();
                if (e.NewItems != null)
                    foreach (UIElement uielement in e.NewItems)
                        uielement.Parent = this;
                if (e.OldItems != null)
                    foreach (UIElement uielement in e.OldItems)
                        uielement.Parent = null;
            };
        }

    }
}
