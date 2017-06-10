using System;

namespace NSCI.UI
{
    public struct Rect
    {
        public static readonly Rect Empty = new Rect();



        public Rect(int x, int y, int width, int height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        public Rect(Point location, Size size)
        {
            this.X = location.X;
            this.Y = location.Y;
            this.Width = size.Width;
            this.Height = size.Height;
        }


        public Point Location => new Point(X, Y);

        public Size Size => new Size(Width, Height);

        public int X { get; }

        public int Y { get; }

        public int Width { get; }

        public int Height { get; }

        public int Left => X;

        public int Top => Y;

        public int Right => X + Width;

        public int Bottom => Y + Height;

        public bool IsEmpty => Height == 0 && Width == 0 && X == 0 && Y == 0;

        public Point Center => Location.Offset(Width / 2, Height / 2);

        public bool Equals(Rect other) => (other.X == X) && (other.Y == Y) && (other.Width == Width) && (other.Height == Height);
        public override bool Equals(object obj)
        {
            if (obj is Rect other)
                return Equals(other);
            return false;
        }


        public static bool operator ==(Rect left, Rect right) => (left.X == right.X && left.Y == right.Y && left.Width == right.Width && left.Height == right.Height);
        public static bool operator !=(Rect left, Rect right) => !(left == right);

        public bool Contains(int x, int y) => X <= x && x < X + Width && Y <= y && y < Y + Height;

        public bool Contains(Point pt) => Contains(pt.X, pt.Y);

        public bool Contains(Rect rect) => (X <= rect.X) && ((rect.X + rect.Width) <= (X + Width)) && (Y <= rect.Y) && ((rect.Y + rect.Height) <= (Y + Height));

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 13;
                hash = (hash * 31) ^ X;
                hash = (hash * 31) ^ Y;
                hash = (hash * 31) ^ Width;
                hash = (hash * 31) ^ Height;
                return hash;
            }
        }

        public Rect Inflate(int width, int height) => new Rect(X - width, Y - height, Width + 2 * width, Height + 2 * height);

        public Rect Inflate(Size size) => Inflate(size.Width, size.Height);

        public Rect Intersect(Rect b)
        {
            int x1 = Math.Max(X, b.X);
            int x2 = Math.Min(X + Width, b.X + b.Width);
            int y1 = Math.Max(Y, b.Y);
            int y2 = Math.Min(Y + Height, b.Y + b.Height);

            if (x2 >= x1 && y2 >= y1)
                return new Rect(x1, y1, x2 - x1, y2 - y1);
            return Rect.Empty;
        }

        public bool IntersectsWith(Rect rect) => (rect.X < X + Width) && (X < (rect.X + rect.Width)) && (rect.Y < Y + Height) && (Y < rect.Y + rect.Height);

        public static Rect Union(Rect a, Rect b)
        {
            int x1 = Math.Min(a.X, b.X);
            int x2 = Math.Max(a.X + a.Width, b.X + b.Width);
            int y1 = Math.Min(a.Y, b.Y);
            int y2 = Math.Max(a.Y + a.Height, b.Y + b.Height);

            return new Rect(x1, y1, x2 - x1, y2 - y1);
        }

        public Rect Offset(Point pos) => Offset(pos.X, pos.Y);

        public Rect Offset(int x, int y) => new Rect(X + x, Y + y, Width, Height);

        public override string ToString() => $"{{X={X},Y={Y},Width={Width},Height={Height}}}";

        internal Rect Translate(Size translation) => new Rect(Location + translation, Size);

    }

}