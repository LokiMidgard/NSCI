using System;
using System.Collections.Generic;

namespace NSCI.Propertys
{
    public interface IResources
    {
        object this[string key] { get; }
        object this[Type type] { get; }

        IReadOnlyCollection<object> Children { get; }
    }
}