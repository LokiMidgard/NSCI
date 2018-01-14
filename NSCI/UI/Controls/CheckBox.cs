using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using NDProperty;
using NDProperty.Propertys;
using NDProperty.Providers.Binding;
using NSCI.Propertys;
using NSCI.UI.Controls.Layout;

namespace NSCI.UI.Controls
{
    public partial class CheckBox : ContentControl
    {

        static CheckBox()
        {
            Template.SetDefaultControlTemplate(Template.CreateControlTemplate((CheckBox c) =>
           {
               var grid = new Grid();

               grid.ColumnDefinitions.Add(new AutoSizeDefinition());
               grid.ColumnDefinitions.Add(new AutoSizeDefinition());
               grid.ColumnDefinitions.Add(new AutoSizeDefinition());
               grid.ColumnDefinitions.Add(new RelativSizeDefinition() { Size = 1 });

               var opening = new TextBlock();
               var closing = new TextBlock();
               var status = new TextBlock();

               TextBlock.ForegroundProperty.Bind(opening as FrameworkElement, CheckBox.HasFocusProperty.Of(c).ConvertOneWay(state => state ? ConsoleColor.Yellow : ConsoleColor.White));
               TextBlock.ForegroundProperty.Bind(status as FrameworkElement, CheckBox.HasFocusProperty.Of(c).ConvertOneWay(state => state ? ConsoleColor.Yellow : ConsoleColor.White));
               TextBlock.ForegroundProperty.Bind(closing as FrameworkElement, CheckBox.HasFocusProperty.Of(c).ConvertOneWay(state => state ? ConsoleColor.Yellow : ConsoleColor.White));

               TextBlock.TextProperty.Bind(opening, CheckBox.OpenParenthiseProperty.Of(c).ConvertOneWay(ch => ch?.ToString() ?? ""));
               TextBlock.TextProperty.Bind(status, CheckBox.IsCheckedProperty.Of(c).ConvertOneWay(state =>
               {
                   if (!state.HasValue)
                       return c.ThirdStateCharacter.ToString();
                   if (state.Value)
                       return c.CheckedCharacter.ToString();
                   else
                       return c.UnCheckedCharacter.ToString();
               }));
               TextBlock.TextProperty.Bind(closing, CheckBox.CloseParenthiseProperty.Of(c).ConvertOneWay(ch => ch?.ToString() ?? ""));

               var content = new ContentPresenter();

               Grid.Column[opening].Value = 0;
               Grid.Column[status].Value = 1;
               Grid.Column[closing].Value = 2;
               Grid.Column[content].Value = 3;

               grid.Children.Add(opening);
               grid.Children.Add(status);
               grid.Children.Add(closing);
               grid.Children.Add(content);

               return grid;
           }));
        }

        public override bool SupportSelection => true;

        [NDP]
        [DefaultValue(false)]
        protected virtual void OnIsCheckedChanging(OnChangingArg<NDPConfiguration, bool?> arg)
        {
            if (arg.Provider.HasNewValue && !arg.Provider.NewValue.HasValue && !IsThreeState)
                arg.Provider.Reject = true;
            else if (arg.Property.IsObjectValueChanging)
                InvalidateRender();
        }
        [NDP]
        protected virtual void OnIsThreeStateChanging(OnChangingArg<NDPConfiguration, bool> arg)
        {
            if (arg.Property.IsObjectValueChanging)
                InvalidateRender();
        }

        [NDP]
        [DefaultValue(Characters.Misc.SQUARE_ROOT)]
        protected virtual void OnCheckedCharacterChanging(OnChangingArg<NDPConfiguration, char> arg)
        {
            if (arg.Property.IsObjectValueChanging)
                InvalidateRender();
        }
        [NDP]
        [DefaultValue(' ')]
        protected virtual void OnUnCheckedCharacterChanging(OnChangingArg<NDPConfiguration, char> arg)
        {
            if (arg.Property.IsObjectValueChanging)
                InvalidateRender();
        }

        [NDP]
        [DefaultValue(Characters.Misc.BLACK_SQUARE)]
        protected virtual void OnThirdStateCharacterChanging(OnChangingArg<NDPConfiguration, char> arg)
        {
            if (arg.Property.IsObjectValueChanging)
                InvalidateRender();
        }

        [NDP]
        [DefaultValue('[')]
        protected virtual void OnOpenParenthiseChanging(OnChangingArg<NDPConfiguration, char?> arg)
        {
            if (arg.Property.IsObjectValueChanging)
            {
                if (arg.Property.NewValue == null || arg.Property.OldValue == null)
                    InvalidateMeasure();
                else
                    InvalidateRender();
            }
        }

        [NDP]
        [DefaultValue(']')]
        protected virtual void OnCloseParenthiseChanging(OnChangingArg<NDPConfiguration, char?> arg)
        {
            if (arg.Property.IsObjectValueChanging)
            {
                if (arg.Property.NewValue == null || arg.Property.OldValue == null)
                    InvalidateMeasure();
                else
                    InvalidateRender();
            }
        }


        public override bool HandleInput(FrameworkElement originalTarget, ConsoleKeyInfo keyInfo)
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
