﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NSCI.UI.Controls
{
    public class Border : ContentControl
    {
        private BorderStyle style;

        public BorderStyle Style { get => this.style; set => this.style = value; }

        public Thickness BorderThikness
        {
            get
            {
                switch (Style)
                {
                    case BorderStyle.None:
                        return new Thickness();
                    case BorderStyle.DropShadowLight:
                    case BorderStyle.DropShadowMedium:
                    case BorderStyle.DropShadowStrong:
                        return new Thickness(0, 0, 1, 1);
                    case BorderStyle.SingelLined:
                    case BorderStyle.DoubleLined:
                    case BorderStyle.Block:
                        return new Thickness(1);
                    default:
                        throw new NotImplementedException($"BoarderStyle {Style} not implemented.");
                }
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            availableSize = EnsureMinMaxWidthHeight(availableSize);

            var boarderThikness = BorderThikness;

            var boarderWith = boarderThikness.Left + boarderThikness.Right;
            var boarderHeight = boarderThikness.Top + boarderThikness.Bottom;

            Content?.Measure(new Size(availableSize.Width - boarderWith, availableSize.Height - boarderHeight));

            var desiredSize = Content?.DesiredSize ?? Size.Empty;
            return new Size(desiredSize.Width + boarderWith, desiredSize.Height + boarderHeight);
        }

        protected override void ArrangeOverride(Size finalSize)
        {
            var borderThikness = BorderThikness;
            var borderWith = borderThikness.Left + borderThikness.Right;
            var borderHeight = borderThikness.Top + borderThikness.Bottom;

            Content?.Arrange(new Rect(borderThikness.Left, borderThikness.Top, finalSize.Width - borderWith, finalSize.Height - borderHeight));
        }

        protected override void RenderCore(IRenderFrame frame)
        {
            RenderBorder(frame);
            if (Content != null)
            {
                var borderThikness = BorderThikness;
                var borderWith = borderThikness.Left + borderThikness.Right;
                var borderHeight = borderThikness.Top + borderThikness.Bottom;
                Content.Render(frame.GetGraphicsBuffer(new Rect(borderThikness.Left, borderThikness.Top, frame.Width - borderWith, frame.Height - borderHeight), frame.Clip));
            }
            else
            {
                var borderThikness = BorderThikness;
                var borderWith = borderThikness.Left + borderThikness.Right;
                var borderHeight = borderThikness.Top + borderThikness.Bottom;
                frame.FillRect(borderThikness.Left, borderThikness.Top, frame.Width - borderWith, frame.Height - borderHeight, Foreground, Background, SpecialChars.Fill);
            }
        }

        private void RenderBorder(IRenderFrame frame)
        {
            RectPen borderPen;
            switch (Style)
            {
                case BorderStyle.None:
                    return;
                case BorderStyle.DropShadowLight:
                case BorderStyle.DropShadowMedium:
                case BorderStyle.DropShadowStrong:
                    RenderDropShadow(frame);
                    return;

                case BorderStyle.SingelLined:
                    borderPen = Pen.SingelLine;
                    break;
                case BorderStyle.DoubleLined:
                    borderPen = Pen.DoubleLine;
                    break;
                case BorderStyle.Block:
                    borderPen = Pen.BlockLine;
                    break;
                default:
                    throw new NotImplementedException($"BoarderStyle {Style} not implemented.");
            }

            frame.DrawRect(0, 0, frame.Width, frame.Height, Foreground, Background, borderPen);
        }

        private void RenderDropShadow(IRenderFrame frame)
        {
            char shadowChar;
            switch (Style)
            {
                case BorderStyle.DropShadowLight:
                    shadowChar = '░';
                    break;
                case BorderStyle.DropShadowMedium:
                    shadowChar = '▒';
                    break;
                case BorderStyle.DropShadowStrong:
                    shadowChar = '▓';
                    break;
                default:
                    throw new NotImplementedException($"BoarderStyle {Style} is not a shadow.");
            }
            frame[frame.Width - 1, 0] = new ColoredKey(' ', Foreground, Background);
            frame[0, frame.Height - 1] = new ColoredKey(' ', Foreground, Background);
            for (int y = 1; y < frame.Height - 1; y++)
                frame[frame.Width - 1, y] = new ColoredKey(shadowChar, Foreground, Background);
            for (int x = 1; x < frame.Width; x++)
                frame[x, frame.Height - 1] = new ColoredKey(shadowChar, Foreground, Background);
        }
    }

    public enum BorderStyle
    {
        None,
        DropShadowLight,
        DropShadowMedium,
        DropShadowStrong,
        SingelLined,
        DoubleLined,
        Block
    }
}