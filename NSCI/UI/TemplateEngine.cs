using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using NSCI.Propertys;

namespace NSCI.UI
{
    public static class TemplateEngine
    {
        private static readonly Dictionary<Type, ITemplate<UIElement>> globalLookup = new Dictionary<Type, ITemplate<UIElement>>();

        public static ITemplate<UIElement> GetTemplate(object obj)
        {
            var currentType = obj.GetType();

            while (currentType != null)
            {
                if (globalLookup.ContainsKey(currentType))
                    return globalLookup[currentType];
                currentType = currentType.GetTypeInfo().BaseType;
            }

            var template = new Template<Controls.TextBlock>
            {
                Setter = new ITemplateSetter<Controls.TextBlock>[]
                {
                    Setter.Create<string,Controls.TextBlock, NDProperty.Propertys.NDPropertyKey<NDPConfiguration,string,Controls.TextBlock>>(Controls.TextBlock.TextProperty, obj.ToString())
                }
            };
            return template;
        }
    }
}
