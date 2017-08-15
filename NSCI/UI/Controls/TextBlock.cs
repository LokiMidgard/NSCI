using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NSCI.UI.Controls
{
    public partial class TextBlock : Control
    {

        [NDProperty.NDP]
        protected virtual void OnTextChanging(NDProperty.Propertys.OnChangingArg<NDPConfiguration, string> arg)
        {
            arg.MutatedValue = arg.NewValue?.Replace("\t", "    ").Replace("\r", "");
        }


        private string[] renderLines = new string[0];


        protected override Size MeasureOverride(Size availableSize)
        {
            availableSize = base.MeasureOverride(availableSize);


            var lines = Text?.Split('\n') ?? new string[0];
            var maxLineLength = lines.Max(x => x.Length);
            int lineheight = lines.Length;
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

        protected override void ArrangeOverride(Size finalSize)
        {
            base.ArrangeOverride(finalSize);
            if (finalSize.Width <= 0 || finalSize.Height <= 0)
                return;// nothing to do :(
            var stringbuffer = new StringBuilder();
            var text = Text;
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
            for (int y = 0; y < frame.Height; y++)
                for (int x = 0; x < frame.Width; x++)
                    frame[x, y] = new ColoredKey(y < this.renderLines.Length && x < this.renderLines[y].Length ? this.renderLines[y][x] : ' ', Foreground, Background);

        }
    }
}
