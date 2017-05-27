using System;
using System.Collections.Generic;
using System.Linq;
using NSCI.Widgets;
using Console = System.Console;

namespace NSCI.UI
{
    public class Graphics
    {
        private readonly RootWindow rootwindow;

        private Buffer backBuffer;
        private Buffer forBuffer;

        private class Buffer
        {
            public Buffer(int size)
            {
                this.buffer = new char[size];
                this.background = new ConsoleColor[size];
                this.forground = new ConsoleColor[size];
                for (int i = 0; i < forground.Length; i++)
                {
                    forground[i] = ConsoleColor.Red;
                }
            }
            public ColoredKey this[int index]
            {
                get => new ColoredKey(buffer[index], forground[index], background[index]);
                set
                {
                    this.buffer[index] = value.Character;
                    this.background[index] = value.background;
                    this.forground[index] = value.forground;
                }
            }
            private readonly char[] buffer;
            private readonly ConsoleColor[] background;
            private readonly ConsoleColor[] forground;

            public char[] CharacterBuffer => this.buffer;
            public ConsoleColor[] ForgroundBuffer => this.forground;
            public ConsoleColor[] BackgroundBuffer => this.background;

            public int Length => this.buffer.Length;

            internal void CopyFrom(Buffer backBuffer)
            {
                Array.Copy(backBuffer.buffer, this.buffer, this.buffer.Length);
                Array.Copy(backBuffer.forground, this.forground, this.buffer.Length);
                Array.Copy(backBuffer.background, this.background, this.buffer.Length);
            }
        }


        private struct ColoredKey
        {
            public readonly ConsoleColor background;
            public readonly ConsoleColor forground;
            public readonly char Character;

            public ColoredKey(char character, ConsoleColor forground, ConsoleColor background)
            {
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
                hash = (hash * 31) ^ (int)this.background;
                hash = (hash * 31) ^ (int)this.forground;
                hash = (hash * 31) ^ this.Character;
                return hash;
            }

            public static bool operator ==(ColoredKey k1, ColoredKey k2) => k1.Equals(k2);
            public static bool operator !=(ColoredKey k1, ColoredKey k2) => !k1.Equals(k2);
        }

        internal Graphics(RootWindow rootwindow)
        {
            this.rootwindow = rootwindow;
            Width = Console.WindowWidth;
            Height = Console.WindowHeight - 1;
            this.backBuffer = new Buffer(Width * Height);
            this.forBuffer = null;

            Console.BufferHeight = Height + 1;
            Console.BufferWidth = Width;

        }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public void DrawRect(int xPos, int yPos, int width, int height, ConsoleColor forground, ConsoleColor background, SpecialChars c)
        {

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    this.backBuffer[GetBufferIndex(x + xPos, y + yPos)] = new ColoredKey((char)c, forground, background);
        }

        public void DrawLine(Pen pen, ConsoleColor forground, ConsoleColor background, IEnumerable<(int x, int y)> points)
        {
            IList<(int x, int y)> list;
            if (points is IList<(int x, int y)>)
                list = points as IList<(int x, int y)>;
            else
                list = points.ToArray();
            bool lastWasHorizontal = false;
            for (int i = 0; i < list.Count - 1; i++)
            {

                bool horisontal;
                if (list[i].y == list[i + 1].y)
                    horisontal = true;
                else if (list[i].x == list[i + 1].x)
                    horisontal = false;
                else
                    throw new ArgumentException("Lines mus be horizontal or vertical", "points");

                if (horisontal)
                {
                    // check if we need to add a curve
                    if (i > 0 && !lastWasHorizontal)
                    {
                        // we need to add a curve, up or down
                        if (list[i].x < list[i + 1].x)
                        {
                            if (list[i - 1].y < list[i].y)
                                this.backBuffer[GetBufferIndex(list[i].x, list[i].y)] = new ColoredKey(pen.bottomLeft, forground, background);
                            else
                                this.backBuffer[GetBufferIndex(list[i].x, list[i].y)] = new ColoredKey(pen.topLeft, forground, background);
                        }
                        else
                        {
                            if (list[i - 1].y < list[i].y)
                                this.backBuffer[GetBufferIndex(list[i].x, list[i].y)] = new ColoredKey(pen.bottemRight, forground, background);
                            else
                                this.backBuffer[GetBufferIndex(list[i].x, list[i].y)] = new ColoredKey(pen.topRight, forground, background);
                        }
                    }
                    else
                        this.backBuffer[GetBufferIndex(list[i].x, list[i].y)] = new ColoredKey(pen.horizontal, forground, background);

                    // draw the line
                    var min = Math.Min(list[i].x, list[i + 1].x) + 1;
                    var max = Math.Max(list[i].x, list[i + 1].x);

                    for (int x = min; x < max; x++)
                        this.backBuffer[GetBufferIndex(x, list[i].y)] = new ColoredKey(pen.horizontal, forground, background);

                    // check if we need to draw the last element or if there is a next segment that will draw this
                    if (i == list.Count - 2)
                        this.backBuffer[GetBufferIndex(list[i + 1].x, list[i + 1].y)] = new ColoredKey(pen.horizontal, forground, background);
                }
                else
                {
                    // check if we need to add a curve
                    if (i > 0 && lastWasHorizontal)
                    {
                            // we need to add a curve, up or down
                        if (list[i].y < list[i + 1].y)
                        {
                            if (list[i - 1].x < list[i].x)
                                this.backBuffer[GetBufferIndex(list[i].x, list[i].y)] = new ColoredKey(pen.topRight, forground, background);
                            else
                                this.backBuffer[GetBufferIndex(list[i].x, list[i].y)] = new ColoredKey(pen.topLeft, forground, background);
                        }
                        else
                        {
                            if (list[i - 1].x < list[i].x)
                            this.backBuffer[GetBufferIndex(list[i].x, list[i].y)] = new ColoredKey(pen.bottemRight, forground, background);
                        else
                            this.backBuffer[GetBufferIndex(list[i].x, list[i].y)] = new ColoredKey(pen.bottomLeft, forground, background);
                        }
                    }
                    else
                        this.backBuffer[GetBufferIndex(list[i].x, list[i].y)] = new ColoredKey(pen.vertical, forground, background);

                    // draw the line
                    var min = Math.Min(list[i].y, list[i + 1].y) + 1;
                    var max = Math.Max(list[i].y, list[i + 1].y);

                    for (int y = min; y < max; y++)
                        this.backBuffer[GetBufferIndex(list[i].x, y)] = new ColoredKey(pen.vertical, forground, background);

                    // check if we need to draw the last element or if there is a next segment that will draw this
                    if (i == list.Count - 2)
                        this.backBuffer[GetBufferIndex(list[i + 1].x, list[i + 1].y)] = new ColoredKey(pen.vertical, forground, background);
                }
                lastWasHorizontal = horisontal;
            }
        }
        public void DrawLine(Pen pen, ConsoleColor forground, ConsoleColor background, params (int x, int y)[] points) => DrawLine(pen, forground, background, points as IEnumerable<(int x, int y)>);

        private int GetBufferIndex(int x, int y)
        {
            return x + y * Width;
        }



        internal void Draw()
        {
            if (this.forBuffer != null)
            {

                for (int i = 0; i < this.backBuffer.Length; i++)
                {
                    if (this.backBuffer[i] == this.forBuffer[i])
                        continue;
                    var (left, top) = GetXYFromIndex(i);
                    Console.SetCursorPosition(left, top);

                    Console.ForegroundColor = this.backBuffer[i].forground;
                    Console.BackgroundColor = this.backBuffer[i].background;
                    int j = 0;
                    while (true)
                    {
                        int k = 0;
                        while (i + j < this.backBuffer.Length
                            && this.backBuffer.ForgroundBuffer[i + j] == this.backBuffer.ForgroundBuffer[i]
                            && this.backBuffer.BackgroundBuffer[i + j] == this.backBuffer.BackgroundBuffer[i]
                            && this.backBuffer[i + j] != this.forBuffer[i + j])
                            ++j;
                        while (i + j + k < this.backBuffer.Length
                            && this.backBuffer.ForgroundBuffer[i + j + k] == this.backBuffer.ForgroundBuffer[i]
                            && this.backBuffer.BackgroundBuffer[i + j + k] == this.backBuffer.BackgroundBuffer[i]
                            && this.backBuffer[i + j + k] == this.forBuffer[i + j + k])
                            ++k;

                        if (i + j + k < this.backBuffer.Length
                            && this.backBuffer.ForgroundBuffer[i + j + k] == this.backBuffer.ForgroundBuffer[i]
                            && this.backBuffer.BackgroundBuffer[i + j + k] == this.backBuffer.BackgroundBuffer[i])
                            j += k;
                        else
                            break;
                    }

                    Console.Write(this.backBuffer.CharacterBuffer, i, j);
                    i += j;
                }
            }
            else
            {

                Console.SetCursorPosition(0, 0);
                for (int i = 0; i < this.backBuffer.Length; i++)
                {

                    Console.ForegroundColor = this.backBuffer[i].forground;
                    Console.BackgroundColor = this.backBuffer[i].background;
                    int j = 0;
                    while (i + j < this.backBuffer.Length
                        && this.backBuffer.ForgroundBuffer[i + j] == this.backBuffer.ForgroundBuffer[i]
                        && this.backBuffer.BackgroundBuffer[i + j] == this.backBuffer.BackgroundBuffer[i])
                        ++j;

                    Console.Write(this.backBuffer.CharacterBuffer, i, j);
                    i += j;
                }

            }

            if (this.forBuffer == null)
                this.forBuffer = new Buffer(this.backBuffer.Length);
            this.forBuffer.CopyFrom(this.backBuffer);
        }

        private (int x, int y) GetXYFromIndex(int i) => (i % Width, i / Width);
    }

    public enum SpecialChars : ushort
    {
        Shade = '▒',
        Fill = ' '
    }
}