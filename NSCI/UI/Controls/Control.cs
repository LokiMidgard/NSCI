using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using NDProperty;
using NDProperty.Propertys;
using NSCI.Widgets;

namespace NSCI.UI.Controls
{
    public abstract partial class Control : FrameworkElement
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
        [NDProperty.NDP]
        protected virtual void OnTabIndexChanging(NDProperty.Propertys.OnChangingArg<NDPConfiguration, int> arg)
        {

        }



        //
        // Zusammenfassung:
        //     Ruft ab oder legt fest, ob ein deaktiviertes Steuerelement den Fokus erhalten
        //     kann.
        //
        // Rückgabewerte:
        //     ** "true" ** Wenn ein deaktiviertes Steuerelement den Fokus erhalten kann; andernfalls
        //     ** "false" **.
        [NDProperty.NDP]
        protected virtual void OnAllowFocusWhenDisabledChanging(global::NDProperty.Propertys.OnChangingArg<NDPConfiguration, bool> arg)
        {

        }



        /// <summary>
        /// Returns a value that indecates if this controle can be selected in general.
        /// </summary>
        /// <remarks>
        /// This value will never change. It does not hold information if the control can currently be selected, e.g. when it is disabled.
        /// </remarks>
        public virtual bool SupportSelection => false;


        [NDProperty.NDP]
        protected virtual void OnHasFocusChanging(NDProperty.Propertys.OnChangingArg<NDPConfiguration, bool> arg)
        {
            if (!SupportSelection || !IsEnabled)
                arg.MutatedValue = false;

            arg.ExecuteAfterChange += (oldValue, newValue) =>
            {
                if (arg.MutatedValue && RootWindow.ActiveControl != this)
                    RootWindow.ActiveControl = this;
                else if (!arg.MutatedValue && RootWindow.ActiveControl == this)
                    RootWindow.ActiveControl = null;
            };

        }

        //
        // Zusammenfassung:
        //     Ruft einen Wert ab, der angibt, ob der Benutzer mit dem Steuerelement interagieren
        //     kann, oder legt diesen fest.
        //
        // Rückgabewerte:
        //     ** "true" ** Wenn der Benutzer mit dem Steuerelement interagieren kann; andernfalls
        //     ** "false" **.
        [NDP]
        [DefaultValue(true)]
        protected virtual void OnIsEnabledChanging(global::NDProperty.Propertys.OnChangingArg<NDPConfiguration, bool> arg)
        {
            if (SupportSelection)
            {
                if (arg.NewValue)
                {
                    RootWindow?.tabList.Add(this);
                }
                else
                {
                    arg.ExecuteAfterChange += (oldValue, newValue) =>
                    {
                        if ((RootWindow?.ActiveControl ?? null) == this)
                            RootWindow.ActiveControl = null;
                        RootWindow?.tabList.Remove(this);
                    };
                }
            }
        }


        protected override void OnRootWindowChanging(OnChangingArg<NDPConfiguration, RootWindow> arg)
        {
            arg.ExecuteAfterChange += (oldValue, newValue) =>
            {
                if (SupportSelection && IsEnabled)
                {
                    if ((oldValue?.ActiveControl ?? null) == this)
                        oldValue.ActiveControl = null;
                    oldValue?.tabList.Remove(this);
                    newValue?.tabList.Add(this);
                }

                base.OnRootWindowChanging(arg);
            };
        }

        /// <summary>
        /// Handles the Input.
        /// </summary>
        /// <param name="k"></param>
        /// <returns><c>true</c>> if this control handled the input.</c></returns>
        public virtual bool HandleInput(Control originalTarget, ConsoleKeyInfo keyInfo) => false;
        /// <summary>
        /// Handles the Input.
        /// </summary>
        /// <param name="k"></param>
        /// <returns><c>true</c>> if this control handled the input.</c></returns>
        public virtual bool PreviewHandleInput(Control originalTarget, ConsoleKeyInfo keyInfo) => false;

        internal IEnumerable<Control> GetPathToRoot()
        {
            UIElement current = this;
            while (current != null)
            {
                if (current is Control c)
                    yield return c;
                current = current.Parent;
            }
        }
    }

}
