using System;
using System.Collections.Generic;
using System.Text;

namespace NSCI.UI
{
    public partial class Scrollbar : FrameworkElement
    {
        private bool isBeginningFullBlock;
        private bool isEndingFullBlock;
        private int startPosition;
        private int endPosition;

        [NDProperty.NDP]
        protected virtual void OnOrientationChanging(NDProperty.Propertys.OnChangingArg<Propertys.NDPConfiguration, Orientation> arg)
        {
            if (arg.Property.IsObjectValueChanging)
                arg.ExecuteAfterChange += (sender, e) => InvalidateMeasure();
        }

        [NDProperty.NDP]
        protected virtual void OnMaximumChanging(NDProperty.Propertys.OnChangingArg<Propertys.NDPConfiguration, int> arg)
        {
            if (arg.Property.IsObjectValueChanging)
                arg.ExecuteAfterChange += (sender, e) => UpdateBlockPosition();
        }

        [NDProperty.NDP]
        protected virtual void OnValueChanging(NDProperty.Propertys.OnChangingArg<Propertys.NDPConfiguration, int> arg)
        {
            if (arg.Property.IsObjectValueChanging)
                arg.ExecuteAfterChange += (sender, e) => UpdateBlockPosition();
        }

        protected override void Arranged()
        {
            UpdateBlockPosition();
        }
        private void UpdateBlockPosition()
        {
            int numberOfCharacters;
            switch (Orientation)
            {
                case Orientation.Vertical:
                    numberOfCharacters = (int)ArrangedPosition.Height - 2;
                    break;
                case Orientation.Horizontal:
                    numberOfCharacters = (int)ArrangedPosition.Width - 2;
                    break;
                default:
                    throw new NotSupportedException($"The value {Orientation} is not expected in {typeof(Orientation).FullName}");
            }

            if (numberOfCharacters < 1)
            {
                this.startPosition = -1;
                this.endPosition = -1;
                this.isBeginningFullBlock = false;
                this.isEndingFullBlock = false;
            }
            else
            {
                int tempStartPosition;
                int tempEndPosition;
                var maximumNumberOfSteps = numberOfCharacters * 2;
                if (Maximum > maximumNumberOfSteps)
                {
                    if (Value == Maximum) // prevent possible rounding failures
                    {
                        tempStartPosition = maximumNumberOfSteps - 1;
                        tempEndPosition = maximumNumberOfSteps - 1;
                    }
                    else
                    {
                        var valueRatio = maximumNumberOfSteps / (double)Maximum;
                        var position = (int)Math.Round(valueRatio * Value);
                        tempStartPosition = position;
                        tempEndPosition = position;
                    }

                }
                else
                {
                    var barWidth = maximumNumberOfSteps - Maximum;
                    tempStartPosition = Value;
                    tempEndPosition = Value + barWidth;
                }
                this.isBeginningFullBlock = tempStartPosition % 2 == 0 && tempEndPosition > tempStartPosition;
                this.isEndingFullBlock = tempEndPosition % 2 == 1 && tempEndPosition > tempStartPosition;

                this.startPosition = 1 + tempStartPosition / 2;
                this.endPosition = 1 + tempEndPosition / 2;

                if (tempStartPosition == tempEndPosition && !this.isEndingFullBlock)
                    this.startPosition = -1; // start position has higher priorety so we need to reset it
            }
            InvalidateRender();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            availableSize = base.MeasureOverride(availableSize);

            switch (Orientation)
            {
                case Orientation.Vertical:
                    availableSize = new Size(1, availableSize.Height);
                    break;
                case Orientation.Horizontal:
                    availableSize = new Size(availableSize.Width, 1);
                    break;
                default:
                    throw new NotSupportedException($"The value {Orientation} is not expected in {typeof(Orientation).FullName}");
            }
            return base.MeasureOverride(availableSize);
        }

        protected override void RenderOverride(IRenderFrame frame)
        {

            switch (Orientation)
            {
                case Orientation.Vertical:

                    RenderVertical(frame);

                    break;
                case Orientation.Horizontal:

                    break;
                default:
                    throw new NotSupportedException($"The value {Orientation} is not expected in {typeof(Orientation).FullName}");
            }
        }

        private void RenderVertical(IRenderFrame frame)
        {
            int left, right;

            switch (HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    left = 0;
                    right = 0;
                    break;
                case HorizontalAlignment.Right:
                    left = frame.Width - 1;
                    right = frame.Width - 1;
                    break;
                case HorizontalAlignment.Center:
                    right = frame.Width / 2;
                    left = frame.Width / 2;
                    break;
                case HorizontalAlignment.Strech:
                    left = 0;
                    right = frame.Width - 1;
                    break;
                default:
                    throw new NotSupportedException($"The value {HorizontalAlignment} is not expected in {typeof(HorizontalAlignment).FullName}");
            }
            for (var x = (int)MathEx.Max(left, frame.Clip.Left); x <= MathEx.Min(right, frame.Clip.Right); x++)
            {
                if (frame.Height == 1)
                    frame[x, 0] = new ColoredKey(Characters.Arrows.UP_DOWN_ARROW, Foreground, Background);
                else
                {
                    for (var y = (int)frame.Clip.Top; y < frame.Clip.Bottom; y++)
                    {
                        if (y == 0)
                            frame[x, 0] = new ColoredKey(Characters.Arrows.UPWARDS_ARROW, Foreground, Background);
                        else if (y == frame.Height - 1)
                            frame[x, frame.Height - 1] = new ColoredKey(Characters.Arrows.DOWNWARDS_ARROW, Foreground, Background);
                        else if (y == this.startPosition)
                        {
                            if (this.isBeginningFullBlock)
                                frame[x, y] = new ColoredKey(Characters.Block.FULL_BLOCK, Foreground, Background);
                            else
                                frame[x, y] = new ColoredKey(Characters.Block.LOWER_HALF_BLOCK, Foreground, Background);
                        }
                        else if (y == this.endPosition)
                        {
                            if (this.isEndingFullBlock)
                                frame[x, y] = new ColoredKey(Characters.Block.FULL_BLOCK, Foreground, Background);
                            else
                                frame[x, y] = new ColoredKey(Characters.Block.UPPER_HALF_BLOCK, Foreground, Background);
                        }
                        else if (y > this.startPosition && y < this.endPosition && this.startPosition != -1 && this.endPosition != -1)
                        {
                            frame[x, y] = new ColoredKey(Characters.Block.FULL_BLOCK, Foreground, Background);
                        }
                        else
                        {
                            frame[x, y] = new ColoredKey(' ', Foreground, Background);
                        }
                    }

                }
            }
        }

        private void RenderHorizontal(IRenderFrame frame)
        {
            int top, buttom;

            switch (VerticalAlignment)
            {
                case VerticalAlignment.Top:
                    top = 0;
                    buttom = 0;
                    break;
                case VerticalAlignment.Bottom:
                    top = frame.Height - 1;
                    buttom = frame.Height - 1;
                    break;
                case VerticalAlignment.Center:
                    buttom = frame.Height / 2;
                    top = frame.Height / 2;
                    break;
                case VerticalAlignment.Strech:
                    top = 0;
                    buttom = frame.Height - 1;
                    break;
                default:
                    throw new NotSupportedException($"The value {HorizontalAlignment} is not expected in {typeof(HorizontalAlignment).FullName}");
            }
            for (var y = (int)MathEx.Max(top, frame.Clip.Top); y <= MathEx.Min(buttom, frame.Clip.Bottom); y++)
            {
                if (frame.Width == 1)
                    frame[0, y] = new ColoredKey(Characters.Arrows.LEFT_RIGHT_ARROW, Foreground, Background);
                else
                {
                    for (var x = (int)frame.Clip.Left; x < frame.Clip.Right; x++)
                    {
                        if (x == 0)
                            frame[0, y] = new ColoredKey(Characters.Arrows.LEFTWARDS_ARROW, Foreground, Background);
                        else if (x == frame.Width - 1)
                            frame[frame.Width - 1, y] = new ColoredKey(Characters.Arrows.RIGHTWARDS_ARROW, Foreground, Background);
                        else if (x == this.startPosition)
                        {
                            if (this.isBeginningFullBlock)
                                frame[x, y] = new ColoredKey(Characters.Block.FULL_BLOCK, Foreground, Background);
                            else
                                frame[x, y] = new ColoredKey(Characters.Block.LEFT_HALF_BLOCK, Foreground, Background);
                        }
                        else if (x == this.endPosition)
                        {
                            if (this.isEndingFullBlock)
                                frame[x, y] = new ColoredKey(Characters.Block.FULL_BLOCK, Foreground, Background);
                            else
                                frame[x, y] = new ColoredKey(Characters.Block.RIGHT_HALF_BLOCK, Foreground, Background);
                        }
                        else if (x > this.startPosition && x < this.endPosition && this.startPosition != -1 && this.endPosition != -1)
                        {
                            frame[x, y] = new ColoredKey(Characters.Block.FULL_BLOCK, Foreground, Background);
                        }
                        else
                        {
                            frame[x, y] = new ColoredKey(' ', Foreground, Background);
                        }
                    }

                }
            }
        }
    }
}
