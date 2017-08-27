using NDProperty.Propertys;
using NSCI.Propertys;

namespace NSCI
{
    public interface ITemplateSetter<TValue, TType, TProperty> : ITemplateSetter<TType>
        where TType : class
        where TProperty : NDReadOnlyPropertyKey<NDPConfiguration, TValue, TType>, INDProperty<NDPConfiguration, TValue, TType>
    {
        TProperty Property { get; }
        TValue Value { get; }
    }
    public interface ITemplateSetter<in TType>
        where TType : class
    {
        void SetValueOnObject(TType obj);
    }
}