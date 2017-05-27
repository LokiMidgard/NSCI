using System;
using System.Collections.Generic;
using System.Text;

namespace NSCI.UI.Controls
{
    public abstract class Control : FrameworkElement
    {
        //
        // Zusammenfassung:
        //     Ruft einen Wert ab, der die Reihenfolge angibt, in der die Elemente den Fokus
        //     erhalten, wenn ein Benutzer mit der TAB-TASTE durch die Steuerelemente navigiert,
        //     oder legt diesen Wert fest.
        //
        // Rückgabewerte:
        //     Ein Wert, der die Reihenfolge für die logische Navigation für ein Gerät bestimmt.
        //     Der Standardwert ist [MaxValue](https://msdn.microsoft.com/library/System.int32.maxvalue.aspx).
        public int TabIndex { get; set; }

        //
        // Zusammenfassung:
        //     Ruft einen Wert ab, der angibt, ob der Benutzer mit dem Steuerelement interagieren
        //     kann, oder legt diesen fest.
        //
        // Rückgabewerte:
        //     ** "true" ** Wenn der Benutzer mit dem Steuerelement interagieren kann; andernfalls
        //     ** "false" **.
        public bool IsEnabled { get; set; }

        //
        // Zusammenfassung:
        //     Ruft den Abstand in einem Steuerelement ab oder legt ihn fest.
        //
        // Rückgabewerte:
        //     Den Abstand zwischen den Inhalt einer Windows.UI.Xaml. Controls.Control und seine
        //     Windows.UI.Xaml. FrameworkElement.Margin oder Windows.UI.Xaml. Controls.Border.
        //     Der Standardwert ist eine Windows.UI.Xaml. Breite mit Werten von 0 auf allen
        //     vier Seiten.
        public Thickness Padding { get; set; }

        //
        // Zusammenfassung:
        //     Ruft einen Pinsel ab, der die Vordergrundfarbe beschreibt, oder legt diesen fest.
        //
        // Rückgabewerte:
        //     Der Pinsel, der den Vordergrund des Steuerelements zeichnet. Der Standardwert
        //     ist eine Windows.UI.Xaml. Media.SolidColorBrush mit der Farbe des Windows.UI.
        //     Colors.Black.
        public ConsoleColor Foreground { get; set; }

        //
        // Zusammenfassung:
        //     Ruft einen Pinsel ab, der den Hintergrund des Steuerelements bereitstellt, oder
        //     legt diesen fest.
        //
        // Rückgabewerte:
        //     Der Pinsel, der den Hintergrund des Steuerelements bereitstellt. Der Standardwert
        //     ist ** Null ** (ein null-Pinsel) der als Windows.UI ausgewertet wird. Colors.Transparent
        //     für das Rendern.
        public ConsoleColor Background { get; set; }
    }
}
