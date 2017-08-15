using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using NDProperty;
using NDProperty.Propertys;

namespace NSCI.UI.Controls
{
    public partial class CheckBox : ContentControl
    {

        public override bool SupportSelection => true;

        [NDP]
        [DefaultValue(false)]
        protected virtual void OnIsCheckedChanging(OnChangingArg<NDPConfiguration, bool?> arg)
        {
            if (!arg.NewValue.HasValue && !this.IsThreeState)
                arg.Reject = true;
            else
                InvalidateRender();
        }
        [NDP]
        protected virtual void OnIsThreeStateChanging(OnChangingArg<NDPConfiguration, bool> arg)
        {
            InvalidateRender();
        }

        [NDP]
        [DefaultValue(Characters.Misc.SQUARE_ROOT)]
        protected virtual void OnCheckedCharacterChanging(OnChangingArg<NDPConfiguration, char> arg)
        {
            InvalidateRender();
        }
        [NDP]
        [DefaultValue(' ')]
        protected virtual void OnUnCheckedCharacterChanging(OnChangingArg<NDPConfiguration, char> arg)
        {
            InvalidateRender();
        }

        [NDP]
        [DefaultValue(Characters.Misc.BLACK_SQUARE)]
        protected virtual void OnThirdStateCharacterChanging(OnChangingArg<NDPConfiguration, char> arg)
        {
            InvalidateRender();
        }

        [NDP]
        [DefaultValue('[')]
        protected virtual void OnOpenParenthiseChanging(OnChangingArg<NDPConfiguration, char?> arg)
        {
            arg.ExecuteAfterChange += (oldValue, newValue) =>
            {

                if (newValue == null || oldValue == null)
                    InvalidateMeasure();
                else
                    InvalidateRender();
            };
        }

        [NDP]
        [DefaultValue(']')]
        protected virtual void OnCloseParenthiseChanging(OnChangingArg<NDPConfiguration, char?> arg)
        {
            arg.ExecuteAfterChange += (oldValue, newValue) =>
            {

                if (newValue == null || oldValue == null)
                    InvalidateMeasure();
                else
                    InvalidateRender();
            };
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Content != null)
            {
                availableSize = EnsureMinMaxWidthHeight(availableSize);
                Content.Measure(availableSize);
                var desiredSize = new Size(Content.DesiredSize.Width + (CloseParenthise.HasValue ? 1 : 0) + (OpenParenthise.HasValue ? 1 : 0) + 1, MathEx.Max(Content.DesiredSize.Height, 1));
                return desiredSize;
            }
            else
                return new Size(CheckWidth, 1);
        }

        private int CheckWidth => (CloseParenthise.HasValue ? 1 : 0) + (OpenParenthise.HasValue ? 1 : 0) + 1;

        protected override void ArrangeOverride(Size finalSize)
        {
            if (Content != null && finalSize.Width > CheckWidth)
            {
                Content.Arrange(new Rect(new Point(CheckWidth, 0), finalSize.Inflat(-CheckWidth, 0)));
            }
        }

        protected override void RenderOverride(IRenderFrame frame)
        {
            if (Content != null && Content.ArrangedPosition.Width > CheckWidth)
            {
                Content.Render(frame.GetGraphicsBuffer(new Rect(CheckWidth, 0, frame.Width - CheckWidth, frame.Height)));// Arrange(new Rect(new Point(CheckWidth, 0), finalSize.Inflat(-CheckWidth, 0)));
            }
            if (OpenParenthise != null)
                frame[0, 0] = new ColoredKey(OpenParenthise.Value, Foreground, Background);
            if (CloseParenthise != null)
                frame[CheckWidth - 1, 0] = new ColoredKey(CloseParenthise.Value, Foreground, Background);
            char selectionChar;
            if (!IsChecked.HasValue)
                selectionChar = ThirdStateCharacter;
            else if (IsChecked.Value)
                selectionChar = CheckedCharacter;
            else
                selectionChar = UnCheckedCharacter;

            frame[OpenParenthise.HasValue ? 1 : 0, 0] = new ColoredKey(selectionChar, Foreground, Background);
        }

        public override bool HandleInput(Control originalTarget, ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key == ConsoleKey.Enter || keyInfo.Key == ConsoleKey.Spacebar)
            {
                IsChecked = !(IsChecked ?? false);
                return true;
            }
            return base.HandleInput(originalTarget, keyInfo);
        }


    }
}
