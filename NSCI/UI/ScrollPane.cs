using System;
using System.Collections.Generic;
using System.Linq;
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
                    oldValue.LogicalParent = null;
                if (oldValue != null)
                    oldValue.VisualParent = null;

                if (newValue != null)
                    newValue.LogicalParent = this;
                if (newValue != null)
                    newValue.VisualParent = this;
            }
        }

        [NDProperty.NDP]
        protected virtual void OnHorizontalScrollPositionChanging(global::NDProperty.Propertys.OnChangingArg<Propertys.NDPConfiguration, int> arg)
        {
            arg.Provider.MutatedValue = MathEx.Clamp(0, HorizontalScrollMaximum, arg.Provider.NewValue);
            if (arg.Property.IsObjectValueChanging)
                InvalidateArrange();
        }

        [NDProperty.NDP]
        protected virtual void OnVerticalScrollPositionChanging(global::NDProperty.Propertys.OnChangingArg<Propertys.NDPConfiguration, int> arg)
        {
            arg.Provider.MutatedValue = MathEx.Clamp(0, VerticalScrollMaximum, arg.Provider.NewValue);
            if (arg.Property.IsObjectValueChanging)
                InvalidateArrange();

        }

        [NDProperty.NDP(Settings = NDProperty.Propertys.NDPropertySettings.ReadOnly)]
        protected virtual void OnVerticalScrollMaximumChanging(global::NDProperty.Propertys.OnChangingArg<Propertys.NDPConfiguration, int> arg)
        {
            if (arg.Property.IsObjectValueChanging)
            {
                if (VerticalScrollPosition > arg.Property.NewValue)
                    arg.ExecuteAfterChange += (sender, e) => VerticalScrollPosition = arg.Property.NewValue;
            }
        }

        [NDProperty.NDP(Settings = NDProperty.Propertys.NDPropertySettings.ReadOnly)]
        protected virtual void OnHorizontalScrollMaximumChanging(global::NDProperty.Propertys.OnChangingArg<Propertys.NDPConfiguration, int> arg)
        {
            if (arg.Property.IsObjectValueChanging)
            {
                if (HorizontalScrollPosition > arg.Property.NewValue)
                    arg.ExecuteAfterChange += (sender, e) => HorizontalScrollPosition = arg.Property.NewValue;
            }
        }


        [NDProperty.NDP]
        protected virtual void OnVerticalScrollEnabledChanging(global::NDProperty.Propertys.OnChangingArg<Propertys.NDPConfiguration, bool> arg)
        {
            if (arg.Property.IsObjectValueChanging)
                InvalidateMeasure();
        }

        [NDProperty.NDP]
        protected virtual void OnHorizontalScrollEnabledChanging(global::NDProperty.Propertys.OnChangingArg<Propertys.NDPConfiguration, bool> arg)
        {
            if (arg.Property.IsObjectValueChanging)
                InvalidateMeasure();
        }

        public void BrinIntoView(UIElement element)
        {
            if (!element.GetPathToRoot().Contains(this))
                throw new ArgumentException("Is not an descendant of Scrollpane.", nameof(element));
            var location = GetLocation(element);

            if (location.Top < 0)
                VerticalScrollPosition += (int)location.Top;
            if (location.Bottom > ActualHeight)
                VerticalScrollPosition += (int)location.Bottom - ActualHeight;


            if (location.Left < 0)
                HorizontalScrollPosition += (int)location.Left;
            if (location.Bottom > ActualWidth)
                HorizontalScrollPosition += (int)location.Right - ActualWidth;
        }

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
                Content.Arrange(new Rect(-HorizontalScrollPosition, -VerticalScrollPosition, HorizontalScrollEnabled ? Content.DesiredSize.Width : finalSize.Width, VerticalScrollEnabled ? Content.DesiredSize.Height : finalSize.Height));
                HorizontalScrollMaximum = (int)(Content.ArrangedPosition.Width - finalSize.Width);
                VerticalScrollMaximum = (int)(Content.ArrangedPosition.Height - finalSize.Height);
            }
            else
            {
                HorizontalScrollMaximum = 0;
                VerticalScrollMaximum = 0;
            }
        }

        protected override void RenderOverride(IRenderFrame frame)
        {
            if (Content != null)
            {
                var horizontalScrollPosition = MathEx.Clamp(0, HorizontalScrollMaximum, HorizontalScrollPosition);
                var verticalScrollPosition = MathEx.Clamp(0, VerticalScrollMaximum, VerticalScrollPosition);
                Content.Render(frame.GetGraphicsBuffer(new Rect(-horizontalScrollPosition, -verticalScrollPosition, Content.ArrangedPosition.Width, Content.ArrangedPosition.Height), new Rect(0, 0, frame.Width, frame.Height)));
            }
        }

        public override bool PreviewHandleInput(FrameworkElement originalTarget, ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Modifiers != ConsoleModifiers.Control)
                return false;
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
