using System;
using System.Collections.Generic;
using System.Text;
using NSCI.Propertys;

namespace NSCI.UI.Controls
{
    public partial class ContentControl : Control
    {

        [NDProperty.NDP]
        protected virtual void OnContentChanging(NDProperty.Propertys.OnChangingArg<NDPConfiguration, UIElement> arg)
        {
            arg.ExecuteAfterChange += (sender, args) =>
            {
                if (args.NewValue != args.OldValue)
                {
                    if (arg.NewValue.Parent != null)
                        throw new ArgumentException($"The Element is already Chiled of {arg.NewValue.Parent}");
                    if (args.OldValue != null)
                        args.OldValue.Parent = null;
                    if (args.NewValue != null)
                        args.NewValue.Parent = this;

                    InvalidateMeasure();
                }
            };
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
                frame.FillRect((int)clip.Left, (int)clip.Right, (int)clip.Width, (int) clip.Height, Foreground, Background, (char)SpecialChars.Fill);
            }
            else
            {
                Content.Render(frame);
            }
        }
    }
}
