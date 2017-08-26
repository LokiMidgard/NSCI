using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSCI
{
    internal static class ConsoleHelper
    {


        internal static int BinarySearch<T>(this IList<T> list, T search) where T : IComparable<T>
        {
            var low = 0;
            var high = list.Count - 1;

            while (low <= high)
            {
                var middle = low + ((high - low) >> 1);
                var midValue = list[middle];

                var comparison = search.CompareTo(midValue);
                if (comparison == 0)
                    return middle;

                if (comparison < 0)
                    high = middle - 1;
                else
                    low = middle + 1;
            }

            return ~low;
        }
        internal static int BinarySearch<T>(this IList<T> list, T search, IComparer<T> comparer)
        {
            var low = 0;
            var high = list.Count - 1;

            while (low <= high)
            {
                var middle = low + ((high - low) >> 1);
                var midValue = list[middle];

                var comparison = comparer.Compare(search, midValue);
                if (comparison == 0)
                    return middle;

                if (comparison < 0)
                    high = middle - 1;
                else
                    low = middle + 1;
            }

            return ~low;
        }

        public static IEnumerable<T> ConsumableEnumerator<T>(this IList<T> queue)
        {
            while (queue.Count != 0)
            {
                var toRemove = queue[0];
                queue.RemoveAt(0);
                yield return toRemove;
            }
        }
        public static IEnumerable<T> ConsumableEnumerator<T>(this Queue<T> queue)
        {
            while (queue.Count != 0)
                yield return queue.Dequeue();
        }
        public static IEnumerable<T> ConsumableEnumerator<T>(this Stack<T> queue)
        {
            while (queue.Count != 0)
                yield return queue.Pop();
        }

        internal static void ResetConsoleWindow()
        {
            Console.SetWindowPosition(0, 0);
            Console.SetCursorPosition(0, 0);
        }

        internal static void DrawText(int x, int y, ConsoleColor fg, ConsoleColor bg, string format, params object[] args)
        {
            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = fg;
            Console.BackgroundColor = bg;
            Console.Write(format, args);
        }

        internal static void DrawText(int x, int y, ConsoleColor fg, string format, params object[] args)
        {
            Console.SetCursorPosition(x, y);
            Console.ForegroundColor = fg;
            Console.Write(format, args);
        }

        internal static void DrawRectShade(int x, int y, int w, int h, ConsoleColor bg, ConsoleColor fg, char ch)
        {
            Console.BackgroundColor = bg;
            Console.ForegroundColor = fg;

            var l = new string(ch, w);

            for (var i = 0; i < h - 1; i++)
            {
                Console.SetCursorPosition(x + w - 1, y + i);
                Console.Write(ch);
            }
            Console.SetCursorPosition(x, y + h - 1);
            Console.Write(l);

            ResetConsoleWindow();
        }

        internal delegate void DrawBoxMethod(int x, int y, int w, int h, ConsoleColor c);

        internal static void DrawNothing(int x, int y, int w, int h, ConsoleColor c) { }

        internal static void DrawRectSolid(int x, int y, int w, int h, ConsoleColor c)
        {
            Console.BackgroundColor = c;
            var l = new string(' ', w);
            for (var i = 0; i < h; i++)
            {
                Console.SetCursorPosition(x, y + i);
                Console.Write(l);
            }
            ResetConsoleWindow();
        }

        internal static void DrawBlockOutline(int x, int y, int w, int h, ConsoleColor c)
        {
            Console.ForegroundColor = c;

            Console.SetCursorPosition(x, y);
            Console.Write("▄");
            Console.Write(new string('█', w - 1));
            Console.Write("▄");

            for (int i = 1; i < h - 1; i++)
            {
                Console.SetCursorPosition(x, y + i);
                Console.Write("█");
                Console.SetCursorPosition(x + w, y + i);
                Console.Write("█");
            }

            Console.SetCursorPosition(x, y + h - 1);
            Console.Write("▀");
            Console.Write(new string('█', w - 1));
            Console.Write("▀");
        }

        internal static void DrawSingleOutline(int x, int y, int w, int h, ConsoleColor c)
        {
            Console.ForegroundColor = c;

            Console.SetCursorPosition(x, y);
            Console.Write("┌");
            Console.Write(new string('─', w - 1));
            Console.Write("┐");

            for (int i = 1; i < h - 1; i++)
            {
                Console.SetCursorPosition(x, y + i);
                Console.Write("│");
                Console.SetCursorPosition(x + w, y + i);
                Console.Write("│");
            }

            Console.SetCursorPosition(x, y + h - 1);
            Console.Write("└");
            Console.Write(new string('─', w - 1));
            Console.Write("┘");
        }

        internal static void DrawDoubleOutline(int x, int y, int w, int h, ConsoleColor c)
        {
            Console.ForegroundColor = c;

            Console.SetCursorPosition(x, y);
            Console.Write("╔");
            Console.Write(new string('═', w - 1));
            Console.Write("╗");

            for (int i = 1; i < h - 1; i++)
            {
                Console.SetCursorPosition(x, y + i);
                Console.Write("║");
                Console.SetCursorPosition(x + w, y + i);
                Console.Write("║");
            }

            Console.SetCursorPosition(x, y + h - 1);
            Console.Write("╚");
            Console.Write(new string('═', w - 1));
            Console.Write("╝");
        }
    }
}
