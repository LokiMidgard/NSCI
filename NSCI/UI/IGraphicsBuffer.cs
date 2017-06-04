using System;
using NSCI.Widgets;

namespace NSCI.UI
{
    public interface IRenderFrame
    {
        ColoredKey this[int x, int y] { get; set; }

        int Width { get; }
        int Height { get; }

        Rect? Clip { get; }

        IRenderFrame GetGraphicsBuffer(Rect? translation = default(Rect?), Rect? clip = default(Rect?));
    }
    public struct ColoredKey
    {
        public readonly ConsoleColor background;
        public readonly ConsoleColor forground;
        public readonly char Character;

        public ColoredKey(char character, ConsoleColor forground, ConsoleColor background)
        {
            if (char.IsControl(character))
                character = ' ';
            this.Character = character;
            this.forground = forground;
            this.background = background;
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj is ColoredKey other)
                return Equals(other);
            return false;
        }

        private bool Equals(ColoredKey other)
        {
            return other.Character == this.Character && other.forground == this.forground && other.background == this.background;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            var hash = 13;
            hash += (hash * 31) ^ (int)this.background;
            hash += (hash * 31) ^ (int)this.forground;
            hash += (hash * 31) ^ this.Character;
            return hash;
        }

        public static bool operator ==(ColoredKey k1, ColoredKey k2) => k1.Equals(k2);
        public static bool operator !=(ColoredKey k1, ColoredKey k2) => !k1.Equals(k2);
    }


}