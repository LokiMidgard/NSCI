using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Height = Console.WindowHeight;
            this.currentBuffer = new Buffer(Width, Height);
            this.previousBuffer = new Buffer(Width, Height);
            this.previousBufferValid = false;

            Console.SetCursorPosition(0, 0); // Reset Curso so we can set buffer
            Console.BufferHeight = Height;
            Console.BufferWidth = Width;

        }
        private class BufferWraper : IRenderFrame
        {
            private readonly IRenderFrame parent;
            private readonly Rect translation;
            public Rect Clip { get; }

            public BufferWraper(IRenderFrame parent, Rect? translation = default, Rect? clip = default)
            {
                this.parent = parent;
                this.translation = translation ?? new Rect(0, 0, parent.Width, parent.Height);
                Clip = clip ?? parent.Clip.Translate(new Size(-this.translation.Location.X, -this.translation.Location.Y));

                Clip = new Rect(
                    x: MathEx.Clamp(0, this.translation.Width, Clip.X),
                    y: MathEx.Clamp(0, this.translation.Height, Clip.Y),
                    width: MathEx.Clamp(0, this.translation.Width - MathEx.Clamp(0, this.translation.Width, Clip.X), Clip.Width),
                    height: MathEx.Clamp(0, this.translation.Height - MathEx.Clamp(0, this.translation.Height, Clip.Y), Clip.Height));

                if (Clip.Width == 0 || Clip.Height == 0)
                    Clip = Rect.Empty;

                var test = clip?.Translate((Size)this.translation.Location) ?? this.translation;
                if (test.Right > parent.Width || test.Bottom > parent.Height || test.Left < 0 || test.Top < 0)
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

                    x += (int)this.translation.X;
                    y += (int)this.translation.Y;
                    return this.parent[x, y];
                }
                set
                {
                    if (x < Clip.Left || x >= Clip.Right || y < Clip.Top || y >= Clip.Bottom)
                        return; // we doe nothing out of clipping

                    if (x < 0 || x > this.translation.Width)
                        throw new ArgumentOutOfRangeException(nameof(x));
                    if (y < 0 || y > this.translation.Height)
                        throw new ArgumentOutOfRangeException(nameof(y));

                    x += (int)this.translation.X;
                    y += (int)this.translation.Y;

                    this.parent[x, y] = value;
                }
            }

            public int Width => (int)this.translation.Width;

            public int Height => (int)this.translation.Height;


            /// <summary>
            /// Returns a Reduced Renderframe
            /// </summary>
            /// <param name="translation">the Translation in Parentscordiants.</param>
            /// <param name="clip">The Clip in Parents coordinates</param>
            /// <returns></returns>
            public IRenderFrame GetGraphicsBuffer(Rect? translation = default, Rect? clip = default)
            {
                if (clip.HasValue && translation.HasValue)
                    clip = clip.Value.Translate(new Size(-translation.Value.X, -translation.Value.Y));// new Rect(clip.Value.Left + translation.Value.Left, clip.Value.Top + translation.Value.Top, clip.Value.Width - translation.Value.Left, clip.Value.Height - translation.Value.Top);
                //clip = new Rect(clip.Value.Left + translation.Value.Left, clip.Value.Top + translation.Value.Top, clip.Value.Width - translation.Value.Left, clip.Value.Height - translation.Value.Top);
                return new BufferWraper(this, translation, clip);
            }

        }

        [System.Diagnostics.DebuggerDisplay("{Debug}")]
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
                get => new ColoredKey(this.buffer[index], this.forground[index], this.background[index]);
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

            public int Length => this.buffer.Length - 1;

            public int Width { get; private set; }

            public int Height { get; private set; }

            public Rect Clip => new Rect(0, 0, Width, Height);

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
                Array.Resize(ref this.buffer, size);
                Array.Resize(ref this.background, size);
                Array.Resize(ref this.forground, size);
            }

            public String Debug
            {
                get
                {
                    var builder = new StringBuilder();
                    for (int i = 0; i < Height; i++)
                    {
                        builder.AppendLine(new string(this.buffer, Width * i, Width));
                    }
                    return builder.ToString();
                }
            }

        }


        internal async void Resize(int counter = 0)
        {
            try
            {
                Width = Console.WindowWidth;
                Height = Console.WindowHeight - 1;
                this.currentBuffer.Resize(Width, Height);
                this.previousBuffer.Resize(Width, Height);
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
            do
            {
                try
                {
                    Console.CursorVisible = false;
                    Console.SetCursorPosition(0, 0);
                    if (this.previousBufferValid)
                    {
                        for (int i = 0; i < this.currentBuffer.Length; i++)
                        {
                            if (this.currentBuffer[i] == this.previousBuffer[i])
                                continue;

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
                                {
                                    //if (i + j >= this.currentBuffer.Length)
                                    //    j -= i + j - this.currentBuffer.Length + 1;
                                    break;
                                }
                            }
                            System.Diagnostics.Debug.Assert(i + j < this.currentBuffer.CharacterBuffer.Length, "We try to write over our buffer :(");

                            var (left, top) = GetXYFromIndex(i);
                            Console.SetCursorPosition(left, top);
                            Console.Write(this.currentBuffer.CharacterBuffer, i, j);
                            i += j - 1;
                        }
                    }
                    else
                    {
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
                        }

                        Console.SetCursorPosition(0, 0);
                    }

                    Console.ResetColor(); // We do not want to have spooky colors
                    this.previousBuffer.CopyFrom(this.currentBuffer);
                    this.previousBufferValid = true;
                    if (this.rootwindow.RootCurserPosition.HasValue)
                    {
                        var value = this.rootwindow.RootCurserPosition.Value;
                        value = this.rootwindow.GetLocation(this.rootwindow.ActiveControl).Location + (Size)value;

                        Console.SetCursorPosition((int)value.X, (int)value.Y);
                        Console.CursorVisible = true;
                    }
                    else
                        Console.SetCursorPosition(0, 0);
                }
                catch (ArgumentOutOfRangeException)
                {
                    // when resizing the window we get problems with the buffer. 
                    // We need to try again and again until the resize is finished.
                    this.previousBufferValid = false;
                }
            } while (!this.previousBufferValid);

        }

        private (int x, int y) GetXYFromIndex(int i) => (i % Width, i / Width);
    }
}