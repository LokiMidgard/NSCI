using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NSCI.UI.Controls.Layput
{
    public class StackPanel : ItemsControl
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            return base.MeasureOverride(availableSize);
        }

        protected override void ArrangeOverride(Size finalSize)
        {
            //base.ArrangeOverride(finalSize);

            var elementHeights = this.Items.Select(x => x.DesiredSize.Height).ToArray();
            var sumHight = elementHeights.Sum();
            if (sumHight > finalSize.Height)
            {
                int i = Items.Count - 1;
                for (; sumHight > finalSize.Height; i--)
                {
                    if (i < 0)
                        i = Items.Count - 1;
                    sumHight--;
                    elementHeights[i]--;
                }
            }
            if (sumHight < finalSize.Height)
                elementHeights[elementHeights.Length - 1] += finalSize.Height - sumHight;

            for (int i = 0; i < Items.Count; i++)
            {

                int x;
                int width;
                switch (Items[i].HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        x = 0;
                        width = Math.Min(finalSize.Width, Items[i].DesiredSize.Width);
                        break;
                    case HorizontalAlignment.Right:
                        width = Math.Min(finalSize.Width, Items[i].DesiredSize.Width);
                        x = finalSize.Width - width;
                        break;
                    case HorizontalAlignment.Center:
                        width = Math.Min(finalSize.Width, Items[i].DesiredSize.Width);
                        if ((finalSize.Width - width) % 2 == 1)
                            width++;
                        x = (finalSize.Width - width) / 2;
                        break;
                    default:
                    case HorizontalAlignment.Strech:
                        x = 0;
                        width = finalSize.Width;
                        break;
                }
                Items[i].Arrange(new Rect(x, i == 0 ? 0 : elementHeights.Take(i).Sum(), width, elementHeights[i]));
            }
        }

        protected override void RenderOverride(IRenderFrame frame)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                var location = GetLocation(Items[i]);
                Items[i].Render(frame.GetGraphicsBuffer(location));
                if (location.Left > 0)
                    frame.FillRect(0, location.Top, location.Left, location.Height, ActualForeground, ActuellBackground, SpecialChars.Fill);
                if (frame.Width - location.Right > 0)
                    frame.FillRect(location.Right, location.Top, frame.Width - location.Right, location.Height, ActualForeground, ActuellBackground, SpecialChars.Fill);
            }
        }

    }
}
