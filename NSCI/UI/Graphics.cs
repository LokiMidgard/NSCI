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

        private readonly Buffer backBuffer;
        private readonly Buffer forBuffer;
        private bool forBufferValid;

        internal Graphics(RootWindow rootwindow)
        {
            this.rootwindow = rootwindow;
            Width = Console.WindowWidth;
            Height = Console.WindowHeight - 1;
            this.backBuffer = new Buffer(Width, Height);
            this.forBuffer = new Buffer(Width, Height);
            forBufferValid = false;

            Console.SetCursorPosition(0, 0); // Reset Curso so we can set buffer
            Console.BufferHeight = Height + 1;
            Console.BufferWidth = Width;

        }
        private class BufferWraper : IRenderFrame
        {
            private readonly IRenderFrame parent;
            private readonly Rect translation;
            private readonly Rect? clip;

            public BufferWraper(IRenderFrame parent, Rect? translation = default(Rect?), Rect? clip = default(Rect?))
            {
                this.parent = parent;
                this.translation = translation ?? new Rect(0, 0, parent.Width, parent.Height);
                this.clip = clip;

                if (this.translation.Right > parent.Width || this.translation.Bottom > parent.Height || this.translation.Left < 0 || this.translation.Top < 0)
                    throw new ArgumentOutOfRangeException(nameof(translation), $"Translation must be insied the width and height of the parent. Width={parent.Width},Height={parent.Height}. Translation={this.translation}");
            }

            public ColoredKey this[int x, int y]
            {
                get
                {
                    if (x < 0 || x > this.translation.Width)
                        throw new ArgumentOutOfRangeException(nameof(x));
                    if (y < 0 || y > this.translation.Height)
                        throw new ArgumentOutOfRangeException(nameof(y));

                    x += this.translation.X;
                    y += this.translation.Y;
                    return this.parent[x, y];
                }
                set
                {
                    if (x < 0 || x > this.translation.Width)
                        throw new ArgumentOutOfRangeException(nameof(x));
                    if (y < 0 || y > this.translation.Height)
                        throw new ArgumentOutOfRangeException(nameof(y));

                    if (this.clip.HasValue && (x < this.clip.Value.Left || x > this.clip.Value.Right || y < this.clip.Value.Top || y > this.clip.Value.Bottom))
                        return; // we doe nothing out of clipping

                    x += this.translation.X;
                    y += this.translation.Y;
                    this.parent[x, y] = value;
                }
            }

            public int Width => this.translation.Width;

            public int Height => this.translation.Height;

            public IRenderFrame GetGraphicsBuffer(Rect? translation = default(Rect?), Rect? clip = default(Rect?)) => new BufferWraper(this, translation, clip);

        }

        private class Buffer : IRenderFrame
        {
            public Buffer(int width, int height)
            {
                var size = width * height;
                Width = width;
                Height = height;
                this.buffer = new char[size];
                this.background = new ConsoleColor[size];
                this.forground = new ConsoleColor[size];
                for (int i = 0; i < this.forground.Length; i++)
                {
                    this.forground[i] = ConsoleColor.Red;
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
            private char[] buffer;
            private ConsoleColor[] background;
            private ConsoleColor[] forground;


            public char[] CharacterBuffer => this.buffer;
            public ConsoleColor[] ForgroundBuffer => this.forground;
            public ConsoleColor[] BackgroundBuffer => this.background;

            public int Length => this.buffer.Length;

            public int Width { get; private set; }

            public int Height { get; private set; }

            public ColoredKey this[int x, int y] { get => this[GetBufferIndex(x, y)]; set => this[GetBufferIndex(x, y)] = value; }

            private int GetBufferIndex(int x, int y) => x + y * Width;

            internal void CopyFrom(Buffer backBuffer)
            {
                Array.Copy(backBuffer.buffer, this.buffer, this.buffer.Length);
                Array.Copy(backBuffer.forground, this.forground, this.buffer.Length);
                Array.Copy(backBuffer.background, this.background, this.buffer.Length);
            }
            public IRenderFrame GetGraphicsBuffer(Rect? translation = default(Rect?), Rect? clip = default(Rect?)) => new BufferWraper(this, translation, clip);

            internal void Resize(int width, int height)
            {
                var size = width * height;
                Width = width;
                Height = height;
                Array.Resize(ref buffer, size);
                Array.Resize(ref background, size);
                Array.Resize(ref forground, size);
            }
        }


        internal void Resize()
        {
            Width = Console.WindowWidth;
            Height = Console.WindowHeight - 1;
            backBuffer.Resize(Width, Height);
            forBuffer.Resize(Width, Height);

            Console.SetCursorPosition(0, 0); // Reset Curso so we can set buffer
            Console.BufferHeight = Height + 1;
            Console.BufferWidth = Width;
        }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public IRenderFrame GraphicsBuffer => this.backBuffer;


        internal void Draw()
        {
            if (this.forBufferValid)
            {

                for (int i = 0; i < this.backBuffer.Length; i++)
                {
                    if (this.backBuffer[i] == this.forBuffer[i])
                        continue;
                    var (left, top) = GetXYFromIndex(i);
                    Console.SetCursorPosition(left, top);

                    Console.ForegroundColor = this.backBuffer.ForgroundBuffer[i];
                    Console.BackgroundColor = this.backBuffer.BackgroundBuffer[i];
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
                for (int i = 0, j = 0; i < this.backBuffer.Length; i += j)
                {

                    Console.ForegroundColor = this.backBuffer.ForgroundBuffer[i];
                    Console.BackgroundColor = this.backBuffer.BackgroundBuffer[i];
                    j = 1;
                    while (i + j < this.backBuffer.Length
                        && this.backBuffer.ForgroundBuffer[i + j] == this.backBuffer.ForgroundBuffer[i]
                        && this.backBuffer.BackgroundBuffer[i + j] == this.backBuffer.BackgroundBuffer[i])
                        ++j;

                    Console.Write(this.backBuffer.CharacterBuffer, i, j);
                    ;
                }

            }

            this.forBuffer.CopyFrom(this.backBuffer);
        }

        private (int x, int y) GetXYFromIndex(int i) => (i % Width, i / Width);
    }
}