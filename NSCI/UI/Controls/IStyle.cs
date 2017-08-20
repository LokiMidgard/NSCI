using System;
using System.Collections.Generic;
using NDProperty.Propertys;

namespace NSCI.UI.Controls
{
    public interface IStyle<TType> : IStyle where TType : class
    {
        new IEnumerable<ISetter<TType>> Setter { get; set; }
        IEnumerable<EventHandler<OnChangingArg<NDPConfiguration, IStyle>.ValueChangedEventArgs>> UpdateStyleProvider(TType control);
    }
    public interface IStyle
    {
        IEnumerable<ISetter> Setter { get; }

        IEnumerable<EventHandler<OnChangingArg<NDPConfiguration, IStyle>.ValueChangedEventArgs>> UpdateStyleProvider(object control);
    }
}