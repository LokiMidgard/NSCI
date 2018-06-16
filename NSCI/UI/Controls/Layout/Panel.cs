using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace NSCI.UI.Controls.Layout
{
    public class Panel : FrameworkElement
    {

        public ObservableCollection<UIElement> Children { get; } = new ObservableCollection<UIElement>();


        public Panel()
        {
            Children.CollectionChanged += (s, e) =>
            {
                InvalidateMeasure();
                if (e.NewItems != null)
                    foreach (UIElement uielement in e.NewItems)
                    {
                        uielement.LogicalParent = this;
                        uielement.VisualParent = this;
                    }
                if (e.OldItems != null)
                    foreach (UIElement uielement in e.OldItems)
                    {
                        uielement.LogicalParent = null;
                        uielement.VisualParent = null;
                    }
            };
        }

    }
}
