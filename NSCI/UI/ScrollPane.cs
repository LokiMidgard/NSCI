using System;
using System.Collections.Generic;
using System.Text;

namespace NSCI.UI
{
    public partial class ScrollPane : FrameworkElement
    {
        [NDProperty.NDP]
        protected virtual void OnContentChanging(global::NDProperty.Propertys.OnChangingArg<Propertys.NDPConfiguration, UIElement> arg)
        {
            if (arg.Property.IsObjectValueChanging)
            {
                var oldValue = arg.Property.OldValue;
                var newValue = arg.Property.NewValue;

                if (oldValue != null)
                    oldValue.VisualParent = null;
                if (newValue != null)
                    newValue.VisualParent = this;
            }
        }

        [NDProperty.NDP]
        protected virtual void OnHorizontalScrollPositionChanging(global::NDProperty.Propertys.OnChangingArg<Propertys.NDPConfiguration, int> arg)
        {
            this.InvalidateRender();
        }

        [NDProperty.NDP]
        protected virtual void OnVerticalScrollPositionChanging(global::NDProperty.Propertys.OnChangingArg<Propertys.NDPConfiguration, int> arg)
        {
            this.InvalidateRender();

        }

        [NDProperty.NDP]
        protected virtual void OnVerticalScrollEnabledChanging(global::NDProperty.Propertys.OnChangingArg<Propertys.NDPConfiguration, bool> arg)
        {
            this.InvalidateMeasure();
        }

        [NDProperty.NDP]
        protected virtual void OnHorizontalScrollEnabledChanging(global::NDProperty.Propertys.OnChangingArg<Propertys.NDPConfiguration, bool> arg)
        {
            this.InvalidateMeasure();
        }

        public override bool SupportSelection => true;

        protected override Size MeasureOverride(Size availableSize)
        {

            if (Content != null)
            {
                Content.Measure(new Size(HorizontalScrollEnabled ? IntEx.PositiveInfinity : availableSize.Width, VerticalScrollEnabled ? IntEx.PositiveInfinity : availableSize.Height));
                availableSize = new Size(HorizontalScrollEnabled ? availableSize.Width : Content.DesiredSize.Width, VerticalScrollEnabled ? availableSize.Height : Content.DesiredSize.Height);
            }

            return base.MeasureOverride(availableSize);
        }

        protected override void ArrangeOverride(Size finalSize)
        {
            if (Content != null)
            {
                Content.Arrange(new Rect(Point.Empty, new Size(HorizontalScrollEnabled ? Content.DesiredSize.Width : finalSize.Width, VerticalScrollEnabled ? Content.DesiredSize.Height : finalSize.Height)));
            }
        }

        protected override void RenderOverride(IRenderFrame frame)
        {
            if (Content != null)
            {

                var maxHorizontalScrollposition = MathEx.Max(0, Content.ArrangedPosition.Width - frame.Width);
                var maxVerticalScrollposition = MathEx.Max(0, Content.ArrangedPosition.Height - frame.Height);

                var horizontalScrollPosition = MathEx.Clamp(0, maxHorizontalScrollposition, HorizontalScrollPosition);
                var verticalScrollPosition = MathEx.Clamp(0, maxVerticalScrollposition, VerticalScrollPosition);

                Content.Render(frame.GetGraphicsBuffer(new Rect(-horizontalScrollPosition, -verticalScrollPosition, Content.ArrangedPosition.Width, Content.ArrangedPosition.Height), new Rect(0, 0, frame.Width, frame.Height)));
            }
        }

        public override bool PreviewHandleInput(FrameworkElement originalTarget, ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key == ConsoleKey.UpArrow)
                VerticalScrollPosition++;
            else if (keyInfo.Key == ConsoleKey.DownArrow)
                VerticalScrollPosition--;
            else if (keyInfo.Key == ConsoleKey.RightArrow)
                HorizontalScrollPosition--;
            else if (keyInfo.Key == ConsoleKey.LeftArrow)
                HorizontalScrollPosition++;
            else
                return base.PreviewHandleInput(originalTarget, keyInfo);
            return true;
        }

    }
}
