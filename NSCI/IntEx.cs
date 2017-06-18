using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NSCI
{
    public struct IntEx : IComparable, IComparable<IntEx>, IEquatable<IntEx>, IFormattable
    {

        public static readonly IntEx NaN = new IntEx(NAN_VALUE);
        public static readonly IntEx PositiveInfinity = new IntEx(long.MaxValue);
        public static readonly IntEx NegativeInfinity = new IntEx(NAN_VALUE + 1);


        private readonly long value;
        private const long NAN_VALUE = long.MinValue;

        public bool IsPositiveInfinity => this.value > int.MaxValue;
        public bool IsNegativeInfinity => this.value < int.MinValue && this.value != NAN_VALUE;
        public bool IsInfinity => IsPositiveInfinity || IsNegativeInfinity;
        public bool IsNaN => this.value == NAN_VALUE;

        public bool IsNumber => !IsNaN && !IsInfinity;

        private IntEx(long value)
        {
            this.value = value;
        }

        public static IntEx operator +(IntEx a, IntEx b)
        {
            if (a.IsInfinity || b.IsInfinity)
            {
                if (!b.IsInfinity)
                    return a;
                if (!a.IsInfinity)
                    return b;
                if (a.IsPositiveInfinity == b.IsPositiveInfinity)
                    return a;

                return new IntEx(NAN_VALUE);
            }
            return new IntEx(a.value + b.value);
        }

        public static IntEx operator -(IntEx a, IntEx b)
        {
            if (a.IsInfinity || b.IsInfinity)
            {
                if (!b.IsInfinity)
                    return a;
                if (!a.IsInfinity)
                    return b;
                if (a.IsPositiveInfinity == b.IsPositiveInfinity)
                    return new IntEx(NAN_VALUE);

                return a;
            }
            return new IntEx(a.value - b.value);
        }
        public static IntEx operator *(IntEx a, IntEx b)
        {
            if (a.IsInfinity || b.IsInfinity)
            {
                if (a.value == 0 || b.value == 0)
                    return new IntEx(NAN_VALUE);


                return (a.value > 0) == (b.value > 0) ? PositiveInfinity : NegativeInfinity;
            }
            return new IntEx(a.value * b.value);
        }
        public static IntEx operator /(IntEx a, IntEx b)
        {
            if (b == 0)
            {
                if (a == 0 || a.IsNaN)
                    return NaN;
                if (a < 0)
                    return NegativeInfinity;
                return PositiveInfinity;
            }
            if (a.IsNaN || b.IsNaN)
                return NaN;

            if (b.IsInfinity)
                return 0;

            if (a.IsInfinity)
            {
                if (a.IsPositiveInfinity && b > 0 || a.IsNegativeInfinity && b < 0)
                    return PositiveInfinity;
                return NegativeInfinity;
            }

            return new IntEx(a.value / b.value);
        }
        public static bool operator <(IntEx a, IntEx b)
        {
            return a.CompareTo(b) == -1;
        }
        public static bool operator >(IntEx a, IntEx b)
        {
            return a.CompareTo(b) == 1;
        }

        public static bool operator <=(IntEx a, IntEx b)
        {
            return a.CompareTo(b) <= 0;
        }
        public static bool operator >=(IntEx a, IntEx b)
        {
            return a.CompareTo(b) >= 0;
        }

        public static IntEx operator ++(IntEx a)
        {
            if (a.IsNaN || a.IsInfinity)
                return a;
            return new IntEx(a.value + 1);
        }
        public static IntEx operator --(IntEx a)
        {
            if (a.IsNaN || a.IsInfinity)
                return a;
            return new IntEx(a.value - 1);
        }

        public static IntEx operator -(IntEx a)
        {
            if (a.IsNaN)
                return a;
            if (a.IsPositiveInfinity)
                return NegativeInfinity;
            if (a.IsNegativeInfinity)
                return PositiveInfinity;
            return new IntEx(-a.value);
        }
        public static IntEx operator +(IntEx a)
        {
            return a;
        }

        public static IntEx operator %(IntEx a, IntEx b)
        {
            if (a.IsNaN || b.IsNaN || a.IsInfinity || b == 0)
                return NaN;
            if (b.IsInfinity)
                return a;
            return new IntEx(a.value % b.value);
        }

        public override bool Equals(object obj)
        {
            if (obj is IntEx i)
            {
                return Equals(i);
            }

            if (obj is float f)
            {
                if (float.IsNaN(f) && IsNaN)
                    return true;
                if (IsNaN)
                    return false;
                if (float.IsNaN(f))
                    return false;
                if (float.IsPositiveInfinity(f) && IsPositiveInfinity)
                    return true;
                if (float.IsNegativeInfinity(f) && IsNegativeInfinity)
                    return true;
            }
            else if (obj is float d)
            {
                if (float.IsNaN(d) && IsNaN)
                    return true;
                if (IsNaN)
                    return false;
                if (float.IsNaN(d))
                    return false;
                if (float.IsPositiveInfinity(d) && IsPositiveInfinity)
                    return true;
                if (float.IsNegativeInfinity(d) && IsNegativeInfinity)
                    return true;
            }

            if (IsNaN || IsInfinity)
                return false;

            return value.Equals(obj);
        }


        public bool Equals(IntEx other)
        {
            if (IsNaN && other.IsNaN || IsPositiveInfinity && other.IsPositiveInfinity || IsNegativeInfinity && other.IsNegativeInfinity)
                return true;
            return this.value == other.value;
        }

        // double NaN from https://docs.microsoft.com/en-us/dotnet/api/system.double.nan?view=netframework-4.7
        //       NaN == NaN: False
        //       NaN != NaN: True
        //       NaN.Equals(NaN): True
        //       ! NaN.Equals(NaN): False
        //       IsNaN: True
        //       
        //       NaN > NaN: False
        //       NaN >= NaN: False
        //       NaN < NaN: False
        //       NaN < 100.0: False
        //       NaN <= 100.0: False
        //       NaN >= 100.0: False
        //       NaN.CompareTo(NaN): 0
        //       NaN.CompareTo(100.0): -1
        //       (100.0).CompareTo(Double.NaN): 1


        public override int GetHashCode()
        {
            if (IsPositiveInfinity)
                return PositiveInfinity.value.GetHashCode();
            if (IsNegativeInfinity)
                return NegativeInfinity.value.GetHashCode();
            return value.GetHashCode();
        }

        public static bool operator ==(IntEx a, IntEx b)
        {
            if (a.IsNaN || b.IsNaN)
                return false;

            if (a.IsInfinity || b.IsInfinity)
                return a.IsPositiveInfinity == b.IsPositiveInfinity;
            return a.value.Equals(b.value);
        }

        public static bool operator !=(IntEx a, IntEx b)
        {
            return !(a == b);
        }

        public override string ToString()
        {
            if (IsPositiveInfinity)
                return double.PositiveInfinity.ToString();
            if (IsNegativeInfinity)
                return double.NegativeInfinity.ToString();
            if (IsNaN)
                return double.NaN.ToString();
            return value.ToString();
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (IsPositiveInfinity)
                return double.PositiveInfinity.ToString(format, formatProvider);
            if (IsNegativeInfinity)
                return double.NegativeInfinity.ToString(format, formatProvider);
            if (IsNaN)
                return double.NaN.ToString(format, formatProvider);

            return value.ToString(format, formatProvider);
        }

        public int CompareTo(IntEx other)
        {
            if (IsNaN && other.IsNaN)
                return 0;
            if (IsNaN)
                return -1;
            if (other.IsNaN)
                return 1;

            return value.CompareTo(other.value);
        }

        public int CompareTo(object obj)
        {
            if (obj is IntEx other)
                return CompareTo(other);

            if (obj is double d)
            {
                if (double.IsNaN(d) && IsNaN)
                    return 0;
                if (IsNaN)
                    return -1;
                if (double.IsNaN(d))
                    return 1;
                if (double.IsPositiveInfinity(d) && IsPositiveInfinity)
                    return 0;
                if (double.IsNegativeInfinity(d) && IsNegativeInfinity)
                    return 0;

            }
            else if (obj is float f)
            {
                if (float.IsNaN(f) && IsNaN)
                    return 0;
                if (IsNaN)
                    return -1;
                if (float.IsNaN(f))
                    return 1;
                if (float.IsPositiveInfinity(f) && IsPositiveInfinity)
                    return 0;
                if (float.IsNegativeInfinity(f) && IsNegativeInfinity)
                    return 0;
            }

            else if (IsNaN)
                return -1;

            var comparedValue = (value as IComparable).CompareTo(obj);
            if (comparedValue == -1 && IsPositiveInfinity)
                return 1;
            if (comparedValue == 1 && IsNegativeInfinity)
                return -1;
            return comparedValue;
        }

        public static implicit operator IntEx(int i)
        {
            return new IntEx(i);
        }

        public static explicit operator int(IntEx i)
        {
            if (i.IsInfinity || i.IsNaN)
                throw new ArithmeticException($"{nameof(IntEx)} was { i.ToString() }");
            return (int)i.value;
        }
        public static explicit operator long(IntEx i)
        {
            if (i.IsInfinity || i.IsNaN)
                throw new ArithmeticException($"{nameof(IntEx)} was { i.ToString() }");
            return (int)i.value;
        }
        public static implicit operator float(IntEx i)
        {
            if (i.IsPositiveInfinity)
                return float.PositiveInfinity;
            if (i.IsNaN)
                return float.NaN;
            if (i.IsNegativeInfinity)
                return float.NegativeInfinity;
            return i.value;
        }
        public static implicit operator double(IntEx i)
        {
            if (i.IsPositiveInfinity)
                return float.PositiveInfinity;
            if (i.IsNaN)
                return float.NaN;
            if (i.IsNegativeInfinity)
                return float.NegativeInfinity;
            return i.value;
        }

        public static explicit operator IntEx(double i)
        {
            if (i == double.PositiveInfinity)
                return PositiveInfinity;
            if (double.IsNaN(i))
                return NaN;
            if (i == double.NegativeInfinity)
                return NegativeInfinity;
            return new IntEx((int)i);
        }
        public static explicit operator IntEx(float i)
        {
            if (i == float.PositiveInfinity)
                return PositiveInfinity;
            if (float.IsNaN(i))
                return NaN;
            if (i == float.NegativeInfinity)
                return NegativeInfinity;
            return new IntEx((int)i);
        }
    }

    public static class MathEx
    {
        //
        // Zusammenfassung:
        //     Berechnet die Summe einer Sequenz von System.Int64-Werten.
        //
        // Parameter:
        //   source:
        //     Eine Sequenz von System.Int64-Werten, deren Summe berechnet werden soll.
        //
        // Rückgabewerte:
        //     Die Summe der Werte in der Sequenz.
        //
        // Ausnahmen:
        //   T:System.ArgumentNullException:
        //     source ist null.
        //
        //   T:System.OverflowException:
        //     Die Summe ist größer als System.Int64.MaxValue.
        public static IntEx Sum(this IEnumerable<IntEx> source)
        {
            IntEx i = 0;
            foreach (var j in source)
                i += j;
            return i;
        }


        public static T Min<T>(params T[] a) where T : IComparable
        {
            return a.Min();
        }
        public static T Min<T>(T a, T b) where T : IComparable
        {
            var compare = a.CompareTo(b);
            if (compare > 0)
                return b;
            return a;
        }
        public static T Max<T>(params T[] a) where T : IComparable
        {
            return a.Max();
        }
        public static T Max<T>(T a, T b) where T : IComparable
        {
            var compare = a.CompareTo(b);
            if (compare < 0)
                return b;
            return a;
        }
        public static T Clamp<T>(T min, T max, T value) where T : IComparable
        {
            if (min.CompareTo(max) < 0)
                throw new ArgumentException("min must be less or equals to max");

            return Min(Max(min, value), max);
        }
    }
}

