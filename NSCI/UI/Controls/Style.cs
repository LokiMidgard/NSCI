using System.Collections.Generic;

namespace NSCI.UI.Controls
{
    public class Style
    {
        public IEnumerable<ISetter> Setter { get; set; }
    }


    public interface ISetter
    {
        object Property { get; set; }
        object Value { get; set; }
    }

    public sealed class Setter<TValue, TType> : ISetter where TType : class
    {
        public NDProperty.Propertys.INDProperty<NDPConfiguration, TValue, TType> Property { get; set; }
        public TValue Value { get; set; }
        object ISetter.Property { get => this.Property; set => this.Property = (NDProperty.Propertys.INDProperty<NDPConfiguration, TValue, TType>)value; }
        object ISetter.Value { get => this.Value; set => this.Value = (TValue)value; }
    }
}