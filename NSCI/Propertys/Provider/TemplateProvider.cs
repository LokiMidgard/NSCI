using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using NDProperty.Providers;
using NSCI.UI;
using NSCI.UI.Controls;

namespace NSCI.Propertys.Provider
{
    class TemplateProvider : ValueProvider<NDPConfiguration>
    {
        public static ValueProvider<NDPConfiguration> Instance { get; } = new TemplateProvider();
        private TemplateProvider()
        {

        }


        public override (TValue value, bool hasValue) GetValue<TValue, TType>(TType targetObject, NDProperty.Propertys.NDReadOnlyPropertyKey<NDPConfiguration, TValue, TType> property)
        {
            if (!property.Equals(UI.Controls.Control.ContentTemplateProperty))
                return (default, false);

            System.Diagnostics.Debug.Assert(targetObject is UI.Controls.Control);
            var control = targetObject as UI.Controls.Control;


            var currentObject = control as UIElement;

            return GetTemplate<TValue, TType>(targetObject, currentObject);

        }


        public static ITemplate<TType> GetTemplate<TType>(object toTemplate, UIElement objectToSetTemplateOn) where TType : class => GetTemplate<ITemplate<TType>, object>(toTemplate, objectToSetTemplateOn).value;


        private static (TValue value, bool hasValue) GetTemplate<TValue, TType>(TType targetObject, UIElement currentObject) where TType : class
        {
            while (currentObject != null)
            {
                var context = X.Values[currentObject].Value;
                if (context != null)
                {
                    var foundTemplate = FindTemplateInContext<TValue>(context, targetObject);
                    if (foundTemplate != null)
                        return (foundTemplate, true);
                }
                currentObject = currentObject.Parent;
            }

            var defaultTemplate = FindTemplateInContext<TValue>(DefalutTemplates.Instance, targetObject);
            if (defaultTemplate == null)
                defaultTemplate = (TValue)DefaultTemplate(targetObject);


            return (defaultTemplate, true);
        }

        internal static void SetDefaultTemplate<T>(Template<T> template) where T : UIElement, new()
        {
            DefalutTemplates.typeLookup[typeof(T)] = template;
        }

        private class DefalutTemplates : IResources
        {
            public static DefalutTemplates Instance { get; } = new DefalutTemplates();
            public static readonly Dictionary<Type, ITemplate<UIElement>> typeLookup = new Dictionary<Type, ITemplate<UIElement>>();

            public object this[string key] => throw new NotSupportedException();

            public object this[Type type]
            {
                get
                {
                    if (typeLookup.ContainsKey(type))
                        return typeLookup[type];
                    return null;
                }
            }

            public IReadOnlyCollection<object> Children => throw new NotSupportedException();
        }

        private static TValue FindTemplateInContext<TValue>(IResources context, object obj)
        {
            var currentType = obj.GetType();

            while (currentType != null)
            {
                if (context[currentType] is TValue foundTemplate)
                    return foundTemplate;
                currentType = currentType.GetTypeInfo().BaseType;
            }
            return default;
        }

        private static ITemplate<UIElement> DefaultTemplate(object obj)
        {
            return new Template<TextBlock>
            {
                Setter = new ITemplateSetter<TextBlock>[]
                {
                    Setter<TextBlock>.Create(TextBlock.TextProperty, obj.ToString())
                }
            };
        }
    }
}
