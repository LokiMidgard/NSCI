using System;

namespace NSCI.UI
{
    public class NsciOptions
    {
        /// <summary>
        /// If set prevents the application from termination on Ctrl+C and calls the suplied action. If <c>null</c> the process will be termintaed.
        /// </summary>
        /// <remarks>
        /// The delegate is executed on the UI Thread.
        /// </remarks>
        public Action<RootWindow> OnCancelPressed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the combination of the System.ConsoleModifiers.Control
        /// modifier key and System.ConsoleKey.C console key (Ctrl+C) is treated as ordinary
        /// input or as an interruption that is handled by the operating system.
        /// <c>true</c> if Ctrl+C is treated as ordinary input; otherwise, <c>false</c>. Default is <c>true</c>
        /// </summary>
        /// <value>
        /// </value>
        public bool TreatControlCAsInput { get; set; } = true;
    }
}