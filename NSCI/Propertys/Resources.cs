using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace NSCI.Propertys
{
    public static partial class X
    {
        [NDProperty.NDPAttach]
        private static void OnValuesChanging(NDProperty.Propertys.OnChangingArg<NDPConfiguration, Resources, object> arg) { }

        [NDProperty.NDPAttach]
        private static void OnKeyChanging(NDProperty.Propertys.OnChangingArg<NDPConfiguration, string, object> arg) { }

        [NDProperty.NDPAttach]
        private static void OnTargetTypeChanging(NDProperty.Propertys.OnChangingArg<NDPConfiguration, Type, object> arg) { }

    }

    public partial class Resources : IResources
    {
        public ICollection<object> Children => this.allEntrys;

        IReadOnlyCollection<object> IResources.Children => new ReadOnlyCollection<object>(this.allEntrys);


        private readonly ObservableCollection<object> allEntrys = new ObservableCollection<object>();

        private readonly Dictionary<Type, object> typeLookup = new Dictionary<Type, object>();
        private readonly Dictionary<string, object> keyLookup = new Dictionary<string, object>();

        public object this[string key]
        {
            get
            {
                if (this.keyLookup.ContainsKey(key))
                    return this.keyLookup[key];
                return null;
            }
        }
        public object this[Type type]
        {
            get
            {
                if (this.typeLookup.ContainsKey(type))
                    return this.typeLookup[type];
                return null;
            }
        }


        public Resources()
        {
            this.allEntrys.CollectionChanged += AllEntrys_CollectionChanged;
        }

        private void AllEntrys_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    if (e.NewItems != null)
                        foreach (var item in e.NewItems)
                        {
                            var key = X.Key[item].Value;
                            var type = X.TargetType[item].Value;
                            if (key != null)
                                this.keyLookup.Add(key, item);
                            if (type != null)
                                this.typeLookup.Add(type, item);
                            if (type == null && key == null)
                                throw new ArgumentException("Resurce must have Key or TargetType");
                        }

                    if (e.OldItems != null)
                        foreach (var item in e.OldItems)
                        {
                            var key = X.Key[item].Value;
                            var type = X.TargetType[item].Value;
                            if (key != null)
                                this.keyLookup.Remove(key);
                            if (type != null)
                                this.typeLookup.Remove(type);
                        }

                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    this.keyLookup.Clear();
                    this.typeLookup.Clear();
                    foreach (var item in this.allEntrys)
                    {
                        var key = X.Key[item].Value;
                        var type = X.TargetType[item].Value;
                        if (key != null)
                            this.keyLookup.Add(key, item);
                        if (type != null)
                            this.typeLookup.Add(type, item);
                        if (type == null && key == null)
                            throw new ArgumentException("Resurce must have Key or TargetType");
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                default:
                    break;
            }
        }
    }

}
