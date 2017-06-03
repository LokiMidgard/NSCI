using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace NSCI.UI.Controls
{
    public class ItemsControl : Control
    {
        public ObservableCollection<UIElement> Items { get; } = new ObservableCollection<UIElement>();

        public ItemsControl()
        {
            this.Items.CollectionChanged += (s, e) => this.InvalidateArrange();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            availableSize = base.MeasureOverride(availableSize);
            foreach (var item in Items)
                item.Measure(availableSize);

            var desiredWidth = Items.Max(x => x.DesiredSize.Width);
            var desiredHeight = Items.Max(x => x.DesiredSize.Height);
            return new Size(desiredWidth, desiredHeight);
        }

        protected override void ArrangeOverride(Size finalSize)
        {
            base.ArrangeOverride(finalSize);
            foreach (var item in Items)
                item.Arrange(new Rect(Point.Empty, finalSize));
        }

        protected override void RenderCore(IRenderFrame frame)
        {
            foreach (var item in Items)
                item.Render(frame);
        }
    }
}
