using System;
using System.Collections.Generic;
using System.Text;

namespace NSCI.UI
{
    public class Pen
    {
        internal readonly char horizontal;
        internal readonly char vertical;
        internal readonly char topLeft;
        internal readonly char topRight;
        internal readonly char bottomLeft;
        internal readonly char bottemRight;
        public Pen(char horizontal, char vertical, char topLeft, char topRight, char bottomLeft, char bottemRight)
        {
            this.horizontal = horizontal;
            this.vertical = vertical;
            this.topLeft = topLeft;
            this.topRight = topRight;
            this.bottomLeft = bottomLeft;
            this.bottemRight = bottemRight;
        }

        public static Pen DoubleLine { get; } = new UI.Pen('═', '║', '╔', '╗', '╚', '╝');
        public static Pen SingelLine { get; } = new UI.Pen('─', '│', '┌', '┐', '└', '┘');
    }
}
