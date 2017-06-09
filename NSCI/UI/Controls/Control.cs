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
        private bool isEnabled = true;
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
                if (!SupportSelection || !IsEnabled)
                    value = false;
                if (this.hasFocus != value)
                {
                    RootWindow.ActiveControl = this;
                    this.hasFocus = value;
                    OnHasFocusChanged(value);
                }
            }
        }

        protected virtual void OnIsEnabledChanged(bool newValue)
        {
            if (SupportSelection)
            {
                if (newValue)
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

        protected virtual void OnHasFocusChanged(bool newValue)
        {
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

        /// <summary>
        /// Handles the Input.
        /// </summary>
        /// <param name="k"></param>
        /// <returns><c>true</c>> if this control handled the input.</c></returns>
        public virtual bool HandleInput(ConsoleKeyInfo k) => false;
    }
}
