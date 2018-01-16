using System;
using System.Collections.Generic;
using System.Text;
using NDProperty;
using NDProperty.Providers.Binding;
using NSCI.Propertys;

namespace NSCI.UI.Controls
{
    public partial class TextBox : Control
    {
        static TextBox()
        {

            Template.SetDefaultControlTemplate(Template.CreateControlTemplate((TextBox textBox) =>
            {
                var textblock = new TextBlock() { MinHeight = 1 };

                TextBlock.TextProperty.Bind(textblock, TextBox.TextProperty.Of(textBox).OneWay());
                //FrameworkElement.ForegroundProperty.Bind(textblock as FrameworkElement, TextBox.HasFocusProperty.Of(textBox).ConvertOneWay(state => state ? ConsoleColor.Yellow : ConsoleColor.White));
                FrameworkElement.BackgroundProperty.Bind(textblock as FrameworkElement, TextBox.HasFocusProperty.Of(textBox).ConvertOneWay(state => state ? ConsoleColor.Cyan : ConsoleColor.Gray));
                return textblock;
            }));

        }

        public TextBox()
        {
            IsEnabledChanged += (sender, e) => InvalidateRender();
        }

        [NDP]
        protected virtual void OnTextChanging(global::NDProperty.Propertys.OnChangingArg<NDPConfiguration, string> arg)
        {

        }

        [NDP]
        protected virtual void OnAcceptReturnChanging(global::NDProperty.Propertys.OnChangingArg<NDPConfiguration, bool> arg)
        {

        }

        [NDP]
        protected virtual void OnAcceptTabChanging(global::NDProperty.Propertys.OnChangingArg<NDPConfiguration, bool> arg)
        {

        }


        public override bool SupportSelection => true;

        public override bool HandleInput(FrameworkElement originalTarget, ConsoleKeyInfo keyInfo)
        {
            if (Text == null)
                Text = string.Empty;
            if (AcceptReturn && keyInfo.Key == ConsoleKey.Enter)
                Text += "\n";
            if (AcceptTab && keyInfo.Key == ConsoleKey.Tab)
                Text += "\t";
            else if (!char.IsControl(keyInfo.KeyChar))
                Text += keyInfo.KeyChar;
            else
                return false;
            return true;
        }

    }
}
