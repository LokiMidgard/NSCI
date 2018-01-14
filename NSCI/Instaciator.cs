using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using NSCI.UI.Controls;
using System.Linq;
using NDProperty.Propertys;
using NDProperty;
using NSCI.Propertys;
using NSCI.UI;

namespace NSCI
{

    public interface IControlTemplate<out TReturnType, in TTemplatedParent>
        where TTemplatedParent : UI.Controls.Control
    {
        TReturnType InstanciateObject(TTemplatedParent templateParent);
    }
    public interface IDataTemplate<out TReturnType, in TTemplatedParent>
    {
        TReturnType InstanciateObject(TTemplatedParent templateParent);
    }


    //public interface ITemplate<out TReturnType, in TToTemplate, in TTemplatedParent>
    //{
    //    /// <summary>
    //    /// Instanciates the Template
    //    /// </summary>
    //    /// <param name="templateParent">The Parent for the Object that will be tamplated.</param>
    //    /// <param name="templatingInstance">The Object which holds the Data. Can be <c>null</c></param>
    //    /// <returns>The Instance created by the Template</returns>
    //    TReturnType InstanciateObject(TTemplatedParent templateParent, TToTemplate templatingInstance);
    //}
    public static partial class Template
    {
        //public static void SetDefaultTemplate<TReturnType, TToTemplate, TTemplatedParent>(Template<TReturnType, TToTemplate, TTemplatedParent> template) where TType : UI.UIElement, new() => Propertys.Provider.TemplateProvider.SetDefaultTemplate(template);

        //public static ITemplate<UI.UIElement, object, UI.Controls.Control> GetTemplate(object objectToTemplate, UI.UIElement parentObject) => Propertys.Provider.TemplateProvider.GetTemplate<UI.UIElement>(objectToTemplate, parentObject);
        //public static ITemplate<UI.UIElement, object, UI.Controls.Control> GetTemplate(object objectToTemplate) => Propertys.Provider.TemplateProvider.GetTemplate<UI.UIElement>(objectToTemplate, null);
        private static readonly Dictionary<Type, object> controlTemplates = new Dictionary<Type, object>();
        private static readonly Dictionary<Type, object> dataTemplates = new Dictionary<Type, object>();


        public static IControlTemplate<TReturnType, TTemplatedParent> CreateControlTemplate<TReturnType, TTemplatedParent>(Func<TTemplatedParent, TReturnType> func)
            where TTemplatedParent : UI.Controls.Control
            => new DelegateControlTemplate<TReturnType, TTemplatedParent>(func);

        public static IDataTemplate<TReturnType, TTemplatedObject> CreateDataTemplate<TReturnType, TTemplatedObject>(Func<TTemplatedObject, TReturnType> func)
            => new DelegateDataTemplate<TReturnType, TTemplatedObject>(func);

        public static void SetControlTemplate<TReturnType, TTemplatedParent>(this TTemplatedParent parent, IControlTemplate<TReturnType, TTemplatedParent> template)
            where TTemplatedParent : UI.Controls.Control
            where TReturnType : UI.UIElement
        {
            parent.SetControlTemplate<TTemplatedParent>(template);
        }

        public static void SetDefaultControlTemplate<TReturnType, TTemplatedParent>(IControlTemplate<TReturnType, TTemplatedParent> controlTemplate)
            where TTemplatedParent : UI.Controls.Control
        {
            controlTemplates.Add(typeof(TTemplatedParent), controlTemplate);
        }

        public static void SetDefaultDataTemplate<TReturnType, TTemplatedObject>(IDataTemplate<TReturnType, TTemplatedObject> controlTemplate)
        {
            dataTemplates.Add(typeof(TTemplatedObject), controlTemplate);
        }



        public static UIElement InstanciateFromDefaultDataTemplate(object obj)
        {
            if (obj == null)
                return null;

            if (obj is UIElement ui)
                return ui;

            var template = typeof(Template)
                .GetMethod(nameof(GetDefaultDataTemplate), BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod)
                .MakeGenericMethod(obj.GetType())
                .Invoke(null, new object[0]);

            var toRetrun = typeof(IDataTemplate<,>)
                .MakeGenericType(typeof(object), obj.GetType())
                .GetMethod(nameof(IDataTemplate<object, object>.InstanciateObject), BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance)
                .Invoke(template, new object[] { obj });

            return toRetrun as UIElement;
        }

        public static IDataTemplate<UI.UIElement, TTEmplatedObject> GetDefaultDataTemplate<TTEmplatedObject>()
        {
            var currentType = typeof(TTEmplatedObject);
            while (currentType != null)
            {
                if (controlTemplates.ContainsKey(currentType))
                    return (IDataTemplate<UI.UIElement, TTEmplatedObject>)controlTemplates[currentType];

                currentType = currentType.BaseType;

            }
            return new DefaultDataTextTemplate<TTEmplatedObject>();
        }


        public static IControlTemplate<UI.UIElement, TTemplatedParent> GetDefaultControlTemplate<TTemplatedParent>()
            where TTemplatedParent : Control
        {
            var currentType = typeof(TTemplatedParent);
            while (currentType != null)
            {
                if (controlTemplates.ContainsKey(currentType))
                    return (IControlTemplate<UI.UIElement, TTemplatedParent>)controlTemplates[currentType];

                currentType = currentType.BaseType;

            }
            return new DefaultControlTextTemplate();
        }


        private class DelegateControlTemplate<TReturnType, TTemplatedParent> : IControlTemplate<TReturnType, TTemplatedParent>
            where TTemplatedParent : Control
        {
            private Func<TTemplatedParent, TReturnType> func;

            public DelegateControlTemplate(Func<TTemplatedParent, TReturnType> func) => this.func = func;

            public TReturnType InstanciateObject(TTemplatedParent templateParent) => this.func(templateParent);
        }
        private class DelegateDataTemplate<TReturnType, TTemplatedObject> : IDataTemplate<TReturnType, TTemplatedObject>
        {
            private Func<TTemplatedObject, TReturnType> func;

            public DelegateDataTemplate(Func<TTemplatedObject, TReturnType> func) => this.func = func;

            public TReturnType InstanciateObject(TTemplatedObject templateParent) => this.func(templateParent);
        }

    }
    //public class Template<TReturnType, TToTemplate, TTemplatedParent> : ITemplate<TReturnType, TToTemplate, TTemplatedParent>
    //    where TReturnType : UI.UIElement, new()
    //{
    //    public IEnumerable<ITemplateSetter<TReturnType>> Setter { get; set; }
    //    public TReturnType InstanciateObject(TTemplatedParent templateParent, TToTemplate datacontext)
    //    {
    //        var obj = new TReturnType();
    //        foreach (var item in Setter)
    //            item.SetValueOnObject(obj);
    //        return obj;
    //    }


    //}

    //public static class Setter<TType>
    //            where TType : UI.UIElement
    //{

    //    public static TemplateSetter<TType, TValue, TProperty> Create<TValue, TProperty>(TProperty property, ITemplate<TValue> template)
    //        where TValue : class, new()
    //        where TProperty : NDReadOnlyPropertyKey<NDPConfiguration, TType, TValue>, INDProperty<NDPConfiguration, TType, TValue>
    //    {
    //        return new TemplateSetter<TType, TValue, TProperty>() { Property = property, ValueTemplate = template };
    //    }
    //    public static ValueSetter<TType, TValue, TProperty> Create<TValue, TProperty>(TProperty property, TValue value)
    //        where TProperty : NDReadOnlyPropertyKey<NDPConfiguration, TType, TValue>, INDProperty<NDPConfiguration, TType, TValue>
    //    {
    //        return new ValueSetter<TType, TValue, TProperty>() { Property = property, Value = value };
    //    }
    //}


    //public class TemplateSetter<TType, TValue, TProperty> : ITemplateSetter<TType, TValue, TProperty>
    //    where TType : UI.UIElement
    //    where TValue : class, new()
    //    where TProperty : NDReadOnlyPropertyKey<NDPConfiguration, TType, TValue>, INDProperty<NDPConfiguration, TType, TValue>
    //{
    //    public TProperty Property { get; set; }

    //    public ITemplate<TValue> ValueTemplate { get; set; }


    //    public void SetValueOnObject(TType obj)
    //    {
    //        PropertyRegistar<NDPConfiguration>.SetValue(Property, obj, ValueTemplate.InstanciateObject(obj));
    //    }
    //}


    //public class ValueSetter<TType, TValue, TProperty> : ITemplateSetter<TType, TValue, TProperty>
    //    where TType : UI.UIElement
    //    where TProperty : NDReadOnlyPropertyKey<NDPConfiguration, TType, TValue>, INDProperty<NDPConfiguration, TType, TValue>
    //{
    //    public TProperty Property { get; set; }

    //    public TValue Value { get; set; }

    //    public void SetValueOnObject(TType obj)
    //    {
    //        PropertyRegistar<NDPConfiguration>.SetValue(Property, obj, Value);
    //    }
    //}

    //public class MarkupSetter<TType, TValue, TProperty> : ITemplateSetter<TType, TValue, TProperty>
    //where TType : UI.UIElement
    //where TValue : class, new()
    //where TProperty : NDReadOnlyPropertyKey<NDPConfiguration, TType, TValue>, INDProperty<NDPConfiguration, TType, TValue>
    //{
    //    public TProperty Property { get; set; }

    //    public MarkupExtension<TValue> Extension { get; set; }

    //    public void SetValueOnObject(TType obj)
    //    {
    //        if (Extension is MarkupValueExtension<TType, TValue> valueExtension)
    //            PropertyRegistar<NDPConfiguration>.SetValue(Property, obj, valueExtension.ProvideValue(obj));
    //        else if (Extension is MarkupSetupExtension<TType, TValue, TProperty> setupExtension)
    //            setupExtension.SetupValue(obj, Property);
    //        else
    //            throw new NotSupportedException($"This type of markup Extionsion is not supported.({Extension.GetType()})");
    //    }
    //}

}
