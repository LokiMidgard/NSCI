using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using NDProperty;
using NDProperty.Propertys;
using NSCI.Propertys;

namespace NSCI.UI.Controls
{
    public abstract partial class Control : FrameworkElement
    {

        public Control()
        {

        }

        [NDProperty.NDP]
        protected virtual void OnStyleChanging(NDProperty.Propertys.OnChangingArg<NDPConfiguration, IStyle> arg)
        {
            var handlers = arg.NewValue.UpdateStyleProvider(this);
            foreach (var handler in handlers)
                arg.ExecuteAfterChange += handler;
        }

        
        [NDProperty.NDP]
        protected void OnContentTemplateChanging(OnChangingArg<NDPConfiguration, ITemplate<FrameworkElement>> arg)
        {
            if (arg.NewValue != null && !typeof(ITemplate<>).MakeGenericType(this.GetType()).IsAssignableFrom(arg.NewValue.GetType()))
            {
                arg.Reject = true;
                return;
            }

            arg.ExecuteAfterChange += (sender, e) =>
            {
                if (e.NewValue is UIElement uiElement)
                    DisplayContent = uiElement;
                else
                    DisplayContent = TemplateEngine.GetTemplate(e.NewValue).InstanciateObject();
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
