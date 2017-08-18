using System;
using System.Collections.Generic;
using System.Text;
using NDProperty.Providers;

namespace NSCI
{
    public class NDPConfiguration : NDProperty.IInitilizer<NDPConfiguration>
    {
        public IEnumerable<ValueProvider<NDPConfiguration>> ValueProvider => new ValueProvider<NDPConfiguration>[] 
        {
            NDProperty.Providers.LocalValueProvider<NDPConfiguration>.Instance,
            StyleValueProvider.Instance,
            NDProperty.Providers.InheritenceValueProvider<NDPConfiguration>.Instance,
            NDProperty.Providers.DefaultValueProvider<NDPConfiguration>.Instance,
        };
    }
}
