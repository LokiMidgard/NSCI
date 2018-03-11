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
                FrameworkElement.ForegroundProperty.Bind(textblock as FrameworkElement, TextBox.HasFocusProperty.Of(textBox).ConvertOneWay(state => state ? ConsoleColor.Black : ConsoleColor.Black));
                return textblock;
            }));

        }

        public TextBox()
        {
            UpdateCurserPosition();
            IsEnabledChanged += (sender, e) => UpdateCurserPosition();
        }

        [NDP]
        protected virtual void OnTextChanging(global::NDProperty.Propertys.OnChangingArg<NDPConfiguration, string> arg)
        {
            if (arg.Property.IsObjectValueChanging)
                arg.ExecuteAfterChange += (sender, e) =>
                {
                    CurserIndex = Math.Min(CurserIndex, arg.Property.NewValue.Length);
                };
        }

        [NDP]
        protected virtual void OnCurserIndexChanging(global::NDProperty.Propertys.OnChangingArg<NDPConfiguration, int> arg)
        {
            if (arg.Provider.HasNewValue)
                arg.Provider.MutatedValue = Math.Max(0, Math.Min(Text.Length, arg.Provider.NewValue));

            if (arg.Property.IsObjectValueChanging)
            {
                arg.ExecuteAfterChange += (sender, e) =>
                {
                    UpdateCurserPosition();
                };
            }
        }

        private void UpdateCurserPosition()
        {
            if (this.IsEnabled)
                SetCurserPosition(new Point(CurserIndex, 0));
            else
                SetCurserPosition(null);
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
                Text = Text.Insert(CurserIndex, "\n");
                CurserIndex++;
            }
            else if (AcceptTab && keyInfo.Key == ConsoleKey.Tab)
            {
                Text = Text.Insert(CurserIndex, "\t");
                CurserIndex++;
            }
            else if (keyInfo.Key == ConsoleKey.Backspace)
            {
                if (CurserIndex > 0)
                {
                    bool currentPositionIsLast = CurserIndex == Text.Length;
                    Text = Text.Remove(CurserIndex - 1, 1);
                    if (!currentPositionIsLast)
                        CurserIndex--; // when we were already at the last position, changing the text will update curserPosition.
                }
            }
            else if (keyInfo.Key == ConsoleKey.Delete)
            {
                if (CurserIndex < Text.Length)
                    Text = Text.Remove(CurserIndex, 1);
            }
            else if (keyInfo.Key == ConsoleKey.RightArrow)
            {
                if (CurserIndex < Text.Length)
                    CurserIndex++;
            }
            else if (keyInfo.Key == ConsoleKey.LeftArrow)
            {
                if (CurserIndex > 0)
                    CurserIndex--;
            }
            else if (keyInfo.Key == ConsoleKey.Home)
            {

                CurserIndex = 0;
            }
            else if (keyInfo.Key == ConsoleKey.End)
            {
                CurserIndex = Text.Length;
            }
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                Text = Text.Insert(CurserIndex, keyInfo.KeyChar.ToString());
                CurserIndex++;
            }
            else
            {
                return false;
            }
            return true;
        }

    }
}
