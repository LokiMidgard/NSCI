using System;
using System.Collections.Generic;
using System.Text;

namespace NSCI.UI
{
    public class SolidColorBrush : Brush
    {
        private ConsoleColor Color { get; }

        public SolidColorBrush(ConsoleColor color)
        {
            Color = color;
        }
    }
}
