using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using NDProperty;
using NDProperty.Propertys;
using NSCI.Propertys;
using NSCI.UI.Controls.Layout;

namespace NSCI.UI.Controls
{
    public partial class ItemsControl
    {
        [NDPAttach(Settings = NDPropertySettings.ReadOnly)]
        protected static void OnIsSelectedChanging(global::NDProperty.Propertys.OnChangingArg<Propertys.NDPConfiguration, object, bool> arg)
        {

        }

        internal static global::NDProperty.Utils.AttachedHelper<Propertys.NDPConfiguration, object, bool> IsSelectedInternal => IsSelected;


    }
    public partial class ItemsControl<T> : Control
    {
        static ItemsControl()
        {
            Template.SetDefaultControlTemplate(Template.CreateControlTemplate((ItemsControl<T> c) =>
            {
                return new ItemsPresenter();
            }));

        }


        private readonly Dictionary<T, UIElement> displayLookup = new Dictionary<T, UIElement>();
        public ItemsControl()
        {
            DisplayContentChanged += ContentControl_DisplayContentChanged;
        }

        private void ContentControl_DisplayContentChanged(object sender, ChangedEventArgs<NDPConfiguration, Control, UIElement> e)
        {
            if (e.NewValue != null)
            {
                var queue = new Queue<UIElement>();
                queue.Enqueue(e.NewValue);

                while (queue.Count != 0)
                {
                    var element = queue.Dequeue();
                    if (element is ItemsPresenter p)
                    {
                        ItemsPresenter = p;
                        return;
                    }
                    foreach (var child in element.VisualChildrean)
                        queue.Enqueue(child);
                }
                ItemsPresenter = null;

            }
        }

        [NDProperty.NDP]
        protected virtual void OnItemsPresenterChanging(NDProperty.Propertys.OnChangingArg<NDPConfiguration, ItemsPresenter> arg)
        {
            if (arg.Property.IsObjectValueChanging)
            {
                if (arg.Property.NewValue != null)
                {
                    var template = PanelTemplate ?? Template.CreateControlTemplate((ItemsControl<T> parent) =>
                    {
                        return new StackPanel();
                    });

                    arg.Property.NewValue.Panel = template.InstanciateObject(this);
                }
                arg.ExecuteAfterChange += (sender, args) => UpdateItemsPresenter();
            }
        }

        protected virtual void UpdateItemsPresenter()
        {
            if (ItemsPresenter != null)
            {
                ItemsPresenter.Clear();
                this.displayLookup.Clear();
                if (Items != null)
                    foreach (var item in Items)
                    {
                        UIElement displayItem;

                        if (ItemsTemplate == null)
                        {
                            displayItem = new ListItem
                            {
                                Content = item
                            };
                        }
                        else
                        {
                            displayItem = ItemsTemplate.InstanciateObject(item);
                        }
                        this.displayLookup.Add(item, displayItem);
                        ItemsPresenter.Add(displayItem);
                    }
            }

        }

        protected UIElement GetViewOfItem(T item)
        {
            if (this.displayLookup.TryGetValue(item, out var element))
                return element;
            return null;
        }

        protected void SetItemSelected(T item, bool isSelected)
        {
            var view = GetViewOfItem(item);
            if (view == null)
                throw new ArgumentException("View of item not found", nameof(item));
            ItemsControl.IsSelectedInternal[view].Value = isSelected;

        }

        protected bool GetItemSelected(T item)
        {
            var view = GetViewOfItem(item);
            if (view == null)
                throw new ArgumentException("View of item not found", nameof(item));
            return ItemsControl.IsSelectedInternal[view].Value;
        }

        [NDP]
        protected void OnPanelTemplateChanging(NDProperty.Propertys.OnChangingArg<Propertys.NDPConfiguration, IControlTemplate<Panel, ItemsControl<T>>> arg)
        {
            if (arg.Property.IsObjectValueChanging)
            {
                ItemsPresenter.Panel = arg.Property.NewValue?.InstanciateObject(this) ?? new StackPanel();
                UpdateItemsPresenter();
            }
        }

        [NDP]
        protected void OnItemsTemplateChanging(NDProperty.Propertys.OnChangingArg<Propertys.NDPConfiguration, IDataTemplate<UIElement, T>> arg)
        {
            if (arg.Property.IsObjectValueChanging)
                UpdateItemsPresenter();
        }

        [NDP]
        protected void OnItemsChanging(NDProperty.Propertys.OnChangingArg<Propertys.NDPConfiguration, IEnumerable<T>> arg)
        {
            if (arg.Property.IsObjectValueChanging)
            {
                if (arg.Property.OldValue is System.Collections.Specialized.INotifyCollectionChanged occ)
                {
                    occ.CollectionChanged -= Cc_CollectionChanged;
                }
                UpdateItemsPresenter();
                if (arg.Property.NewValue is System.Collections.Specialized.INotifyCollectionChanged ncc)
                {
                    ncc.CollectionChanged += Cc_CollectionChanged;
                }
            }
        }

        private void Cc_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateItemsPresenter();
        }
    }
}
