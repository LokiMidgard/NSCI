using System;
using System.Collections.Generic;
using System.Text;

namespace NSCI.UI.Controls
{
    public partial class SingelSelectionItemsControl<T> : ItemsControl<T>
    {


        [NDProperty.NDP]
        protected virtual void OnSelectedItemChanging(global::NDProperty.Propertys.OnChangingArg<Propertys.NDPConfiguration, T> arg)
        {
            if (arg.Property.IsObjectValueChanging)
            {
                var oldValue = arg.Property.OldValue;
                var newValue = arg.Property.NewValue;

                if (oldValue != null)
                    SetItemSelected(oldValue, false);

                if (newValue != null)
                    SetItemSelected(newValue, true);

            }
        }
    }
}
