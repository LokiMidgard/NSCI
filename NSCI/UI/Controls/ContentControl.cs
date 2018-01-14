using System;
using System.Collections.Generic;
using System.Text;
using NSCI.Propertys;

namespace NSCI.UI.Controls
{
    public partial class ContentControl : Control
    {

        static ContentControl()
        {
            Template.SetDefaultControlTemplate(Template.CreateControlTemplate((ContentControl c) =>
            {
                return new ContentPresenter();
            }));
        }

        public ContentControl()
        {
            DisplayContentChanged += ContentControl_DisplayContentChanged;
        }

        private void ContentControl_DisplayContentChanged(object sender, NDProperty.Propertys.ChangedEventArgs<NDPConfiguration, Control, UIElement> e)
        {
            if (e.NewValue != null)
            {
                var queue = new Queue<UIElement>();
                queue.Enqueue(e.NewValue);

                while (queue.Count != 0)
                {
                    var element = queue.Dequeue();
                    if (element is ContentPresenter p)
                    {
                        ContentPresenter = p;
                        return;
                    }
                    foreach (var child in element.VisualChildrean)
                        queue.Enqueue(child);
                }
                ContentPresenter = null;

            }
        }


        [NDProperty.NDP]
        protected virtual void OnContentPresenterChanging(NDProperty.Propertys.OnChangingArg<NDPConfiguration, ContentPresenter> arg)
        {
            if (arg.Property.IsObjectValueChanging)
                arg.ExecuteAfterChange += (sender, args) => UpdateContentPresenter();
        }


        [NDProperty.NDP]
        protected virtual void OnContentChanging(NDProperty.Propertys.OnChangingArg<NDPConfiguration, object> arg)
        {
            if (arg.Property.IsObjectValueChanging)
                arg.ExecuteAfterChange += (sender, args) =>
            {
                if (arg.Property.OldValue is UIElement oldValue)
                    if (oldValue != null)
                        oldValue.LogicalParent = null;

                if (arg.Property.NewValue is UIElement newValue)
                {
                    if (newValue.LogicalParent != null)
                        throw new ArgumentException($"The Element is already Chiled of {newValue.VisualParent}");

                    if (newValue != null)
                        newValue.LogicalParent = this;
                }
                UpdateContentPresenter();
            };
        }

        private void UpdateContentPresenter()
        {
            if (ContentPresenter != null)
                ContentPresenter.Content = Content;

            //InvalidateMeasure(); Should be done when Content on ContentPresenter is set.
        }


    }
}
