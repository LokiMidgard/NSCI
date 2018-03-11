using System;
using System.Collections.Generic;
using System.Text;
using NDProperty;
using NSCI.UI.Controls.Layout;

namespace NSCI.UI.Controls
{
    public partial class ItemsPresenter : FrameworkElement
    {

        [NDP]
        private void OnPanelChanging(NDProperty.Propertys.OnChangingArg<Propertys.NDPConfiguration, Panel> arg)
        {
            if (arg.Property.IsObjectValueChanging)
            {
                if (arg.Property.OldValue != null)
                    arg.Property.OldValue.VisualParent = null;
                if (arg.Property.NewValue != null)
                    arg.Property.NewValue.VisualParent = this;
            }
        }

        internal void Add(UIElement item) => Panel?.Children.Add(item);

        internal void Clear() => Panel?.Children.Clear();

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Panel != null)
            {
                Panel.Measure(availableSize);
                return Panel.DesiredSize;
            }
            return base.MeasureOverride(availableSize);
        }

        protected override void ArrangeOverride(Size finalSize)
        {
            if (Panel != null)
                Panel.Arrange(new Rect(Point.Empty, finalSize));
            else
                base.ArrangeOverride(finalSize);
        }

        protected override void RenderOverride(IRenderFrame frame)
        {
            if (Panel != null)
                Panel.Render(frame);
            else
                base.RenderOverride(frame);
        }
    }
}
