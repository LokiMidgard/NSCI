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
        private Func<UIElement> templateInstanciator;

        public Control()
        {

        }

        protected UIElement InstanciateDefaultTemplate()
        {
            var getDefaultControlTemplateMethod = typeof(Template).GetMethod(nameof(Template.GetDefaultControlTemplate));
            getDefaultControlTemplateMethod = getDefaultControlTemplateMethod.MakeGenericMethod(new Type[] { GetType() });
            var emptyArgumentList = new object[0];
            var controlTemplate = getDefaultControlTemplateMethod.Invoke(null, emptyArgumentList);
            var instance = (UIElement)typeof(IControlTemplate<,>)
                .MakeGenericType(typeof(object), GetType())
                .GetMethod(nameof(IControlTemplate<object, Control>.InstanciateObject), BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public)
                .Invoke(controlTemplate, new object[] { this });
            return instance;
        }

        public void SetControlTemplate<TThis>(IControlTemplate<UIElement, TThis> template)
                where TThis : Control
        {
            if (this is TThis me)
            {
                if (template == null)
                    this.templateInstanciator = null;
                else
                    this.templateInstanciator = () => template.InstanciateObject(me);
                DisplayContent = GetControlTemplatedInstance();
            }
            else
                throw new ArgumentException($"{nameof(TThis)} must be of type {this.GetType().FullName} or assinable to it.", nameof(TThis));
        }

        private UIElement GetControlTemplatedInstance()
        {
            if (this.templateInstanciator != null)
                return this.templateInstanciator();

            return InstanciateDefaultTemplate();
        }


        [NDProperty.NDP(Settings = NDProperty.Propertys.NDPropertySettings.ReadOnly)]
        protected void OnDisplayContentChanging(OnChangingArg<NDPConfiguration, UIElement> arg)
        {
            if (arg.Property.IsObjectValueChanging)
                arg.ExecuteAfterChange += (sender, e) =>
                {
                    if (e.Property.OldValue != null && e.Property.OldValue.VisualParent == this)
                        e.Property.OldValue.VisualParent = null;
                    if (e.Property.NewValue != null)
                        e.Property.NewValue.VisualParent = this;
                    InvalidateMeasure();
                };
        }

        protected sealed override void ArrangeOverride(Size finalSize)
        {
            if (DisplayContent == null)
                DisplayContent = GetControlTemplatedInstance();

            DisplayContent?.Arrange(new Rect(Point.Empty, finalSize));
        }

        protected sealed override Size MeasureOverride(Size availableSize)
        {
            if (DisplayContent == null)
                DisplayContent = GetControlTemplatedInstance();

            DisplayContent?.Measure(availableSize);
            return DisplayContent?.DesiredSize ?? Size.Empty;
        }

        protected sealed override void RenderOverride(IRenderFrame frame)
        {
            if (DisplayContent == null)
                DisplayContent = GetControlTemplatedInstance();

            DisplayContent.Render(frame);
        }



    }

}
