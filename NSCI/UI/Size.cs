using System;

namespace NSCI.UI
{
    public struct Size
    {

        public static readonly Size Empty = new Size();

        public bool IsEmpty => Width == 0 && Height == 0;
        public int Width { get; }
        public int Height { get; }

        public Size(Point pt)
        {
            Width = pt.X;
            Height = pt.Y;
        }

        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public static Size operator +(Size sz1, Size sz2) => Add(sz1, sz2);

        public static Size operator -(Size sz1, Size sz2) => Subtract(sz1, sz2);

        public static bool operator ==(Size sz1, Size sz2) => sz1.Width == sz2.Width && sz1.Height == sz2.Height;

        public static bool operator !=(Size sz1, Size sz2) => !(sz1 == sz2);

        public static explicit operator Point(Size size) => new Point(size.Width, size.Height);



        public static Size Add(Size sz1, Size sz2) => new Size(sz1.Width + sz2.Width, sz1.Height + sz2.Height);


        public static Size Subtract(Size sz1, Size sz2) => new Size(sz1.Width - sz2.Width, sz1.Height - sz2.Height);

        public bool Equals(Size other) => (other.Width == Width) && (other.Height == Height);

        public override bool Equals(object obj)
        {
            if ((obj is Size other))
                return Equals(other);
            return false;

        }

        public override int GetHashCode() => Width ^ (13 * Height);

        public override string ToString() => $"{{Width={Width}, Height={Height}}}";

        internal Size Inflat(int width, int height)
        {
            return new Size(Width + width, Height + height);
        }
    }
}