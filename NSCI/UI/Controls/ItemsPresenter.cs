using System;
using System.Collections.Generic;
using System.Text;
using NDProperty;
using NSCI.UI.Controls.Layout;

namespace NSCI.UI.Controls
{
    public partial class ItemsPresenter : FrameworkElement
    {

        [NDP]
        private void OnPanelChanging(NDProperty.Propertys.OnChangingArg<Propertys.NDPConfiguration, Panel> arg)
        {

        }

        internal void Add(object item)
        {
            throw new NotImplementedException();
        }

        internal void Clear()
        {
            throw new NotImplementedException();
        }
    }
}
