using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using NSCI.UI.Controls;
using System.Linq;
using NDProperty.Propertys;
using NDProperty;

namespace NSCI
{
    public interface ITemplate<out TType> where TType : class, new()
    {
        TType InstanciateObject();
    }
    public class Template<TType> :ITemplate<TType>
        where TType : class, new()
    {
        public IEnumerable<ITemplateSetter<TType>> Setter { get; set; }
        public TType InstanciateObject()
        {
            var obj = new TType();

            foreach (var item in Setter)
                item.SetValueOnObject(obj);
            return obj;
        }

    }

    public class TemplateSetter<TValue, TType, TProperty> : ITemplateSetter<TValue, TType, TProperty>
        where TType : class
        where TValue : class, new()
        where TProperty : NDReadOnlyPropertyKey<NDPConfiguration, TValue, TType>, INDProperty<NDPConfiguration, TValue, TType>
    {
        public TProperty Property { get; set; }

        public ITemplate<TValue> ValueTemplate { get; set; }

        public TValue Value => ValueTemplate.InstanciateObject();

        public void SetValueOnObject(TType obj)
        {
            PropertyRegistar<NDPConfiguration>.SetValue(Property, obj, Value);
        }
    }
    public class ValueSetter<TValue, TType, TProperty> : ITemplateSetter<TValue, TType, TProperty>
        where TType : class
        where TProperty : NDReadOnlyPropertyKey<NDPConfiguration, TValue, TType>, INDProperty<NDPConfiguration, TValue, TType>
    {
        public TProperty Property { get; set; }

        public TValue Value { get; set; }

        public void SetValueOnObject(TType obj)
        {
            PropertyRegistar<NDPConfiguration>.SetValue(Property, obj, Value);
        }
    }
}
