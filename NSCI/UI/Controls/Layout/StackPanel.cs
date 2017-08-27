using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NSCI.UI.Controls.Layout
{
    public class StackPanel : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            return base.MeasureOverride(availableSize);
        }

        protected override void ArrangeOverride(Size finalSize)
        {
            //base.ArrangeOverride(finalSize);
            System.Diagnostics.Debug.Assert(finalSize.Width >= 0);
            System.Diagnostics.Debug.Assert(finalSize.Height >= 0);

            var elementHeights = Children.Select(x => x.DesiredSize.Height).ToArray();
            var sumHight = elementHeights.Sum();
            if (sumHight > finalSize.Height)
            {
                int i = Children.Count - 1;
                for (; sumHight > finalSize.Height; i--)
                {
                    if (i < 0)
                        i = Children.Count - 1;
                    if (elementHeights[i] <= 0)
                        continue;
                    sumHight--;
                    elementHeights[i]--;
                }
            }
            if (sumHight < finalSize.Height)
                elementHeights[elementHeights.Length - 1] += finalSize.Height - sumHight;

            for (int i = 0; i < Children.Count; i++)
            {

                int x;
                int width;
                switch (Children[i].HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        x = 0;
                        width = (int)MathEx.Min(finalSize.Width, Children[i].DesiredSize.Width);
                        break;
                    case HorizontalAlignment.Right:
                        width = (int)MathEx.Min(finalSize.Width, Children[i].DesiredSize.Width);
                        x = (int)finalSize.Width - width;
                        break;
                    case HorizontalAlignment.Center:
                        width = (int)MathEx.Min(finalSize.Width, Children[i].DesiredSize.Width);
                        if ((finalSize.Width - width) % 2 == 1)
                            width++;
                        x = (int)(finalSize.Width - width) / 2;
                        break;
                    default:
                    case HorizontalAlignment.Strech:
                        x = 0;
                        width = (int)finalSize.Width;
                        break;
                }
                Children[i].Arrange(new Rect(x, i == 0 ? 0 : elementHeights.Take(i).Sum(), width, elementHeights[i]));
            }
        }

        protected override void RenderOverride(IRenderFrame frame)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                var location = GetLocation(Children[i]);
                Children[i].Render(frame.GetGraphicsBuffer(location));
                if (location.Left > 0)
                    frame.FillRect(0, (int)location.Top, (int)location.Left, (int)location.Height, Foreground, Background, (char)SpecialChars.Fill);
                if (frame.Width - location.Right > 0)
                    frame.FillRect((int)location.Right, (int)location.Top, (int)(frame.Width - location.Right), (int)location.Height, Foreground, Background, (char)SpecialChars.Fill);
            }
        }

    }
}
