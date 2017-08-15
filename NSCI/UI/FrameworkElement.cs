using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using NDProperty;
using NDProperty.Propertys;

namespace NSCI.UI
{
    public abstract partial class
        FrameworkElement : UIElement
    {
        //
        // Zusammenfassung:
        //     Abgerufen oder die vorgeschlagene Höhe einer Windows.UI.Xaml festgelegt. FrameworkElement.
        //
        // Rückgabewerte:
        //     Die Höhe des Objekts in Pixel. Der Standardwert ist [NaN](https://msdn.microsoft.com/library/System.int.nan.aspx).
        //     Mit Ausnahme der speziellen [NaN](https://msdn.microsoft.com/library/System.int.nan.aspx)-Wert,
        //     lautet der Wert muss gleich oder größer als 0.
        //public int? Height { get; set; } = null;
        [NDP]
        [DefaultValue(float.NaN)]
        protected virtual void OnHeightChanging(global::NDProperty.Propertys.OnChangingArg<NDPConfiguration, IntEx> arg) { }

        //
        // Zusammenfassung:
        //     Ruft oder legt die Einschränkung für die Mindesthöhe des eine Windows.UI.Xaml.
        //     FrameworkElement.
        //
        // Rückgabewerte:
        //     Die minimale Höhe des Objekts in Pixel. Der Standardwert ist 0. Dieser Wert kann
        //     ein beliebiger Wert größer oder gleich 0 sein. Allerdings [PositiveInfinity](https://msdn.microsoft.com/library/System.int.positiveinfinity.aspx)
        //     ist ungültig.
        //public int MinHeight { get; set; } = 0;
        [DefaultValue(0)]
        [NDP]
        protected virtual void OnMinHeightChanging(OnChangingArg<NDPConfiguration, IntEx> arg) { }

        //
        // Zusammenfassung:
        //     Ruft den identifizierenden Namen des Objekts ab oder legt diesen fest. Wenn ein
        //     XAML-Prozessor die Objektstruktur aus XAML-Markup erstellt, kann Laufzeitcode
        //     anhand dieses Namens auf das in XAML deklarierte Objekt verweisen.
        //
        // Rückgabewerte:
        //     Der Name des Objekts, das eine Zeichenfolge sein muss, die in der XamlName-Grammatik
        //     gültig ist (siehe Tabelle [X: Name attribute](http://msdn.microsoft.com/library/4ff1f3ed-903A-4305-b2bd-dcd29e0c9e6d)
        //     Referenz). Der Standard ist eine leere Zeichenfolge.
        //public string Name { get; set; }
        [NDP]
        protected virtual void OnNameChanging(OnChangingArg<NDPConfiguration, string> arg) { }

        //
        // Zusammenfassung:
        //     Abgerufen oder die Einschränkung der Mindestbreite von einem Windows.UI.Xaml
        //     festgelegt. FrameworkElement.
        //
        // Rückgabewerte:
        //     Die minimale Breite des Objekts in Pixel. Der Standardwert ist 0. Dieser Wert
        //     kann ein beliebiger Wert größer oder gleich 0 sein. Allerdings [PositiveInfinity](https://msdn.microsoft.com/library/System.int.positiveinfinity.aspx)
        //     ist ungültig.
        //public int MinWidth { get; set; } = 0;
        [DefaultValue(0)]
        [NDP]
        protected virtual void OnMinWidthChanging(OnChangingArg<NDPConfiguration, IntEx> arg) { }

        //
        // Zusammenfassung:
        //     Ruft oder legt die Einschränkung für die maximale Breite des eine Windows.UI.Xaml.
        //     FrameworkElement.
        //
        // Rückgabewerte:
        //     Die maximale Breite des Objekts in Pixel. Der Standardwert ist [PositiveInfinity](https://msdn.microsoft.com/library/System.int.positiveinfinity.aspx).
        //     Dieser Wert kann ein beliebiger Wert größer oder gleich 0 sein. [PositiveInfinity]
        //     (https://msdn.microsoft.com/library/System.int.positiveinfinity.aspx) ist
        //     ebenfalls zulässig.
        //public int MaxWidth { get; set; } = int.MaxValue;
        [NDP]
        [DefaultValue(float.PositiveInfinity)]
        protected virtual void OnMaxWidthChanging(global::NDProperty.Propertys.OnChangingArg<NDPConfiguration, IntEx> arg) { }

        //
        // Zusammenfassung:
        //     Ruft oder legt die Einschränkung für die maximale Höhe des eine Windows.UI.Xaml.
        //     FrameworkElement.
        //
        // Rückgabewerte:
        //     Die maximale Höhe des Objekts in Pixel. Der Standardwert ist [PositiveInfinity](https://msdn.microsoft.com/library/System.int.positiveinfinity.aspx).
        //     Dieser Wert kann ein beliebiger Wert größer oder gleich 0 sein. [PositiveInfinity]
        //     (https://msdn.microsoft.com/library/System.int.positiveinfinity.aspx) ist
        //     ebenfalls zulässig.
        //public int MaxHeight { get; set; } = int.MaxValue;
        [NDP]
        [DefaultValue(float.PositiveInfinity)]
        protected virtual void OnMaxHeightChanging(OnChangingArg<NDPConfiguration, IntEx> arg) { }


        //
        // Zusammenfassung:
        //     Ruft ab oder legt die Breite des eine Windows.UI.Xaml. FrameworkElement.
        //
        // Rückgabewerte:
        //     Die Breite des Objekts in Pixel. Der Standardwert ist [NaN](https://msdn.microsoft.com/library/System.int.nan.aspx).
        //     Mit Ausnahme der speziellen [NaN](https://msdn.microsoft.com/library/System.int.nan.aspx)-Wert,
        //     lautet der Wert muss gleich oder größer als 0.
        //public int? Width { get; set; } = null;
        [NDP]
        [DefaultValue(float.NaN)]
        protected virtual void OnWidthChanging(OnChangingArg<NDPConfiguration, IntEx> arg) { }



        //
        // Zusammenfassung:
        //     Ruft einen beliebigen Objektwert ab, mit dem benutzerdefinierte Informationen
        //     über dieses Objekt gespeichert werden können, oder legt diesen fest.
        //
        // Rückgabewerte:
        //     Der Wert des beabsichtigten beliebigen Objekts. Diese Eigenschaft verfügt über
        //     keinen Standardwert.
        //public object Tag { get; set; }
        [NDP]
        protected virtual void OnTagChanging(OnChangingArg<NDPConfiguration, object> arg) { }

        //
        // Zusammenfassung:
        //     Ruft die gerenderte Höhe einer Windows.UI.Xaml ab. FrameworkElement. Siehe Anmerkungen.
        //
        // Rückgabewerte:
        //     Die Höhe des Objekts in Pixel. Der Standardwert ist 0. Der Standardwert wird
        //     möglicherweise gefunden, wenn das Objekt nicht geladen wurde und noch nicht Teil
        //     einer Layoutübergabe war, die die Benutzeroberfläche rendert.
        //public int ActualHeight { get; private set; }
        [NDP(Settigns = NDPropertySettings.ReadOnly)]
        protected virtual void OnActualHeightChanging(OnChangingArg<NDPConfiguration, int> arg) { }

        //
        // Zusammenfassung:
        //     Ruft die gerenderte Breite ein Windows.UI.Xaml ab. FrameworkElement. Siehe Anmerkungen.
        //
        // Rückgabewerte:
        //     Die Breite des Objekts in Pixel. Der Standardwert ist 0. Der Standardwert wird
        //     möglicherweise gefunden, wenn das Objekt nicht geladen wurde und noch nicht Teil
        //     einer Layoutübergabe war, die die Benutzeroberfläche rendert.
        //public int ActualWidth { get; private set; }
        [NDP(Settigns = NDPropertySettings.ReadOnly)]
        protected virtual void OnActualWidthChanging(OnChangingArg<NDPConfiguration, int> arg) { }



        //
        // Zusammenfassung:
        //     Ruft den Abstand in einem Steuerelement ab oder legt ihn fest.
        //
        // Rückgabewerte:
        //     Den Abstand zwischen den Inhalt einer Windows.UI.Xaml. Controls.Control und seine
        //     Windows.UI.Xaml. FrameworkElement.Margin oder Windows.UI.Xaml. Controls.Border.
        //     Der Standardwert ist eine Windows.UI.Xaml. Breite mit Werten von 0 auf allen
        //     vier Seiten.
        [NDP]
        protected virtual void OnPaddingChanging(OnChangingArg<NDPConfiguration, Thickness> arg)
        {
            arg.ExecuteAfterChange += (oldValue, newValue) =>
              {
                  if (oldValue != newValue)
                      InvalidateMeasure();
              };
        }

        //
        // Zusammenfassung:
        //     Ruft einen Pinsel ab, der die Vordergrundfarbe beschreibt, oder legt diesen fest.
        //
        // Rückgabewerte:
        //     Der Pinsel, der den Vordergrund des Steuerelements zeichnet. Der Standardwert
        //     ist eine Windows.UI.Xaml. Media.SolidColorBrush mit der Farbe des Windows.UI.
        //     Colors.Black.
        //private ConsoleColor foreground = ConsoleColor.Inherited;


        [NDP(Settigns = NDPropertySettings.Inherited)]
        [DefaultValue(ConsoleColor.White)]
        protected virtual void OnForegroundChanging(OnChangingArg<NDPConfiguration, ConsoleColor> arg)
        {
            arg.ExecuteAfterChange += (oldValue, newValue) =>
            {
                if (oldValue != newValue)
                    InvalidateRender();
            };
        }



        //
        // Zusammenfassung:
        //     Ruft einen Pinsel ab, der den Hintergrund des Steuerelements bereitstellt, oder
        //     legt diesen fest.
        //
        // Rückgabewerte:
        //     Der Pinsel, der den Hintergrund des Steuerelements bereitstellt. Der Standardwert
        //     ist ** Null ** (ein null-Pinsel) der als Windows.UI ausgewertet wird. Colors.Transparent
        //     für das Rendern.
        [NDP(Settigns = NDPropertySettings.Inherited)]
        [DefaultValue(ConsoleColor.DarkBlue)]
        protected virtual void OnBackgroundChanging(OnChangingArg<NDPConfiguration, ConsoleColor> arg)
        {
            arg.ExecuteAfterChange += (oldValue, newValue) =>
            {
                if (oldValue != newValue)
                    InvalidateRender();
            };
        }

        [NDP(Settigns = NDPropertySettings.Inherited)]
        [DefaultValue(ConsoleColor.DarkGray)]
        protected virtual void OnBackgroundDisabledChanging(OnChangingArg<NDPConfiguration, ConsoleColor> arg)
        {
            arg.ExecuteAfterChange += (oldValue, newValue) =>
            {
                if (oldValue != newValue)
                    InvalidateRender();
            };
        }

        [NDP(Settigns = NDPropertySettings.Inherited)]
        [DefaultValue(ConsoleColor.Gray)]
        protected virtual void OnForegroundDisabledChanging(OnChangingArg<NDPConfiguration, ConsoleColor> arg)
        {
            arg.ExecuteAfterChange += (oldValue, newValue) =>
            {
                if (oldValue != newValue)
                    InvalidateRender();
            };
        }

        [NDP(Settigns = NDPropertySettings.Inherited)]
        [DefaultValue(ConsoleColor.Red)]
        protected virtual void OnPrimaryColorChanging(OnChangingArg<NDPConfiguration, ConsoleColor> arg)
        {
            arg.ExecuteAfterChange += (oldValue, newValue) =>
            {
                if (oldValue != newValue)
                    InvalidateRender();
            };
        }
        [NDP(Settigns = NDPropertySettings.Inherited)]
        [DefaultValue(ConsoleColor.DarkRed)]
        protected virtual void OnPrimaryColorDisabledChanging(OnChangingArg<NDPConfiguration, ConsoleColor> arg)
        {
            arg.ExecuteAfterChange += (oldValue, newValue) =>
            {
                if (oldValue != newValue)
                    InvalidateRender();
            };
        }

        [NDP(Settigns = NDPropertySettings.Inherited)]
        [DefaultValue(ConsoleColor.Cyan)]
        protected virtual void OnSecondaryColorChanging(OnChangingArg<NDPConfiguration, ConsoleColor> arg)
        {
            arg.ExecuteAfterChange += (oldValue, newValue) =>
            {
                if (oldValue != newValue)
                    InvalidateRender();
            };
        }
        [NDP(Settigns = NDPropertySettings.Inherited)]
        [DefaultValue(ConsoleColor.DarkCyan)]
        protected virtual void OnSecondaryColorDisabledChanging(OnChangingArg<NDPConfiguration, ConsoleColor> arg)
        {
            arg.ExecuteAfterChange += (oldValue, newValue) =>
            {
                if (oldValue != newValue)
                    InvalidateRender();
            };
        }



        //
        // Zusammenfassung:
        //     Stellt das Verhalten für den "messdurchlauf" von der Layoutzyklus bereit. Klassen
        //     können diese Methode zum Definieren ihrer eigenen "Measure" Pass-Verhaltens überschreiben.
        //
        // Parameter:
        //   availableSize:
        //     Die verfügbare Größe, die dieses Objekt für untergeordnete Objekte bereitstellen
        //     kann. Sie können den Wert unendlich angeben, um festzulegen, dass das Objekt
        //     an die Größe jedes verfügbaren Inhalts angepasst wird.
        //
        // Rückgabewerte:
        //     Die Größe, die dieses Objekt auf Grundlage der Berechnung der zugewiesenen Größenangaben
        //     für untergeordnete Objekte oder möglicherweise anderer Aspekte wie der festen
        //     Containergröße bestimmt und während des Layouts benötigt.
        protected virtual Size MeasureOverride(Size availableSize)
        {
            return EnsureMinMaxWidthHeight(availableSize);
        }

        protected Size EnsureMinMaxWidthHeight(Size availableSize)
        {
            var width = Width.IsNaN ? IntEx.PositiveInfinity : Width;
            if (!MaxWidth.IsNaN)
                width = MathEx.Min(width, MaxWidth);
            if (!MinWidth.IsNaN)
                width = MathEx.Max(width, MinWidth);
            width = MathEx.Min(width, availableSize.Width);

            var height = Height.IsNaN ? IntEx.PositiveInfinity : Height;

            height = MathEx.Min(height, MaxHeight);
            height = MathEx.Max(height, MinHeight);
            height = MathEx.Min(height, availableSize.Height);
            return new Size(width, height);
        }

        protected override sealed Size MeasureCore(Size availableSize)
        {
            var paddingWith = Padding.Left + Padding.Right;
            var paddingHeight = Padding.Top + Padding.Bottom;

            return MeasureOverride(availableSize.Inflat(-paddingHeight, -paddingHeight)).Inflat(paddingWith, paddingHeight);
        }
        //
        // Zusammenfassung:
        //     Stellt das Verhalten für die "Anordnungsübergabe" des Layouts. Klassen können
        //     diese Methode zum Definieren ihrer eigenen "Arrange" Pass-Verhaltens überschreiben.
        //
        // Parameter:
        //   finalSize:
        //     Der letzte Bereich im übergeordneten Objekt, in dem dieses Objekt sich selbst
        //     und seine untergeordneten Elemente anordnen soll.
        //
        // Rückgabewerte:
        //     Die tatsächliche verwendete Größe, nachdem das Element im Layout angeordnet wurde.
        protected virtual void ArrangeOverride(Size finalSize)
        {

        }

        protected override sealed void ArrangeCore(Size finalRect)
        {
            var paddingWith = Padding.Left + Padding.Right;
            var paddingHeight = Padding.Top + Padding.Bottom;

            System.Diagnostics.Debug.Assert(finalRect.Width >= 0);
            System.Diagnostics.Debug.Assert(finalRect.Height >= 0);

            ActualHeight = (int)finalRect.Height;
            ActualWidth = (int)finalRect.Width;

            finalRect = finalRect.Inflat(-paddingWith, -paddingHeight);
            finalRect = new Size(MathEx.Max(0, finalRect.Width), MathEx.Max(0, finalRect.Height));
            ArrangeOverride(finalRect);
        }

        protected override sealed void RenderCore(IRenderFrame frame)
        {
            frame.FillRect(0, 0, Padding.Left, Padding.Top, Foreground, Background, (char)SpecialChars.Fill);
            frame.FillRect(frame.Width - Padding.Right - Padding.Left, 0, Padding.Right, Padding.Top, Foreground, Background, (char)SpecialChars.Fill);
            frame.FillRect(0, frame.Height - Padding.Top - Padding.Bottom, Padding.Left, Padding.Bottom, Foreground, Background, (char)SpecialChars.Fill);
            frame.FillRect(frame.Width - Padding.Right - Padding.Left, frame.Height - Padding.Top - Padding.Bottom, Padding.Right, Padding.Bottom, Foreground, Background, (char)SpecialChars.Fill);

            frame.FillRect(Padding.Left, 0, frame.Width - Padding.Left - Padding.Right, Padding.Top, Foreground, Background, (char)SpecialChars.Fill);
            frame.FillRect(0, Padding.Top, Padding.Left, frame.Height - Padding.Top - Padding.Bottom, Foreground, Background, (char)SpecialChars.Fill);
            frame.FillRect(Padding.Left, frame.Height - Padding.Bottom, frame.Width - Padding.Left - Padding.Right, Padding.Bottom, Foreground, Background, (char)SpecialChars.Fill);
            frame.FillRect(frame.Width - Padding.Right, Padding.Top, Padding.Right, frame.Height - Padding.Bottom - Padding.Top, Foreground, Background, (char)SpecialChars.Fill);

            if (Padding.Left + Padding.Right >= frame.Width || Padding.Top + Padding.Bottom >= frame.Height)
                return; // Not enough Place to draw content.

            RenderOverride(frame.GetGraphicsBuffer(new Rect(Padding.Left, Padding.Top, frame.Width - Padding.Right - Padding.Left, frame.Height - Padding.Bottom - Padding.Top)));
        }

        protected virtual void RenderOverride(IRenderFrame frame)
        {

        }
    }
}
