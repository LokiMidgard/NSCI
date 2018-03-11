using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using NDProperty;
using NDProperty.Propertys;
using NSCI.Propertys;

namespace NSCI.UI.Controls.Layout
{
    public partial class Grid : Panel
    {
        [NDPAttach] 
        private static void OnRowChanging(OnChangingArg<NDPConfiguration, UIElement, int> arg) { }
        [NDPAttach]
        private static void OnColumnChanging(OnChangingArg<NDPConfiguration, UIElement, int> arg) { }

        [DefaultValue(1)]
        [NDPAttach]
        private static void OnRowSpanChanging(OnChangingArg<NDPConfiguration, UIElement, int> arg) { }
        [DefaultValue(1)]
        [NDPAttach]
        private static void OnColumnSpanChanging(OnChangingArg<NDPConfiguration, UIElement, int> arg) { }

        public ObservableCollection<ISizeDefinition> RowDefinitions { get; } = new ObservableCollection<ISizeDefinition>();
        public ObservableCollection<ISizeDefinition> ColumnDefinitions { get; } = new ObservableCollection<ISizeDefinition>();

        public Grid()
        {
            RowDefinitions.CollectionChanged += (sender, e) => InvalidateArrange();
            ColumnDefinitions.CollectionChanged += (sender, e) => InvalidateArrange();

        }

        protected override Size MeasureOverride(Size availableSize)
        {
            availableSize = EnsureMinMaxWidthHeight(availableSize);

            foreach (var item in Children)
                item.Measure(availableSize);


            var rows = this.RowDefinitions as IList<ISizeDefinition>;
            var columns = this.ColumnDefinitions as IList<ISizeDefinition>;
            if (rows.Count == 0)
                rows = new ISizeDefinition[] { new RelativSizeDefinition() { Size = 1 } };
            if (columns.Count == 0)
                columns = new ISizeDefinition[] { new RelativSizeDefinition() { Size = 1 } };
            int[] columnWidth = new int[columns.Count];
            int[] rowHeight = new int[rows.Count];

            // calculate singelspan items.

            for (int i = 0; i < columnWidth.Length; i++)
            {
                if (columns[i] is FixSizeDefinition f)
                    columnWidth[i] = f.Size;
                else
                {

                    var singelSpanElements = Children
                        .Where(x => Grid.ColumnSpan[x].Value == 1 && Grid.Column[x].Value == i);
                    columnWidth[i] = (int)(singelSpanElements.Any() ? singelSpanElements.Max(x => x.MarginAndDesiredSize.Width) : 0);
                }
            }
            for (int i = 0; i < rowHeight.Length; i++)
            {
                if (rows[i] is FixSizeDefinition f)
                    rowHeight[i] = f.Size;
                else
                {

                    var singelSpanElements = Children
                        .Where(x => Grid.RowSpan[x].Value == 1 && Grid.Row[x].Value == i);
                    rowHeight[i] = (int)(singelSpanElements.Any() ? singelSpanElements.Max(x => x.MarginAndDesiredSize.Height) : 0);
                }
            }

            // calculate multispan items
            foreach (var item in Children)
            {

                var row = Grid.Row[item].Value;
                var column = Grid.Column[item].Value;
                var rowSpan = Grid.RowSpan[item].Value;
                var columnSpan = Grid.ColumnSpan[item].Value;

                if (rowSpan == 1 && columnSpan == 1)
                    continue; // Nothing to do here. First step should be enough

                var xFrom = Math.Min(columns.Count - 1, column);
                var xTo = Math.Min(columns.Count - 1, column + columnSpan);
                var yFrom = Math.Min(rows.Count - 1, row);
                var yTo = Math.Min(rows.Count - 1, row + rowSpan);

                var usedWidth = columnWidth.Skip(xFrom).Take(columnSpan).Sum();
                var usedHeight = rowHeight.Skip(yFrom).Take(rowSpan).Sum();

                if (usedWidth > item.MarginAndDesiredSize.Width && columnSpan != 1) // we need more space from relativ Size Elements
                {
                    var totalRelativSizeColumns = columns.Skip(xFrom).Take(columnSpan).OfType<RelativSizeDefinition>().Sum(x => x.Size);
                    if (totalRelativSizeColumns > 0)
                    {
                        var missingWidth = item.MarginAndDesiredSize.Width - usedWidth;

                        for (int x = xFrom; x < xTo; x++)
                            if (columns[x] is RelativSizeDefinition r)
                                columnWidth[x] += (int)Math.Ceiling(missingWidth * r.Size / totalRelativSizeColumns);
                    }
                }

                if (usedHeight > item.MarginAndDesiredSize.Height && rowSpan != 1) // we need more space from relativ Size Elements
                {
                    var totalRelativSizeColumns = rows.Skip(yFrom).Take(rowSpan).OfType<RelativSizeDefinition>().Sum(x => x.Size);
                    if (totalRelativSizeColumns > 0)
                    {
                        var missingHeight = item.MarginAndDesiredSize.Height - usedHeight;

                        for (int y = yFrom; y < yTo; y++)
                            if (rows[y] is RelativSizeDefinition r)
                                rowHeight[y] += (int)Math.Ceiling(missingHeight * r.Size / totalRelativSizeColumns);
                    }
                }

            }

            // Ensure ratio between colums and rows
            var relativeWidthRatio = columns.Zip(columnWidth, (c, w) =>
            {
                if (c is RelativSizeDefinition r)
                    return r.Size != 0.0 ? w / r.Size : 0.0;
                return 0.0;
            }).Max();
            var relativeHeightRatio = rows.Zip(rowHeight, (c, h) =>
            {
                if (c is RelativSizeDefinition r)
                    return r.Size != 0.0 ? h / r.Size : 0;
                return 0.0;
            }).Max();



            for (int i = 0; i < columns.Count; i++)
                if (columns[i] is RelativSizeDefinition r)
                    columnWidth[i] = (int)Math.Round(r.Size * relativeWidthRatio);

            for (int i = 0; i < rows.Count; i++)
                if (rows[i] is RelativSizeDefinition r)
                    rowHeight[i] = (int)Math.Round(r.Size * relativeHeightRatio);

            // Ensure min max on columns and rows

            for (int i = 0; i < columns.Count; i++)
            {
                if (columns[i].Min.HasValue)
                    columnWidth[i] = Math.Max(columns[i].Min.Value, columnWidth[i]);
                if (columns[i].Max.HasValue)
                    columnWidth[i] = Math.Min(columns[i].Max.Value, columnWidth[i]);
            }

            for (int i = 0; i < rows.Count; i++)
            {
                if (rows[i].Min.HasValue)
                    rowHeight[i] = Math.Max(rows[i].Min.Value, rowHeight[i]);
                if (rows[i].Max.HasValue)
                    rowHeight[i] = Math.Min(rows[i].Max.Value, rowHeight[i]);
            }

            return new Size(columnWidth.Sum(), rowHeight.Sum());
        }

        protected override void ArrangeOverride(Size finalSize)
        {
            //base.ArrangeOverride(finalSize);

            var rows = this.RowDefinitions as IList<ISizeDefinition>;
            var columns = this.ColumnDefinitions as IList<ISizeDefinition>;
            if (rows.Count == 0)
                rows = new ISizeDefinition[] { new RelativSizeDefinition() { Size = 1 } };
            if (columns.Count == 0)
                columns = new ISizeDefinition[] { new RelativSizeDefinition() { Size = 1 } };
            int[] columnWidth = new int[columns.Count];
            int[] rowHeight = new int[rows.Count];

            var availableWidth = finalSize.Width;

            for (int i = 0; i < columnWidth.Length; i++)
                if (columns[i] is FixSizeDefinition f)
                    if (availableWidth > f.Size)
                    {
                        columnWidth[i] = f.Size;
                        availableWidth -= f.Size;
                    }
                    else
                    {
                        columnWidth[i] = (int)availableWidth;
                        availableWidth = 0;
                    }



            for (int i = 0; i < columnWidth.Length; i++)
                if (columns[i] is AutoSizeDefinition a)
                {

                    var viableElements = Children.Where(x => Column[x].Value == i && ColumnSpan[x].Value == 1);

                    var desiredWidth = viableElements.Any() ? viableElements.Max(x => x.MarginAndDesiredSize.Width) : 0;
                    if (availableWidth > desiredWidth)
                    {
                        columnWidth[i] = (int)desiredWidth;
                        availableWidth -= desiredWidth;
                    }
                    else
                    {
                        columnWidth[i] = (int)availableWidth;
                        availableWidth = 0;
                    }
                }

            var totalRelativeSize = columns.OfType<RelativSizeDefinition>().Sum(x => x.Size);
            if (totalRelativeSize > 0)
            {
                var widthRatio = availableWidth / totalRelativeSize;
                for (int i = 0; i < columnWidth.Length; i++)
                {
                    if (columns[i] is RelativSizeDefinition r)
                    {
                        var desiredWidth = (int)Math.Ceiling(r.Size * widthRatio);
                        if (availableWidth > desiredWidth)
                        {
                            columnWidth[i] = desiredWidth;
                            availableWidth -= desiredWidth;
                        }
                        else
                        {
                            columnWidth[i] = (int)availableWidth;
                            availableWidth = 0;
                        }
                    }
                }
            }

            /////////////////

            var availableHeight = finalSize.Height;

            for (int i = 0; i < rowHeight.Length; i++)
                if (rows[i] is FixSizeDefinition f)
                    if (availableHeight > f.Size)
                    {
                        rowHeight[i] = f.Size;
                        availableHeight -= f.Size;
                    }
                    else
                    {
                        rowHeight[i] = (int)availableHeight;
                        availableHeight = 0;
                    }



            for (int i = 0; i < rowHeight.Length; i++)
                if (rows[i] is AutoSizeDefinition a)
                {
                    var validElements = Children.Where(x => Row[x].Value == i && RowSpan[x].Value == 1);
                    var desiredHeight = validElements.Any() ? validElements.Max(x => x.MarginAndDesiredSize.Height) : 0;
                    if (availableHeight > desiredHeight)
                    {
                        rowHeight[i] = (int)desiredHeight;
                        availableHeight -= desiredHeight;
                    }
                    else
                    {
                        rowHeight[i] = (int)availableHeight;
                        availableHeight = 0;
                    }
                }

            totalRelativeSize = rows.OfType<RelativSizeDefinition>().Sum(x => x.Size);
            if (totalRelativeSize > 0)
            {
                var heightRatio = availableHeight / totalRelativeSize;
                for (int i = 0; i < rowHeight.Length; i++)
                {
                    if (rows[i] is RelativSizeDefinition r)
                    {
                        var desiredheight = (int)Math.Ceiling(r.Size * heightRatio);
                        if (availableHeight > desiredheight)
                        {
                            rowHeight[i] = desiredheight;
                            availableHeight -= desiredheight;
                        }
                        else
                        {
                            rowHeight[i] = (int)availableHeight;
                            availableHeight = 0;
                        }
                    }
                }
            }
            ///////////////////////////

            foreach (var item in Children)
            {
                var row = Grid.Row[item].Value;
                var column = Grid.Column[item].Value;
                var rowSpan = Grid.RowSpan[item].Value;
                var columnSpan = Grid.ColumnSpan[item].Value;

                var xFrom = Math.Min(columns.Count - 1, column);
                var xTo = Math.Min(columns.Count, column + columnSpan);
                var yFrom = Math.Min(rows.Count - 1, row);
                var yTo = Math.Min(rows.Count, row + rowSpan);

                int width = 0;
                int height = 0;
                for (int i = xFrom; i < xTo; i++)
                    width += columnWidth[i];
                for (int i = yFrom; i < yTo; i++)
                    height += rowHeight[i];

                int x = 0;
                int y = 0;
                for (int i = 0; i < xFrom; i++)
                    x += columnWidth[i];
                for (int i = 0; i < yFrom; i++)
                    y += rowHeight[i];


                if (item.MarginAndDesiredSize.Width < width)
                {
                    int diff = width - (int)item.MarginAndDesiredSize.Width;
                    switch (item.HorizontalAlignment)
                    {
                        case HorizontalAlignment.Left:
                            width -= diff;
                            break;
                        case HorizontalAlignment.Right:
                            x += diff;
                            break;
                        case HorizontalAlignment.Center:
                            x += (int)Math.Floor(diff / 2.0);
                            width -= (int)Math.Ceiling(diff / 2.0);
                            break;
                        case HorizontalAlignment.Strech:
                        default:
                            break;
                    }
                }
                if (item.MarginAndDesiredSize.Height < height)
                {
                    int diff = height - (int)item.MarginAndDesiredSize.Height;
                    switch (item.VerticalAlignment)
                    {
                        case VerticalAlignment.Top:
                            height -= diff;
                            break;
                        case VerticalAlignment.Bottom:
                            y += diff;
                            break;
                        case VerticalAlignment.Center:
                            y += (int)Math.Floor(diff / 2.0);
                            height -= (int)Math.Ceiling(diff / 2.0);
                            break;
                        case VerticalAlignment.Strech:
                        default:
                            break;
                    }
                }
                if (height > item.DesiredSize.Height)
                {
                    y += item.Margin.Top;
                    height -= (item.Margin.Bottom + item.Margin.Top);
                }

                if (width > item.DesiredSize.Width)
                {
                    x += item.Margin.Left;
                    width -= (item.Margin.Left + item.Margin.Right);
                }

                width = Math.Max(0, width);
                height = Math.Max(0, height);


                item.Arrange(new Rect(x, y, width, height));

            }
        }

        protected override void RenderOverride(IRenderFrame frame)
        {
            frame.FillRect(0, 0, frame.Width, frame.Height, Foreground, Background, (char)SpecialChars.Fill);

            for (int i = 0; i < Children.Count; i++)
            {
                var location = GetLocation(Children[i]);
                Children[i].Render(frame.GetGraphicsBuffer(location));

            }

            base.RenderOverride(frame);
        }
    }

    public interface ISizeDefinition
    {
        int? Min { get; }
        int? Max { get; }
    }
    public struct AutoSizeDefinition : ISizeDefinition
    {
        public int? Min { get; set; }

        public int? Max { get; set; }
    }
    public struct FixSizeDefinition : ISizeDefinition
    {
        public int? Min { get; set; }

        public int? Max { get; set; }
        public int Size { get; set; }
    }
    public struct RelativSizeDefinition : ISizeDefinition
    {
        public int? Min { get; set; }

        public int? Max { get; set; }
        public double Size { get; set; }

    }

}
