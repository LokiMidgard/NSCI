using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NSCI.UI.Controls
{
    public partial class SingleSelectionItemsControl<T> : ItemsControl<T>
    {

        public override bool SupportSelection => true;

        public override bool HandleInput(FrameworkElement originalTarget, ConsoleKeyInfo keyInfo)
        {
            if ((this.Items?.Count() ?? 0) > 0)
            {
                if (this.SelectedItem == null)
                {
                    this.SelectedItem = this.Items.First();
                    return true;
                }
                else if (keyInfo.Key == ConsoleKey.UpArrow)
                {

                    var iterator = Items.GetEnumerator();
                    T previousElement = default;
                    bool wasFound = false;
                    foreach (var item in Items)
                    {
                        if (SelectedItem.Equals(item))
                        {
                            if (wasFound)
                            {
                                SelectedItem = previousElement;
                                return true;
                            }
                            return false;
                        }
                        previousElement = item;
                        wasFound = true;
                    }
                    return false;
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    bool wasFound = false;
                    foreach (var item in Items)
                    {
                        if (wasFound)
                        {
                            SelectedItem = item;
                            return true;
                        }
                        if (SelectedItem.Equals(item))
                            wasFound = true;
                    }
                    return false;
                }
            }
            return base.HandleInput(originalTarget, keyInfo);
        }

        [NDProperty.NDP]
        protected virtual void OnSelectedItemChanging(global::NDProperty.Propertys.OnChangingArg<Propertys.NDPConfiguration, T> arg)
        {
            if (arg.Property.IsObjectValueChanging)
            {
                var oldValue = arg.Property.OldValue;
                var newValue = arg.Property.NewValue;

                if (oldValue != null && GetViewOfItem(oldValue) != null) // HACK: We need SelectedIndex and should track the selected items when items before the index get deleted.
                    SetItemSelected(oldValue, false);

                if (newValue != null)
                    SetItemSelected(newValue, true);

            }
        }

    }
}
