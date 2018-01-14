using NSCI.UI;
using NSCI.UI.Controls;

namespace NSCI
{
    internal class DefaultControlTextTemplate : IControlTemplate<UIElement, UI.Controls.Control>
    {

        public UIElement InstanciateObject(Control templateParent) 
            => new UI.Controls.TextBlock { Text = templateParent?.ToString() ?? "NULL" };

    }
    internal class DefaultDataTextTemplate<T> : IDataTemplate<UIElement, T>
    {
        public UIElement InstanciateObject(T templateParent) => new UI.Controls.TextBlock { Text = templateParent?.ToString() ?? "NULL" };
    }
}