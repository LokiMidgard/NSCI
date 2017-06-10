using System;
using System.Collections.Generic;
using System.Text;

namespace NSCI.UI
{
    public abstract class FrameworkElement : UIElement
    {
        //
        // Zusammenfassung:
        //     Abgerufen oder die vorgeschlagene Höhe einer Windows.UI.Xaml festgelegt. FrameworkElement.
        //
        // Rückgabewerte:
        //     Die Höhe des Objekts in Pixel. Der Standardwert ist [NaN](https://msdn.microsoft.com/library/System.int.nan.aspx).
        //     Mit Ausnahme der speziellen [NaN](https://msdn.microsoft.com/library/System.int.nan.aspx)-Wert,
        //     lautet der Wert muss gleich oder größer als 0.
        public int? Height { get; set; } = null;

        //
        // Zusammenfassung:
        //     Ruft oder legt die Einschränkung für die Mindesthöhe des eine Windows.UI.Xaml.
        //     FrameworkElement.
        //
        // Rückgabewerte:
        //     Die minimale Höhe des Objekts in Pixel. Der Standardwert ist 0. Dieser Wert kann
        //     ein beliebiger Wert größer oder gleich 0 sein. Allerdings [PositiveInfinity](https://msdn.microsoft.com/library/System.int.positiveinfinity.aspx)
        //     ist ungültig.
        public int MinHeight { get; set; } = 0;

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
        public string Name { get; set; }

        //
        // Zusammenfassung:
        //     Abgerufen oder die Einschränkung der Mindestbreite von einem Windows.UI.Xaml
        //     festgelegt. FrameworkElement.
        //
        // Rückgabewerte:
        //     Die minimale Breite des Objekts in Pixel. Der Standardwert ist 0. Dieser Wert
        //     kann ein beliebiger Wert größer oder gleich 0 sein. Allerdings [PositiveInfinity](https://msdn.microsoft.com/library/System.int.positiveinfinity.aspx)
        //     ist ungültig.
        public int MinWidth { get; set; } = 0;

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
        public int MaxWidth { get; set; } = int.MaxValue;

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
        public int MaxHeight { get; set; } = int.MaxValue;


        //
        // Zusammenfassung:
        //     Ruft ab oder legt die Breite des eine Windows.UI.Xaml. FrameworkElement.
        //
        // Rückgabewerte:
        //     Die Breite des Objekts in Pixel. Der Standardwert ist [NaN](https://msdn.microsoft.com/library/System.int.nan.aspx).
        //     Mit Ausnahme der speziellen [NaN](https://msdn.microsoft.com/library/System.int.nan.aspx)-Wert,
        //     lautet der Wert muss gleich oder größer als 0.
        public int? Width { get; set; } = null;



        //
        // Zusammenfassung:
        //     Ruft einen beliebigen Objektwert ab, mit dem benutzerdefinierte Informationen
        //     über dieses Objekt gespeichert werden können, oder legt diesen fest.
        //
        // Rückgabewerte:
        //     Der Wert des beabsichtigten beliebigen Objekts. Diese Eigenschaft verfügt über
        //     keinen Standardwert.
        public object Tag { get; set; }

        //
        // Zusammenfassung:
        //     Ruft die gerenderte Höhe einer Windows.UI.Xaml ab. FrameworkElement. Siehe Anmerkungen.
        //
        // Rückgabewerte:
        //     Die Höhe des Objekts in Pixel. Der Standardwert ist 0. Der Standardwert wird
        //     möglicherweise gefunden, wenn das Objekt nicht geladen wurde und noch nicht Teil
        //     einer Layoutübergabe war, die die Benutzeroberfläche rendert.
        public int ActualHeight { get; private set; }

        //
        // Zusammenfassung:
        //     Ruft die gerenderte Breite ein Windows.UI.Xaml ab. FrameworkElement. Siehe Anmerkungen.
        //
        // Rückgabewerte:
        //     Die Breite des Objekts in Pixel. Der Standardwert ist 0. Der Standardwert wird
        //     möglicherweise gefunden, wenn das Objekt nicht geladen wurde und noch nicht Teil
        //     einer Layoutübergabe war, die die Benutzeroberfläche rendert.
        public int ActualWidth { get; private set; }



        //
        // Zusammenfassung:
        //     Ruft den Abstand in einem Steuerelement ab oder legt ihn fest.
        //
        // Rückgabewerte:
        //     Den Abstand zwischen den Inhalt einer Windows.UI.Xaml. Controls.Control und seine
        //     Windows.UI.Xaml. FrameworkElement.Margin oder Windows.UI.Xaml. Controls.Border.
        //     Der Standardwert ist eine Windows.UI.Xaml. Breite mit Werten von 0 auf allen
        //     vier Seiten.
        private Thickness padding;
        public Thickness Padding
        {
            get => padding; set
            {
                if (padding != value)
                {
                    var oldValue = padding;
                    padding = value;
                    OnPaddingChanged(oldValue, value);
                }
            }
        }

        protected void OnPaddingChanged(Thickness oldPadding, Thickness newPadding)
        {
            InvalidateMeasure();
        }

        //
        // Zusammenfassung:
        //     Ruft einen Pinsel ab, der die Vordergrundfarbe beschreibt, oder legt diesen fest.
        //
        // Rückgabewerte:
        //     Der Pinsel, der den Vordergrund des Steuerelements zeichnet. Der Standardwert
        //     ist eine Windows.UI.Xaml. Media.SolidColorBrush mit der Farbe des Windows.UI.
        //     Colors.Black.
        private Color foreground = Color.Inherited;
        public Color Foreground
        {
            get => foreground; set
            {
                if (value != this.foreground)
                {
                    this.foreground = value;
                    InvalidateRender();
                }
            }
        }
        public ConsoleColor ActualForeground
        {
            get
            {
                if (Foreground == Color.Inherited)
                {
                    var p = Parent;
                    while (p != null)
                    {
                        if (p is FrameworkElement c)
                            return c.ActualForeground;
                        p = p.Parent;
                    }
                    return ConsoleColor.Black;
                }
                return (ConsoleColor)Foreground;
            }
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
        public Color background = Color.Inherited;

        public Color Background
        {
            get => background; set
            {
                if (value != this.background)
                {
                    this.background = value;
                    InvalidateRender();
                }
            }
        }

        public ConsoleColor ActuellBackground
        {
            get
            {
                if (Background == Color.Inherited)
                {
                    var p = Parent;
                    while (p != null)
                    {
                        if (p is FrameworkElement c)
                            return c.ActuellBackground;
                        p = p.Parent;
                    }
                    return ConsoleColor.Black;
                }
                return (ConsoleColor)Background;
            }
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
            var width = Width ?? int.MaxValue;
            width = Math.Min(width, MaxWidth);
            width = Math.Max(width, MinWidth);
            width = Math.Min(width, availableSize.Width);

            var height = Height ?? int.MaxValue;
            height = Math.Min(height, MaxHeight);
            height = Math.Max(height, MinHeight);
            height = Math.Min(height, availableSize.Height);
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

            ActualHeight = finalRect.Height;
            ActualWidth = finalRect.Width;
            ArrangeOverride(finalRect.Inflat(-paddingWith, -paddingHeight));
        }

        protected override sealed void RenderCore(IRenderFrame frame)
        {
            frame.FillRect(0, 0, Padding.Left, Padding.Top, ActualForeground, ActuellBackground, SpecialChars.Fill);
            frame.FillRect(frame.Width - Padding.Right - Padding.Left, 0, Padding.Right, Padding.Top, ActualForeground, ActuellBackground, SpecialChars.Fill);
            frame.FillRect(0, frame.Height - Padding.Top - Padding.Bottom, Padding.Left, Padding.Bottom, ActualForeground, ActuellBackground, SpecialChars.Fill);
            frame.FillRect(frame.Width - Padding.Right - Padding.Left, frame.Height - Padding.Top - Padding.Bottom, Padding.Right, Padding.Bottom, ActualForeground, ActuellBackground, SpecialChars.Fill);

            frame.FillRect(Padding.Left, 0, frame.Width - Padding.Left - Padding.Right, Padding.Top, ActualForeground, ActuellBackground, SpecialChars.Fill);
            frame.FillRect(0, Padding.Top, Padding.Left, frame.Height - Padding.Top - Padding.Bottom, ActualForeground, ActuellBackground, SpecialChars.Fill);
            frame.FillRect(Padding.Left, frame.Height -  Padding.Bottom, frame.Width - Padding.Left-Padding.Right, Padding.Bottom, ActualForeground, ActuellBackground, SpecialChars.Fill);
            frame.FillRect(frame.Width- Padding.Right, Padding.Top, Padding.Right, frame.Height-Padding.Bottom-Padding.Top, ActualForeground, ActuellBackground, SpecialChars.Fill);

            if (Padding.Left + Padding.Right >= frame.Width || Padding.Top + Padding.Bottom >= frame.Height)
                return; // Not enough Place to draw content.

            RenderOverride(frame.GetGraphicsBuffer(new Rect(Padding.Left, Padding.Top, frame.Width - Padding.Right - Padding.Left, frame.Height - Padding.Bottom - Padding.Top)));
        }

        protected virtual void RenderOverride(IRenderFrame frame)
        {

        }
    }
}
