using System;
using System.Runtime.CompilerServices;
using NDProperty.Propertys;
using NDProperty.Providers;
using NSCI.UI.Controls;

namespace NSCI
{
    internal class StyleValueProvider : ValueProvider<NDPConfiguration>
    {
        public static StyleValueProvider Instance { get; } = new StyleValueProvider();

        private StyleValueProvider()
        {

        }

        public override (TValue value, bool hasValue) GetValue<TValue, TType>(TType targetObject, NDProperty.Propertys.NDReadOnlyPropertyKey<NDPConfiguration, TValue, TType> property)
        {
            if (targetObject is UI.Controls.Control control)
            {
                foreach (var setter in control.Style.Setter)
                {
                    if (setter.Property == property)
                        return ((TValue)setter.Value, true);
                }
            }
            return (default(TValue), false);
        }


        internal bool Update<TValue, TType, TPropertyType>(IStyle style, TType targetObject, TPropertyType property, TValue value, TValue oldValue, ValueProvider<NDPConfiguration> oldProvider)
            where TType : class
            where TPropertyType : NDReadOnlyPropertyKey<NDPConfiguration, TValue, TType>, INDProperty<NDPConfiguration, TValue, TType>
        {
            return base.Update(style, targetObject, property, value, oldValue, oldProvider);

        }


    }
}