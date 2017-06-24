using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using NDProperty;

namespace NSCI.UI.Controls
{
    public partial class ScrollView : ContentControl
    {
        [NDP(IsReadOnly = true)]
        protected void OnMaxScrollPositionChanged(OnChangedArg<int> arg) { }

        [NDP]
        [DefaultValue(true)]
        protected void OnScroolbarVisibleChanged(OnChangedArg<bool> arg)
        {
            InvalidateMeasure();
        }

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
            if (Content != null)
            {
                availableSize = base.EnsureMinMaxWidthHeight(availableSize);
                Content.Measure(availableSize.Inflat(ScroolbarVisible ? -1 : 0, 0));
                return EnsureMinMaxWidthHeight(Content.DesiredSize.Inflat(ScroolbarVisible ? 1 : 0, 0));
            }
            else
            {
                return base.MeasureOverride(availableSize);
            }
        }

        protected override void ArrangeOverride(Size finalSize)
        {
            //ScroolbarVisible = finalSize.Height < Content.DesiredSize.Height;
            if (Content != null)
            {
                finalSize = finalSize.Inflat(ScroolbarVisible && finalSize.Width > 0 ? -1 : 0, 0);

                MaxScrollPosition = (int)(Content.DesiredSize.Height - finalSize.Height);

                if (ScrollPosition > MaxScrollPosition)
                    ScrollPosition = MaxScrollPosition;
                if (ScrollPosition < 0)
                    ScrollPosition = 0;

                Content.Arrange(new Rect(0, -ScrollPosition, finalSize.Width - 1, Content.DesiredSize.Height));
            }
        }

        protected override void RenderOverride(IRenderFrame frame)
        {
            if (Content != null)
                Content.Render(frame.GetGraphicsBuffer(new Rect(0, -ScrollPosition, ScroolbarVisible ? frame.Width - 1 : frame.Width, Content.ArrangedPosition.Height), new Rect(0, 0, ScroolbarVisible ? frame.Width - 1 : frame.Width, frame.Height)));
            else
                frame.FillRect(0, 0, ScroolbarVisible ? frame.Width - 1 : frame.Width, frame.Height, Foreground, Background, ' ');

            if (ScroolbarVisible)
            {
                var foregroundColor = HasFocus ? ConsoleColor.Red : Background;

                if (frame.Height == 1)
                {
                    if (ScrollPosition == 0)
                        frame[frame.Width - 1, 0] = new ColoredKey(Characters.Arrows.DOWNWARDS_ARROW, foregroundColor, Foreground);
                    else if (ScrollPosition == MaxScrollPosition)
                        frame[frame.Width - 1, 0] = new ColoredKey(Characters.Arrows.UPWARDS_ARROW, foregroundColor, Foreground);
                    else
                        frame[frame.Width - 1, 0] = new ColoredKey(Characters.Arrows.UP_DOWN_ARROW, foregroundColor, Foreground);
                }
                else if (frame.Height == 2)
                {
                    if (ScrollPosition == 0)
                        frame[frame.Width - 1, 0] = new ColoredKey(Characters.Block.FULL_BLOCK, Foreground, Background);
                    else
                        frame[frame.Width - 1, 0] = new ColoredKey(Characters.Arrows.BLACK_UP_POINTING_TRIANGLE, foregroundColor, Foreground);

                    if (ScrollPosition == MaxScrollPosition)
                        frame[frame.Width - 1, 1] = new ColoredKey(Characters.Block.FULL_BLOCK, Foreground, Background);
                    else
                        frame[frame.Width - 1, 1] = new ColoredKey(Characters.Arrows.BLACK_DOWN_POINTING_TRIANGLE, foregroundColor, Foreground);

                }
                else
                {
                    frame[frame.Width - 1, 0] = new ColoredKey(Characters.Arrows.BLACK_UP_POINTING_TRIANGLE, foregroundColor, Foreground);
                    frame[frame.Width - 1, frame.Height - 1] = new ColoredKey(Characters.Arrows.BLACK_DOWN_POINTING_TRIANGLE, foregroundColor, Foreground);
                    frame.FillRect(frame.Width - 1, 1, 1, frame.Height - 2, Foreground, Background, Characters.Block.FULL_BLOCK);


                    var numberOfPossibleScrollbarLocation = (frame.Height - 2) * 2 - 1;
                    int currentScrollBarPosition;
                    if (ScrollPosition == 0)
                        currentScrollBarPosition = 0;
                    else if (ScrollPosition == MaxScrollPosition)
                        currentScrollBarPosition = numberOfPossibleScrollbarLocation - 1;
                    else
                        currentScrollBarPosition = (int)((ScrollPosition - 1) / (double)(MaxScrollPosition - 2) * (numberOfPossibleScrollbarLocation - 2)) + 1;

                    if (currentScrollBarPosition % 2 == 0) // Print FullBlock
                    {
                        frame[frame.Width - 1, currentScrollBarPosition / 2 + 1] = new ColoredKey(Characters.Block.FULL_BLOCK, foregroundColor, Foreground);
                    }
                    else // Print two Half Blocks
                    {
                        frame[frame.Width - 1, currentScrollBarPosition / 2 + 1] = new ColoredKey(Characters.Block.LOWER_HALF_BLOCK, foregroundColor, Foreground);
                        frame[frame.Width - 1, currentScrollBarPosition / 2 + 2] = new ColoredKey(Characters.Block.UPPER_HALF_BLOCK, foregroundColor, Foreground);
                    }
                }
            }


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
