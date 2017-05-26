using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NSCI.Widgets
{
    public class TinySpinner : Widget
    {
        internal TinySpinner()
        {
        }

        public TinySpinner(Widget parent)
            : base(parent)
        {
            Background = Parent.Background;
            Foreground = ConsoleColor.White;
        }


        private async void TimerProciding()
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

        private int Cycle = 3;
        private string Chars = @"|/-\";
        private bool timerStarted;

        internal override void Render()
        {
            this.Cycle = (this.Cycle + 1) % 4;
            var c = this.Chars[this.Cycle];
            ConsoleHelper.DrawText(DisplayLeft, DisplayTop, Foreground, Background, "{0}", c);
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
                    TimerProciding();
                    ;
                }
                else
                {
                    this.timerStarted = false;
                }
            }
        }
    }
}
