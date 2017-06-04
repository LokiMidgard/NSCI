using System;
using System.Collections.Generic;
using System.Text;

namespace NSCI.UI
{
    public class Pen : RectPen
    {
        internal readonly char horizontal;
        internal readonly char vertical;
        public Pen(char horizontal, char vertical, char topLeft, char topRight, char bottomLeft, char bottemRight) : base(horizontal, horizontal, vertical, vertical, topLeft, topRight, bottomLeft, bottemRight)
        {
            this.horizontal = horizontal;
            this.vertical = vertical;
        }

        public static Pen DoubleLine { get; } = new UI.Pen('═', '║', '╔', '╗', '╚', '╝');
        public static Pen SingelLine { get; } = new UI.Pen('─', '│', '┌', '┐', '└', '┘');
        public static RectPen BlockLine { get; } = new RectPen('▄', '▀', '█', '█', '▄', '▄', '▀', '▀');
    }

    public class RectPen
    {
        internal readonly char top;
        internal readonly char bottom;
        internal readonly char left;
        internal readonly char right;
        internal readonly char topLeft;
        internal readonly char topRight;
        internal readonly char bottomLeft;
        internal readonly char bottemRight;
        public RectPen(char top, char bottom, char left, char right, char topLeft, char topRight, char bottomLeft, char bottemRight)
        {
            this.top = top;
            this.bottom = bottom;
            this.left = left;
            this.right = right;
            this.topLeft = topLeft;
            this.topRight = topRight;
            this.bottomLeft = bottomLeft;
            this.bottemRight = bottemRight;
        }

    }
}
