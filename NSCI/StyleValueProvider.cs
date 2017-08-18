using System.Runtime.CompilerServices;
using NDProperty.Providers;

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
    }
}