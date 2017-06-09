using System;
using System.Collections.Generic;
using System.Text;

namespace NSCI.UI.Controls
{
    public class ContentControl : Control
    {
        private UIElement content;
        public UIElement Content
        {
            get => content; set
            {
                if (content != value)
                {
                    if (value.Parent != null)
                        throw new ArgumentException($"The Element is already Chiled of {value.Parent}");
                    if (content != null)
                        content.Parent = null;
                    content = value;
                    if (content != null)
                        content.Parent = this;
                    InvalidateMeasure();
                }
            }
        }

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
            Content?.Arrange(new Rect(Point.Empty, finalSize));
        }

        protected override void RenderOverride(IRenderFrame frame)
        {
            if (Content == null)
            {
                var clip = frame.Clip ?? new Rect(0, 0, frame.Width, frame.Height);
                frame.FillRect(clip.Left, clip.Right, clip.Width, clip.Height, this.ActualForeground, this.ActuellBackground, SpecialChars.Fill);
            }
            else
            {
                Content.Render(frame);
            }
        }
    }
}
