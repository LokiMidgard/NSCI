using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NDProperty;
using NDProperty.Propertys;
using NDProperty.Providers.Binding;
using NSCI.Propertys;

namespace NSCI.UI.Controls
{
    public partial class Button : ContentControl
    {
        static Button()
        {
            Template.SetDefaultControlTemplate(Template.CreateControlTemplate((Button b) =>
            {
                var outerBorder = new Border
                {
                    Foreground = ConsoleColor.Gray
                };

                var innerBorder = new Border() { BorderStyle = BorderStyle.SingelLined, Background = ConsoleColor.DarkRed };
                outerBorder.Child = innerBorder;

                var contentPresenter = new ContentPresenter() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                innerBorder.Child = contentPresenter;

                Border.ForegroundProperty.Bind(innerBorder as FrameworkElement, HasFocusProperty.Of(b).ConvertOneWay(state => state ? ConsoleColor.Yellow : ConsoleColor.White));

                //Border.BackgroundProperty.Bind(innerBorder as FrameworkElement, Button.IsPressedProperty.Of(b).ConvertOneWay(state => state ? ConsoleColor.Red : ConsoleColor.DarkRed));
                Border.PaddingProperty.Bind(outerBorder as FrameworkElement, Button.IsPressedProperty.Of(b).ConvertOneWay(state => state ? new Thickness(2, 2, 0, 0) : new Thickness(1, 1, 0, 0)));
                Border.BorderStyleProperty.Bind(outerBorder, Button.IsPressedProperty.Of(b).ConvertOneWay(state => state ? BorderStyle.None : BorderStyle.DropShadowLight));


                return outerBorder;
            }));
        }

        public override bool SupportSelection => true;

        public event EventHandler ButtonPressed;

        public Button()
        {
            Up();
        }

        private void Up() => IsPressed = false;
        private void Down() => IsPressed = true;



        [NDP(Settings = NDPropertySettings.ReadOnly)]
        protected virtual void OnIsPressedChanging(OnChangingArg<NDPConfiguration, bool> arg)
        {

        }


        public override bool HandleInput(FrameworkElement originalTarget, ConsoleKeyInfo k)
        {
            if (k.Key == ConsoleKey.Enter)
            {
#pragma warning disable CS4014 // Da dieser Aufruf nicht abgewartet wird, wird die Ausführung der aktuellen Methode fortgesetzt, bevor der Aufruf abgeschlossen ist
                AnimatePressButtonAsync();
#pragma warning restore CS4014 // Da dieser Aufruf nicht abgewartet wird, wird die Ausführung der aktuellen Methode fortgesetzt, bevor der Aufruf abgeschlossen ist

                return true;
            }
            return false;
        }

        private async System.Threading.Tasks.Task AnimatePressButtonAsync()
        {
            Down();
            ButtonPressed?.Invoke(this, EventArgs.Empty);
            await Task.Delay(500);
            Up();
        }
    }
}
