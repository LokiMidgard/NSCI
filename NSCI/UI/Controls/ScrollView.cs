using System;
using System.Collections.Generic;
using System.Text;
using NDProperty;

namespace NSCI.UI.Controls
{
    public partial class ScrollView : ContentControl
    {

        public override bool SupportSelection => true;

        [NDP]
        private void OnScrollPositionChanged(OnChangedArg<int> arg)
        {

        }

        protected override void OnHasFocusChanged(OnChangedArg<bool> arg)
        {
            base.OnHasFocusChanged(arg);
            InvalidateRender();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            availableSize = base.EnsureMinMaxWidthHeight(availableSize);
            var scroolbarVisible = true;
            Content.Measure(availableSize.Inflat(scroolbarVisible ? -1 : 0, 0));
            return EnsureMinMaxWidthHeight(Content.DesiredSize.Inflat(scroolbarVisible ? 1 : 0, 0));
        }

        protected override void ArrangeOverride(Size finalSize)
        {
            var scroolbarVisible = true;
            finalSize = finalSize.Inflat(scroolbarVisible && finalSize.Width > 0 ? -1 : 0, 0);

            var maxScrollPosition = (int)(Content.DesiredSize.Height - finalSize.Height);

            if (maxScrollPosition < ScrollPosition)
                ScrollPosition = maxScrollPosition;
            if (ScrollPosition < 0)
                ScrollPosition = 0;

            Content.Arrange(new Rect(0, -ScrollPosition, finalSize.Width - 1, Content.DesiredSize.Height));

        }

        protected override void RenderOverride(IRenderFrame frame)
        {
            var scroolbarVisible = true;
            //new Rect(new Point(0, -ScrollPosition), finalSize)
            Content.Render(frame.GetGraphicsBuffer(new Rect(0, -ScrollPosition, scroolbarVisible ? frame.Width - 1 : frame.Width, Content.ArrangedPosition.Height), new Rect(0, 0, scroolbarVisible ? frame.Width - 1 : frame.Width, frame.Height)));
            var foregroundColor = HasFocus ? ConsoleColor.Red : Background;
            frame[frame.Width - 1, 0] = new ColoredKey(Characters.Arrows.BLACK_UP_POINTING_TRIANGLE, foregroundColor, Foreground);
            frame[frame.Width - 1, frame.Height - 1] = new ColoredKey(Characters.Arrows.BLACK_DOWN_POINTING_TRIANGLE, foregroundColor, Foreground);
            frame.FillRect(frame.Width - 1, 1, 1, frame.Height - 2, Foreground, Background, Characters.Shade.LIGHT);


        }

        public override bool PreviewHandleInput(Control originalTarget, ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key == ConsoleKey.UpArrow && keyInfo.Modifiers == ConsoleModifiers.Control || keyInfo.Key == ConsoleKey.DownArrow && keyInfo.Modifiers == ConsoleModifiers.Control)
            {
                if (keyInfo.Key == ConsoleKey.UpArrow)
                    ScrollPosition--;
                else
                    ScrollPosition++;
                InvalidateArrange();
                return true;
            }
            return base.PreviewHandleInput(originalTarget, keyInfo);
        }

    }
}
