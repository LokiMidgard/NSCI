using System;
using System.Collections.Generic;
using System.Text;
using NDProperty.Providers;
using NSCI.Propertys.Provider;

namespace NSCI.Propertys
{
    public class NDPConfiguration : NDProperty.IInitilizer<NDPConfiguration>
    {
        public IEnumerable<ValueProvider<NDPConfiguration>> ValueProvider => new ValueProvider<NDPConfiguration>[] 
        {
            LocalValueProvider<NDPConfiguration>.Instance,
            StyleValueProvider.Instance,
            InheritenceValueProvider<NDPConfiguration>.Instance,
            TemplateProvider.Instance,
            DefaultValueProvider<NDPConfiguration>.Instance,
        };
    }
}
