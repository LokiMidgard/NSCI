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
    public partial class ItemsControl : Control
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
                arg.ExecuteAfterChange += (sender, args) => UpdateItemsPresenter();
        }

        private void UpdateItemsPresenter()
        {
            if (ItemsPresenter != null)
            {
                foreach (var item in this.Items)
                    ItemsPresenter.Add(item);
            }

        }

        [NDP]
        protected void OnPanelTemplateChanging(NDProperty.Propertys.OnChangingArg<Propertys.NDPConfiguration, IControlTemplate<Panel, ItemsControl>> arg)
        {

        }

        [NDP]
        protected void OnItemsTemplateChanging(NDProperty.Propertys.OnChangingArg<Propertys.NDPConfiguration, IControlTemplate<Panel, ItemsControl>> arg)
        {

        }

        [NDP]
        protected void OnItemsChanging(NDProperty.Propertys.OnChangingArg<Propertys.NDPConfiguration, IEnumerable<object>> arg)
        {

        }



    }
}
