﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NSCI.UI.Controls.Layput
{
    public class StackPanel : ItemsControl
    {
        private int[] elementHeights;

        protected override void ArrangeOverride(Size finalSize)
        {
            base.ArrangeOverride(finalSize);

            elementHeights = this.Items.Select(x => x.DesiredSize.Height).ToArray();
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
                Items[i].Arrange(new Size(finalSize.Width, elementHeights[i]));
        }

        public override void Render(IRenderFrame frame)
        {
            base.Render(frame);
            for (int i = 0; i < Items.Count; i++)
                Items[i].Render(frame.GetGraphicsBuffer(new Rect(0, i == 0 ? 0 : elementHeights.Take(i).Sum(), frame.Width, elementHeights[i])));
        }
    }
}
