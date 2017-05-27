using System;
using System.Collections.Generic;
using System.Linq;

namespace NSCI.UI
{
    public static class RenderFrame
    {

        public static void DrawRect(this IGraphicsBuffer buffer, int xPos, int yPos, int width, int height, ConsoleColor forground, ConsoleColor background, SpecialChars c)
        {

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    buffer[x + xPos, y + yPos] = new ColoredKey((char)c, forground, background);
        }

        public static void DrawLine(this IGraphicsBuffer buffer, Pen pen, ConsoleColor forground, ConsoleColor background, IEnumerable<(int x, int y)> points)
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
                                buffer[list[i].x, list[i].y] = new ColoredKey(pen.bottomLeft, forground, background);
                            else
                                buffer[list[i].x, list[i].y] = new ColoredKey(pen.topLeft, forground, background);
                        }
                        else
                        {
                            if (list[i - 1].y < list[i].y)
                                buffer[list[i].x, list[i].y] = new ColoredKey(pen.bottemRight, forground, background);
                            else
                                buffer[list[i].x, list[i].y] = new ColoredKey(pen.topRight, forground, background);
                        }
                    }
                    else
                        buffer[list[i].x, list[i].y] = new ColoredKey(pen.horizontal, forground, background);

                    // draw the line
                    var min = Math.Min(list[i].x, list[i + 1].x) + 1;
                    var max = Math.Max(list[i].x, list[i + 1].x);

                    for (int x = min; x < max; x++)
                        buffer[x, list[i].y] = new ColoredKey(pen.horizontal, forground, background);

                    // check if we need to draw the last element or if there is a next segment that will draw this
                    if (i == list.Count - 2)
                        buffer[list[i + 1].x, list[i + 1].y] = new ColoredKey(pen.horizontal, forground, background);
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
                                buffer[list[i].x, list[i].y] = new ColoredKey(pen.topRight, forground, background);
                            else
                                buffer[list[i].x, list[i].y] = new ColoredKey(pen.topLeft, forground, background);
                        }
                        else
                        {
                            if (list[i - 1].x < list[i].x)
                                buffer[list[i].x, list[i].y] = new ColoredKey(pen.bottemRight, forground, background);
                            else
                                buffer[list[i].x, list[i].y] = new ColoredKey(pen.bottomLeft, forground, background);
                        }
                    }
                    else
                        buffer[list[i].x, list[i].y] = new ColoredKey(pen.vertical, forground, background);

                    // draw the line
                    var min = Math.Min(list[i].y, list[i + 1].y) + 1;
                    var max = Math.Max(list[i].y, list[i + 1].y);

                    for (int y = min; y < max; y++)
                        buffer[list[i].x, y] = new ColoredKey(pen.vertical, forground, background);

                    // check if we need to draw the last element or if there is a next segment that will draw this
                    if (i == list.Count - 2)
                        buffer[list[i + 1].x, list[i + 1].y] = new ColoredKey(pen.vertical, forground, background);
                }
                lastWasHorizontal = horisontal;
            }
        }
        public static void DrawLine(this IGraphicsBuffer buffer, Pen pen, ConsoleColor forground, ConsoleColor background, params (int x, int y)[] points) => DrawLine(buffer, pen, forground, background, points as IEnumerable<(int x, int y)>);




    }
}