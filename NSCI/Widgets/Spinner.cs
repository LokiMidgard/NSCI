using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NSCI.Widgets
{
    public class Spinner : Widget
    {
        internal Spinner()
        {
        }

        public Spinner(Widget parent) : base(parent)
        {
            Background = Parent.Background;
            Foreground = ConsoleColor.White;
            SelectedBackground = ConsoleColor.Magenta;
            ActiveBackground = ConsoleColor.DarkMagenta;
        }



        private async void Timerproceded()
        {
            while (this.timerStarted)
            {
                Draw();
                await Task.Delay(100);
            }
        }

        private string RotateString(string Input, int Offset)
        {
            return Input.Substring(Offset) + Input.Substring(0, Offset);
        }

        private int Cycle = 8;
        private string Chars = "    ░▒▓█";
        private bool timerStarted;

        internal override void Render()
        {
            this.Cycle = (this.Cycle - 1);
            if (this.Cycle == 0) { this.Cycle = 8; }
            var DrawStr = RotateString(this.Chars, this.Cycle);
            ConsoleHelper.DrawText(DisplayLeft, DisplayTop, Foreground, Background, DrawStr.Substring(0, 3));
            ConsoleHelper.DrawText(DisplayLeft, DisplayTop + 1, Foreground, Background, "{0} {1}", DrawStr[7], DrawStr[3]);
            ConsoleHelper.DrawText(DisplayLeft, DisplayTop + 2, Foreground, Background, "{0}{1}{2}", DrawStr[6], DrawStr[5], DrawStr[4]);
        }

        private bool _spinning = false;
        [XmlAttribute]
        public bool Spinning
        {
            get
            {
                return this._spinning;
            }
            set
            {
                this._spinning = value;
                if (value)
                {
                    this.timerStarted = true;
                    Timerproceded();
                }
                else
                {
                    this.timerStarted = false;
                }
            }
        }
    }
}
