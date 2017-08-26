using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NDProperty;
using NDProperty.Propertys;

namespace NSCI.UI.Controls
{
    public partial class Button : Control
    {
        private readonly TextBlock text;
        private readonly Border border;

        [NDProperty.NDP]
        protected virtual void OnTextChanging(NDProperty.Propertys.OnChangingArg<NDPConfiguration,  string> arg)
        {
            this.text.Text = arg.NewValue;
        }

        public override bool SupportSelection => true;


        [NDProperty.NDP]
        protected virtual void OnTextColorChanging(NDProperty.Propertys.OnChangingArg<NDPConfiguration, ConsoleColor> arg)
        {
            arg.ExecuteAfterChange += (oldValue, newValue) =>
            {

                if (newValue != oldValue)
                    InvalidateRender();
            };
        }


        public event EventHandler ButtonPressed;

        public Button()
        {
            this.border = new Border();
            this.text = new TextBlock() { Background = ConsoleColor.Yellow, Height = 3, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            this.border.Content = this.text;
            this.border.Parent = this;
            Up();
        }

        private void Up()
        {
            this.border.BorderStyle = BorderStyle.DropShadowLight;
            this.border.Foreground = ConsoleColor.Gray;
            this.border.Padding = new Thickness(1, 1, 0, 0);
        }
        private void Down()
        {
            this.border.BorderStyle = BorderStyle.None;
            this.border.Foreground = ConsoleColor.Gray;
            this.border.Padding = new Thickness(2, 2, 0, 0);
        }

        protected override void OnHasFocusChanging(OnChangingArg<NDPConfiguration, bool> arg)
        {
            base.OnHasFocusChanging(arg);
            if (arg.NewValue)
                this.text.Foreground = ConsoleColor.Red;
            else
                this.text.Foreground = ConsoleColor.Green;
    }

    protected override Size MeasureOverride(Size availableSize)
        {
            var marginWidth = Margin.Left + Margin.Right;
            var marginHeight = Margin.Top + Margin.Bottom;

            var pardingWidth = Margin.Left + Margin.Right;
            var pardingHeight = Margin.Top + Margin.Bottom;

            this.border.Measure(base.MeasureOverride(availableSize));
            var desiredSize = this.border.DesiredSize;
            return base.MeasureOverride(desiredSize);
        }

        protected override void ArrangeOverride(Size finalSize)
        {
            base.ArrangeOverride(finalSize);
            this.border.Arrange(new Rect(Point.Empty, finalSize));
        }

        protected override void RenderOverride(IRenderFrame frame)
        {
            this.border.Render(frame);
        }


        public override bool HandleInput(Control originalTarget,ConsoleKeyInfo k)
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
