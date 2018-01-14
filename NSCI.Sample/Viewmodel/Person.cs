using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDProperty;

namespace NSCI.Sample.Viewmodel
{
    partial class Person
    {
        [NDP(Settings = NDProperty.Propertys.NDPropertySettings.ReadOnly)]
        [DefaultValue("")]
        protected void OnNameChanging(NDProperty.Propertys.OnChangingArg<NSCI.Propertys.NDPConfiguration, string> arg) { }

        [NDP]
        [DefaultValue("")]
        protected void OnSureNameChanging(NDProperty.Propertys.OnChangingArg<NSCI.Propertys.NDPConfiguration, string> arg)
        {
            if (arg.Property.IsObjectValueChanging)
                Name = $"{FirstName} {arg.Property.NewValue}";
        }

        [NDP]
        [DefaultValue("")]
        protected void OnFirstNameChanging(NDProperty.Propertys.OnChangingArg<NSCI.Propertys.NDPConfiguration, string> arg)
        {
            if (arg.Property.IsObjectValueChanging)
                Name = $"{arg.Property.NewValue} {SureName}";
        }

        [NDP]
        [DefaultValue("")]
        protected void OnCityChanging(NDProperty.Propertys.OnChangingArg<NSCI.Propertys.NDPConfiguration, string> arg) { }

        [NDP]
        [DefaultValue("")]
        protected void OnStreetChanging(NDProperty.Propertys.OnChangingArg<NSCI.Propertys.NDPConfiguration, string> arg) { }




    }
}
