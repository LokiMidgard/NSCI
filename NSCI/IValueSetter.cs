using NDProperty.Propertys;
using NSCI.Propertys;

namespace NSCI
{
    public interface ITemplateSetter<TType, TValue, TProperty> : ITemplateSetter<TType>
        where TType : UI.UIElement
        where TProperty : NDReadOnlyPropertyKey<NDPConfiguration, TType, TValue>, INDProperty<NDPConfiguration, TType, TValue>
    {
        TProperty Property { get; }
    }
    public interface ITemplateSetter<in TType>
        where TType : UI.UIElement
    {
        void SetValueOnObject(TType obj);
    }
}