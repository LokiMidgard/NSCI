namespace NSCI.UI
{
    public struct Point
    {
        public static readonly Point Empty = new Point();

        public bool IsEmpty => X == 0 && Y == 0;
        public int X { get; }
        public int Y { get; }


        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }


        public Point(Size sz)
        {
            X = sz.Width;
            Y = sz.Height;
        }




        public static explicit operator Size(Point p) => new Size(p.X, p.Y);

        public static Point operator +(Point pt, Size sz) => Add(pt, sz);

        public static Point operator -(Point pt, Size sz) => Subtract(pt, sz);

        public static bool operator ==(Point left, Point right) => left.X == right.X && left.Y == right.Y;

        public static bool operator !=(Point left, Point right) => !(left == right);

        public static Point Add(Point pt, Size sz) => new Point(pt.X + sz.Width, pt.Y + sz.Height);

        public static Point Subtract(Point pt, Size sz) => new Point(pt.X - sz.Width, pt.Y - sz.Height);

        public override bool Equals(object obj)
        {
            if (obj is Point p)
                return Equals(p);
            return false;
        }

        public bool Equals(Point other) => other.X == X && other.Y == Y;

        public override int GetHashCode() => unchecked(X ^ (13 * Y));

        public Point Offset(int dx, int dy) => new Point(X + dx, Y + dy);

        public Point Offset(Point p) => Offset(p.X, p.Y);
        public override string ToString() => $"{{X={X},Y={Y}}}";

    }

}