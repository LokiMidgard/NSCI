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

        }

        [NDP]
        protected virtual void OnTextChanging(global::NDProperty.Propertys.OnChangingArg<NDPConfiguration, string> arg)
        {
            if (arg.Property.IsObjectValueChanging)
                arg.ExecuteAfterChange += (sender, e) =>
                {
                    CurserPosition = Math.Min(CurserPosition, arg.Property.NewValue.Length);
                };
        }

        [NDP]
        protected virtual void OnCurserPositionChanging(global::NDProperty.Propertys.OnChangingArg<NDPConfiguration, int> arg)
        {
            if (arg.Provider.HasNewValue)
                arg.Provider.MutatedValue = Math.Max(0, Math.Min(Text.Length, arg.Provider.NewValue));
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
            {
                Text = Text.Insert(CurserPosition, "\n");
                CurserPosition++;
            }
            else if (AcceptTab && keyInfo.Key == ConsoleKey.Tab)
            {
                Text = Text.Insert(CurserPosition, "\t");
                CurserPosition++;
            }
            else if (keyInfo.Key == ConsoleKey.Backspace)
            {
                if (CurserPosition > 0)
                {
                    bool currentPositionIsLast = CurserPosition == Text.Length;
                    Text = Text.Remove(CurserPosition - 1, 1);
                    if (!currentPositionIsLast)
                        CurserPosition--; // when we were already at the last position, changing the text will update curserPosition.
                }
            }
            else if (keyInfo.Key == ConsoleKey.Delete)
            {
                if (CurserPosition < Text.Length)
                    Text = Text.Remove(CurserPosition, 1);
            }
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                Text = Text.Insert(CurserPosition, keyInfo.KeyChar.ToString());
                CurserPosition++;
            }
            else
                return false;
            return true;
        }

    }
}
