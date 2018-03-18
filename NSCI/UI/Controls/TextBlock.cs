using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NSCI.Propertys;

namespace NSCI.UI.Controls
{
    public partial class TextBlock : FrameworkElement
    {

        [NDProperty.NDP]
        protected virtual void OnTextChanging(NDProperty.Propertys.OnChangingArg<NDPConfiguration, string> arg)
        {
            arg.Provider.MutatedValue = arg.Provider.NewValue?.Replace("\t", "    ").Replace("\r", "");
            if (arg.Property.IsObjectValueChanging)
                arg.ExecuteAfterChange += (sender, e) =>
                {
                    var (_, oldWidht, oldHeight) = GetTextDimensions(e.Property.OldValue);
                    var (_, newWidht, newHeight) = GetTextDimensions(e.Property.NewValue);
                    if (oldHeight != newHeight || oldWidht != newWidht)
                        InvalidateMeasure();
                    else
                        InvalidateArrange();
                };
        }


        private string[] renderLines = new string[0];


        protected override Size MeasureOverride(Size availableSize)
        {
            availableSize = base.MeasureOverride(availableSize);

            var (lines, maxLineLength, lineheight) = GetTextDimensions(Text);
            if (maxLineLength > availableSize.Width)
            {
                maxLineLength = (int)availableSize.Width;
                if (maxLineLength == 0)
                    maxLineLength = 1;
                lineheight = lines.Select(x => (int)Math.Ceiling(x.Length / (double)maxLineLength)).Sum();
            }

            if (lineheight < (Height.IsNaN ? 0 : Height))
            {
                if (Height <= availableSize.Height)
                    lineheight = (int)Height;


            }

            return new Size(maxLineLength, lineheight);
        }

        private static (string[] lines, int width, int height) GetTextDimensions(string text)
        {
            if (text == null)
                return (new string[0], 0, 0);
            var lines = text.Split('\n');
            var maxLineLength = lines.Max(x => x.Length);
            var lineheight = lines.Length;
            return (lines, maxLineLength, lineheight);
        }

        protected override void ArrangeOverride(Size finalSize)
        {
            base.ArrangeOverride(finalSize);
            if (finalSize.Width <= 0 || finalSize.Height <= 0)
                return;// nothing to do :(
            var stringbuffer = new StringBuilder();
            var text = Text ?? string.Empty;
            for (int i = 0; i < text.Length; i++)
            {
                if (i != 0 && i % finalSize.Width == 0 && text[i] != '\n')
                    stringbuffer.AppendLine();
                stringbuffer.Append(text[i]);
            }
            this.renderLines = stringbuffer.ToString().Replace("\r", "").Split('\n');
        }

        protected override void RenderOverride(IRenderFrame frame)
        {
            var clip = frame.Clip;
            for (int y = (int)clip.Top; y < clip.Bottom; y++)
                for (int x = (int)clip.Left; x < clip.Right; x++)
                    frame[x, y] = new ColoredKey(y < this.renderLines.Length && x < this.renderLines[y].Length ? this.renderLines[y][x] : ' ', Foreground, Background);

        }
    }
}
