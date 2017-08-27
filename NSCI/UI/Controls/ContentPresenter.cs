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
            arg.ExecuteAfterChange += (sender, e) =>
            {
                if (e.NewValue is UIElement uiElement)
                    DisplayContent = uiElement;
                else
                    DisplayContent = Template.GetTemplate(e.NewValue, this).InstanciateObject();
            };
        }

        [NDProperty.NDP(Settigns = NDProperty.Propertys.NDPropertySettings.ReadOnly)]
        protected void OnDisplayContentChanging(OnChangingArg<NDPConfiguration, UIElement> arg) => arg.ExecuteAfterChange += (sender, e) => InvalidateMeasure();


        protected override void ArrangeOverride(Size finalSize) => DisplayContent?.Arrange(new Rect(Point.Empty, finalSize));

        protected override Size MeasureOverride(Size availableSize)
        {
            DisplayContent?.Measure(availableSize);
            return DisplayContent?.DesiredSize ?? Size.Empty;
        }

        protected override void RenderOverride(IRenderFrame frame) => DisplayContent?.Render(frame);

    }
}
