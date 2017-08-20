using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using NSCI.UI.Controls;
using System.Linq;

namespace NSCI
{
    public class Instaciator<TType> where TType : class, new()
    {
        public IEnumerable<ISetter<TType>> Setter { get; set; }
        public TType InstanciateObject()
        {
            var obj = new TType();

            foreach (var item in Setter)
                item.SetOnObject(obj);
            return obj;
        }

    }
}
