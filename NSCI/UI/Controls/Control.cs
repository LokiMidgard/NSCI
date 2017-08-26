using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using NDProperty;
using NDProperty.Propertys;


namespace NSCI.UI.Controls
{
    public abstract partial class Control : FrameworkElement
    {

        [NDProperty.NDP]
        protected virtual void OnStyleChanging(NDProperty.Propertys.OnChangingArg<NDPConfiguration, IStyle> arg)
        {
            var handlers = arg.NewValue.UpdateStyleProvider(this);
            foreach (var handler in handlers)
                arg.ExecuteAfterChange += handler;
        }


    
    }

}
