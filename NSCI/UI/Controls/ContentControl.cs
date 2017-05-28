using System;
using System.Collections.Generic;
using System.Text;

namespace NSCI.UI.Controls
{
    public class ContentControl : Control
    {
        public UIElement Content { get; set; }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Content == null)
                return base.MeasureOverride(availableSize);
            Content.Measure(base.MeasureOverride(availableSize));
            var desiredSize = Content.DesiredSize;
            return base.MeasureOverride(desiredSize);
        }

        protected override void ArrangeOverride(Size finalSize)
        {
            base.ArrangeOverride(finalSize);
            Content?.Arrange(finalSize);
        }

        public override void Render(IRenderFrame frame)
        {
            Content?.Render(frame);
        }
    }
}
