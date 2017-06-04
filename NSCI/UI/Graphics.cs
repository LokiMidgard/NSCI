using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NSCI.Widgets;
using Console = System.Console;

namespace NSCI.UI
{
    public class Graphics
    {
        private readonly RootWindow rootwindow;

        private readonly Buffer currentBuffer;
        private readonly Buffer previousBuffer;
        private bool previousBufferValid;

        internal Graphics(RootWindow rootwindow)
        {
            this.rootwindow = rootwindow;
            Width = Console.WindowWidth;
            Height = Console.WindowHeight - 1;
            this.currentBuffer = new Buffer(Width, Height);
            this.previousBuffer = new Buffer(Width, Height);
            this.previousBufferValid = false;

            Console.SetCursorPosition(0, 0); // Reset Curso so we can set buffer
            Console.BufferHeight = Height + 1;
            Console.BufferWidth = Width;

        }
        private class BufferWraper : IRenderFrame
        {
            private readonly IRenderFrame parent;
            private readonly Rect translation;
            public Rect? Clip { get; }

            public BufferWraper(IRenderFrame parent, Rect? translation = default(Rect?), Rect? clip = default(Rect?))
            {
                this.parent = parent;
                this.translation = translation ?? new Rect(0, 0, parent.Width, parent.Height);
                Clip = clip;

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

                    if (Clip.HasValue && (x < Clip.Value.Left || x > Clip.Value.Right || y < Clip.Value.Top || y > Clip.Value.Bottom))
                        return; // we doe nothing out of clipping

                    x += this.translation.X;
                    y += this.translation.Y;
                    this.parent[x, y] = value;
                }
            }

            public int Width => this.translation.Width;

            public int Height => this.translation.Height;


            /// <summary>
            /// Returns a Reduced Renderframe
            /// </summary>
            /// <param name="translation">the Translation in Parentscordiants.</param>
            /// <param name="clip">The Clip in Parents coordinates</param>
            /// <returns></returns>
            public IRenderFrame GetGraphicsBuffer(Rect? translation = default(Rect?), Rect? clip = default(Rect?))
            {
                if (clip.HasValue && translation.HasValue)
                    clip = new Rect(clip.Value.Left + translation.Value.Left, clip.Value.Top + translation.Value.Top, clip.Value.Width - translation.Value.Left, clip.Value.Height - translation.Value.Top);
                return new BufferWraper(this, translation, clip);
            }

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

            public Rect? Clip => null;

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


        internal async void Resize(int counter = 0)
        {
            try
            {
                Width = Console.WindowWidth;
                Height = Console.WindowHeight - 1;
                currentBuffer.Resize(Width, Height);
                previousBuffer.Resize(Width, Height);
                this.previousBufferValid = false;

                Console.SetCursorPosition(0, 0); // Reset Curso so we can set buffer
                Console.BufferHeight = Height + 1;
                Console.BufferWidth = Width;

            }
            catch (Exception)
            {
                if (counter < 3)
                {
                    await Task.Delay(200);
                    Resize(counter + 1);
                }
                else
                    throw;
            }
        }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public IRenderFrame GraphicsBuffer => this.currentBuffer;


        internal void Draw()
        {
            if (this.previousBufferValid)
            {

                for (int i = 0; i < this.currentBuffer.Length; i++)
                {
                    if (this.currentBuffer[i] == this.previousBuffer[i])
                        continue;
                    var (left, top) = GetXYFromIndex(i);
                    Console.SetCursorPosition(left, top);

                    Console.ForegroundColor = this.currentBuffer.ForgroundBuffer[i];
                    Console.BackgroundColor = this.currentBuffer.BackgroundBuffer[i];
                    int j = 0;
                    while (true)
                    {
                        int k = 0;
                        while (i + j < this.currentBuffer.Length
                            && this.currentBuffer.ForgroundBuffer[i + j] == this.currentBuffer.ForgroundBuffer[i]
                            && this.currentBuffer.BackgroundBuffer[i + j] == this.currentBuffer.BackgroundBuffer[i]
                            && this.currentBuffer[i + j] != this.previousBuffer[i + j])
                            ++j;
                        while (i + j + k < this.currentBuffer.Length
                            && this.currentBuffer.ForgroundBuffer[i + j + k] == this.currentBuffer.ForgroundBuffer[i]
                            && this.currentBuffer.BackgroundBuffer[i + j + k] == this.currentBuffer.BackgroundBuffer[i]
                            && this.currentBuffer[i + j + k] == this.previousBuffer[i + j + k])
                            ++k;

                        if (i + j + k < this.currentBuffer.Length
                            && this.currentBuffer.ForgroundBuffer[i + j + k] == this.currentBuffer.ForgroundBuffer[i]
                            && this.currentBuffer.BackgroundBuffer[i + j + k] == this.currentBuffer.BackgroundBuffer[i])
                            j += k;
                        else
                            break;
                    }

                    Console.Write(this.currentBuffer.CharacterBuffer, i, j);
                    i += j;
                }
            }
            else
            {

                Console.SetCursorPosition(0, 0);
                for (int i = 0, j = 0; i < this.currentBuffer.Length; i += j)
                {

                    Console.ForegroundColor = this.currentBuffer.ForgroundBuffer[i];
                    Console.BackgroundColor = this.currentBuffer.BackgroundBuffer[i];
                    j = 1;
                    while (i + j < this.currentBuffer.Length
                        && this.currentBuffer.ForgroundBuffer[i + j] == this.currentBuffer.ForgroundBuffer[i]
                        && this.currentBuffer.BackgroundBuffer[i + j] == this.currentBuffer.BackgroundBuffer[i])
                        ++j;

                    Console.Write(this.currentBuffer.CharacterBuffer, i, j);
                    ;
                }

            }

            this.previousBuffer.CopyFrom(this.currentBuffer);
        }

        private (int x, int y) GetXYFromIndex(int i) => (i % Width, i / Width);
    }
}