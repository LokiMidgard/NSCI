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
    public partial class ItemsControl<T> : Control
    {

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
                    arg.Property.NewValue.Panel = PanelTemplate.InstanciateObject(this);
                arg.ExecuteAfterChange += (sender, args) => UpdateItemsPresenter();
            }
        }

        private void UpdateItemsPresenter()
        {
            if (ItemsPresenter != null)
            {
                ItemsPresenter.Clear();
                if (Items != null)
                    foreach (var item in Items)
                    {
                        UIElement displayItem;

                        if (ItemsTemplate == null)
                        {
                            displayItem = new ContentPresenter
                            {
                                Content = item
                            };
                        }
                        else
                        {
                            displayItem = ItemsTemplate.InstanciateObject(item);
                        }

                        ItemsPresenter.Add(displayItem);
                    }
            }

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
            // we don't have animations so visualy it is no difference if we clear the collection and rebuild it from scrap
            // or if we actually insert an item.

            // of course Performance could be a problem with big collections. But we can fix this later ;)


            UpdateItemsPresenter();


            //switch (e.Action)
            //{
            //    case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
            //        break;
            //    case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
            //        break;
            //    case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
            //        break;
            //    case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
            //        break;
            //    case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
            //        break;
            //    default:
            //        break;
            //}
        }
    }
}
