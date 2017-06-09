using System;
using System.Collections.Generic;
using System.Text;
using NSCI.Widgets;

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

        /// <summary>
        /// Returns a value that indecates if this controle can be selected in general.
        /// </summary>
        /// <remarks>
        /// This value will never change. It does not hold information if the control can currently be selected, e.g. when it is disabled.
        /// </remarks>
        public virtual bool SupportSelection => false;
        //
        // Zusammenfassung:
        //     Ruft einen Wert ab, der angibt, ob der Benutzer mit dem Steuerelement interagieren
        //     kann, oder legt diesen fest.
        //
        // Rückgabewerte:
        //     ** "true" ** Wenn der Benutzer mit dem Steuerelement interagieren kann; andernfalls
        //     ** "false" **.
        private bool isEnabled;
        public bool IsEnabled
        {
            get => isEnabled;
            set
            {
                if (value != this.isEnabled)
                {
                    this.isEnabled = value;
                    OnIsEnabledChanged(this.isEnabled);
                }
            }
        }

        private bool hasFocus;
        public bool HasFocus
        {
            get => hasFocus;
            set
            {
                if (this.hasFocus != value)
                    RootWindow.ActiveControl = this;
            }
        }

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
        public Color Foreground { get; set; } = Color.Inherited;
        public ConsoleColor ActualForeground
        {
            get
            {
                if (Foreground == Color.Inherited)
                {
                    var p = Parent;
                    while (p != null)
                    {
                        if (p is Control c)
                            return c.ActualForeground;
                        p = p.Parent;
                    }
                    return ConsoleColor.Black;
                }
                return (ConsoleColor)Foreground;
            }
        }


        protected virtual void OnIsEnabledChanged(bool newValue)
        {
            if(SupportSelection)
            {
                if(newValue)
                {
                    RootWindow?.tabList.Add(this);
                }
                else
                {
                    if ((RootWindow?.ActiveControl ?? null) == this)
                        RootWindow.ActiveControl = null;
                    RootWindow?.tabList.Remove(this);
                }
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
        public Color Background { get; set; } = Color.Inherited;

        public ConsoleColor ActuellBackground
        {
            get
            {
                if (Background == Color.Inherited)
                {
                    var p = Parent;
                    while (p != null)
                    {
                        if (p is Control c)
                            return c.ActuellBackground;
                        p = p.Parent;
                    }
                    return ConsoleColor.Black;
                }
                return (ConsoleColor)Background;
            }
        }

        protected override void OnRootWindowChanged(RootWindow oldWindow, RootWindow newWindow)
        {
            if (SupportSelection && IsEnabled)
            {
                if ((oldWindow?.ActiveControl ?? null) == this)
                    oldWindow.ActiveControl = null;
                oldWindow?.tabList.Remove(this);
                newWindow?.tabList.Add(this);
            }

            base.OnRootWindowChanged(oldWindow, newWindow);
        }

    }
}
