using System;
using System.Collections.Generic;
using System.Text;

namespace NSCI.UI
{
    public abstract class FrameworkElement : UIElement
    {
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

        public override sealed void Measure(Size availableSize)
        {
            if (!this.IsVisible)
            {
                this.DesiredSize = Size.Empty;
                return;
            }

            DesiredSize = MeasureOverride(availableSize);
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

        public override sealed void Arrange(Size finalRect)
        {
            ArrangeOverride(finalRect);
            ActualHeight = finalRect.Height;
            ActualWidth = finalRect.Width;
        }

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
        //     Ruft oder legt den äußeren Rand einer Windows.UI.Xaml. FrameworkElement.
        //
        // Rückgabewerte:
        //     Stellt Randwerte für das Objekt bereit. Der Standardwert ist eine Standard-Windows.UI.Xaml.
        //     Breite gleich 0 alle Eigenschaften (Dimensionen).
        public Thickness Margin { get; set; }

        //
        // Zusammenfassung:
        //     Ruft oder legt die horizontale Ausrichtung Merkmale, die auf eine Windows.UI.Xaml
        //     angewendet werden. FrameworkElement, wenn es in einem übergeordneten Layoutelement,
        //     z. B. einem Panel oder Elemente besteht.
        //
        // Rückgabewerte:
        //     Eine Einstellung für die horizontale Ausrichtung als Wert der Enumeration. Der
        //     Standardwert ist ** Stretch **.
        public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Strech;

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
        //     Ruft oder legt die vertikale Ausrichtung Merkmale, die auf eine Windows.UI.Xaml
        //     angewendet werden. FrameworkElement, wenn er in ein übergeordnetes Objekt z.
        //     B. ein Bereich oder Elemente besteht.
        //
        // Rückgabewerte:
        //     Eine Einstellung für die vertikale Ausrichtung als Enumerationswert. Der Standardwert
        //     ist ** Stretch **.
        public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Strech;
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
        //     Ruft das übergeordnete Objekt dieses Windows.UI.Xaml ab. FrameworkElement in
        //     der Objektstruktur.
        //
        // Rückgabewerte:
        //     Das übergeordnete Objekt dieses Objekts in der Objektstruktur.
        public UIElement Parent { get; }
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
        //     Ruft ab oder legt fest, ob ein deaktiviertes Steuerelement den Fokus erhalten
        //     kann.
        //
        // Rückgabewerte:
        //     ** "true" ** Wenn ein deaktiviertes Steuerelement den Fokus erhalten kann; andernfalls
        //     ** "false" **.
        public bool AllowFocusWhenDisabled { get; set; }


    }
}
