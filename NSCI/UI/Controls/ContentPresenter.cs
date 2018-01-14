using System;
using System.Collections.Generic;
using System.Text;
using NDProperty.Propertys;
using NSCI.Propertys;

namespace NSCI.UI.Controls
{
    public partial class ContentPresenter : FrameworkElement
    {

        [NDProperty.NDP]
        protected void OnContentChanging(OnChangingArg<NDPConfiguration, object> arg)
        {
            if (arg.Property.IsObjectValueChanging)
                arg.ExecuteAfterChange += (sender, e) =>
                {
                    DisplayContent = Template.InstanciateFromDefaultDataTemplate(e.Property.NewValue);
                };
        }

        [NDProperty.NDP(Settings = NDProperty.Propertys.NDPropertySettings.ReadOnly)]
        protected void OnDisplayContentChanging(OnChangingArg<NDPConfiguration, UIElement> arg)
        {
            if (arg.Property.IsObjectValueChanging)
                arg.ExecuteAfterChange += (sender, e) =>
                {
                    if (e.Property.OldValue != null)
                        e.Property.OldValue.VisualParent = null;
                    if (e.Property.NewValue != null)
                        e.Property.NewValue.VisualParent = this;
                    InvalidateMeasure();
                };
        }


        protected override void ArrangeOverride(Size finalSize) => DisplayContent?.Arrange(new Rect(Point.Empty, finalSize));

        protected override Size MeasureOverride(Size availableSize)
        {
            DisplayContent?.Measure(availableSize);
            return DisplayContent?.DesiredSize ?? Size.Empty;
        }

        protected override void RenderOverride(IRenderFrame frame) => DisplayContent?.Render(frame);

    }
}
