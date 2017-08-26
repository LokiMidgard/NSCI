using System;
using System.Collections.Generic;
using NDProperty;
using NDProperty.Propertys;
using NDProperty.Providers;

namespace NSCI.UI.Controls
{
    public class Style<TType> : IStyle<TType> where TType : class
    {
        public IEnumerable<ISetter<TType>> Setter { get; set; }
        IEnumerable<ISetter> IStyle.Setter { get => this.Setter; }

        public IEnumerable<EventHandler<OnChangingArg<NDPConfiguration, IStyle>.ValueChangedEventArgs>> UpdateStyleProvider(TType control)
        {
            List<EventHandler<OnChangingArg<NDPConfiguration, IStyle>.ValueChangedEventArgs>> handlers = new List<EventHandler<OnChangingArg<NDPConfiguration, IStyle>.ValueChangedEventArgs>>();
            foreach (var item in Setter)
            {
                var (oldValue, oldProvider) = item.GetCurrentValueAndProvider(control);
                handlers.Add((sender, e) => item.NotifyValueProvider(this, control, oldValue, oldProvider));
            }
            return handlers;
        }

        IEnumerable<EventHandler<OnChangingArg<NDPConfiguration, IStyle>.ValueChangedEventArgs>> IStyle.UpdateStyleProvider(object targetObject)
        {
            if (targetObject is TType correctType)
                return this.UpdateStyleProvider(correctType);
            else
                throw new ArgumentException($"Target object is not of Type {typeof(TType).Name}. (Was {targetObject?.GetType().Name ?? "null"})");
        }
    }


    public interface ISetter<TType> : ISetter where TType : class
    {
        void SetOnObject(TType targetObject);
        void NotifyValueProvider(IStyle style, TType targetObject, object oldValue, NDProperty.Providers.ValueProvider<NDPConfiguration> oldProvider);
        (object value, ValueProvider<NDPConfiguration> provider) GetCurrentValueAndProvider(TType control);
    }
    public interface ISetter<TType, TValue> : ISetter<TType> where TType : class
    {
        void NotifyValueProvider(IStyle style, TType targetObject, TValue oldValue, NDProperty.Providers.ValueProvider<NDPConfiguration> oldProvider);
        new (TValue value, ValueProvider<NDPConfiguration> provider) GetCurrentValueAndProvider(TType control);
    }

    public interface ISetter
    {
        object Property { get; set; }
        object Value { get; set; }
        void SetOnObject(object targetObject);
        void NotifyValueProvider(IStyle style, object targetObject, object oldValue, NDProperty.Providers.ValueProvider<NDPConfiguration> oldProvider);
        (object value, ValueProvider<NDPConfiguration> provider) GetCurrentValueAndProvider(object control);
    }


    public sealed class Setter<TValue, TType, TPropertyType> : ISetter<TType, TValue>
        where TType : class
        where TPropertyType : NDReadOnlyPropertyKey<NDPConfiguration, TValue, TType>, INDProperty<NDPConfiguration, TValue, TType>
    {

        public TPropertyType Property { get; set; }
        public TValue Value { get; set; }
        object ISetter.Property { get => this.Property; set => this.Property = (TPropertyType)value; }
        object ISetter.Value { get => this.Value; set => this.Value = (TValue)value; }

        public void NotifyValueProvider(IStyle style, TType targetObject, TValue oldValue, NDProperty.Providers.ValueProvider<NDPConfiguration> oldProvider)
        {
            StyleValueProvider.Instance.Update(style, targetObject, Property, Value, oldValue, oldProvider);
        }

        public void SetOnObject(TType targetObject)
        {
            if (Property is NDProperty.Propertys.NDPropertyKey<NDPConfiguration, TValue, TType> normalProperty)
                NDProperty.PropertyRegistar<NDPConfiguration>.SetValue(normalProperty, targetObject, Value);
            else if (Property is NDProperty.Propertys.NDAttachedPropertyKey<NDPConfiguration, TValue, TType> attachedProperty)
                NDProperty.PropertyRegistar<NDPConfiguration>.SetValue(attachedProperty, targetObject, Value);
            else
                throw new NotSupportedException();
        }


        void ISetter.SetOnObject(object targetObject)
        {
            if (targetObject is TType correctType)
                this.SetOnObject(correctType);
            else
                throw new ArgumentException($"Target object is not of Type {typeof(TType).Name}. (Was {targetObject?.GetType().Name ?? "null"})");
        }


        void ISetter<TType>.NotifyValueProvider(IStyle style, TType targetObject, object oldValue, ValueProvider<NDPConfiguration> oldProvider)
        {
            if (oldValue is TValue correctType)
                this.NotifyValueProvider(style, targetObject, correctType, oldProvider);
            else
                throw new ArgumentException($"Target object is not of Type {typeof(TType).Name}. (Was {targetObject?.GetType().Name ?? "null"})");
        }

        void ISetter.NotifyValueProvider(IStyle style, object targetObject, object oldValue, ValueProvider<NDPConfiguration> oldProvider)
        {
            if (targetObject is TType correctTargetObject && oldValue is TValue correctOldValue)
                this.NotifyValueProvider(style, correctTargetObject, correctOldValue, oldProvider);
            else
                throw new ArgumentException($"Target object is not of Type {typeof(TType).Name}. (Was {targetObject?.GetType().Name ?? "null"})");
        }

        public (TValue value, ValueProvider<NDPConfiguration> provider) GetCurrentValueAndProvider(TType control)
        {
            return PropertyRegistar<NDPConfiguration>.GetValueAndProvider(Property, control);
        }


        (object value, ValueProvider<NDPConfiguration> provider) ISetter<TType>.GetCurrentValueAndProvider(TType control)
        {

            return this.GetCurrentValueAndProvider(control);
        }

        (object value, ValueProvider<NDPConfiguration> provider) ISetter.GetCurrentValueAndProvider(object targetObject)
        {
            if (targetObject is TType correctTargetObject)
                return this.GetCurrentValueAndProvider(correctTargetObject);
            else
                throw new ArgumentException($"Target object is not of Type {typeof(TType).Name}. (Was {targetObject?.GetType().Name ?? "null"})");
        }
    }
}