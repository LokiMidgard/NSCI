using System;
using NSCI.Widgets;

namespace NSCI.UI
{
    public abstract class UIElement
    {
        private Size lastAvailableSize;
        private IRenderFrame lastFrame;

        public RootWindow RootWindow => this is RootWindow ? this as RootWindow : Parent?.RootWindow;

        public UIElement Parent { get; internal set; }

        internal bool WasNeverrArranged { get; private set; } = true;
        internal bool WasNeverrMeasured { get; private set; } = true;
        internal bool WasNeverrRendered { get; private set; } = true;

        internal void MeasureWithLastAvailableSize()
        {
            Measure(lastAvailableSize);
        }

        //
        // Zusammenfassung:
        //     Aktualisiert die Windows.UI.Xaml. UIElement.DesiredSize von einem Windows.UI.Xaml.
        //     UIElement. In der Regel rufen Sie Objekte, die benutzerdefinierte Layouts für
        //     ihre Kinder Layout implementieren diese Methode aus ihren eigenen Windows.UI.Xaml.
        //     FrameworkElement.MeasureOverride (Windows.Foundation.Size) Implementierungen
        //     zu einem rekursiven Layout aktualisieren.
        //
        // Parameter:
        //   availableSize:
        //     Der verfügbare Speicherplatz, den ein übergeordnetes Objekt für ein untergeordnetes
        //     Objekt reservieren kann. Ein untergeordnetes Objekt kann mehr Speicherplatz erfordern,
        //     als verfügbar ist. Die angegebene Größe kann zugewiesen werden, wenn Bildlaufvorgänge
        //     oder anderes Größenänderungsverhalten in diesem speziellen Container möglich
        //     sind.
        public void Measure(Size availableSize)
        {
            try
            {
                MeasureInProgress = true;
                this.lastAvailableSize = availableSize;
                //No reason to calc the same thing.
                if (!MeasureDirty)
                    return;

                //if hidden, we should not Measure, keep dirty
                if (!IsVisible)
                {
                    DesiredSize = Size.Empty;
                    return;
                }

                // you can Arrange without measure but not measure without arrange
                InvalidateArrange();

                var desiredSize = new Size(0, 0);
                desiredSize = MeasureCore(availableSize);
                MeasureDirty = false;
                WasNeverrMeasured = false;
                //notify parent if our desired size changed
                if (DesiredSize != desiredSize)
                {
                    DesiredSize = desiredSize;
                    var p = Parent;
                    if (!p?.MeasureInProgress ?? false)
                        p.OnChildDesiredSizeChanged(this);
                }
            }
            finally
            {
                MeasureInProgress = false;
            }
        }

        /// <summary>
        /// Notification that is called by Measure of a child when
        /// it ends up with different desired size then before.
        /// </summary>
        /// <remarks>
        /// This method will only be called when Measure was not called by the Parent Measure.
        /// </remarks>
        protected virtual void OnChildDesiredSizeChanged(UIElement child)
        {
            if (!MeasureDirty)
                InvalidateMeasure();
        }

        protected virtual Size MeasureCore(Size availableSize) => Size.Empty;


        internal void ArrangeWithLastAvailableSize()
        {
            Arrange(ArrangedPosition);
        }

        //
        // Zusammenfassung:
        //     Positioniert untergeordnete Objekte und bestimmt die Größe für ein Windows.UI.Xaml.
        //     UIElement. Übergeordnete Objekte, die benutzerdefiniertes Layout für ihre untergeordneten
        //     Elemente implementieren, rufen diese Methode aus ihren Layoutüberschreibungsimplementierungen
        //     auf, um ein rekursives Layoutupdate auszuführen.
        //
        // Parameter:
        //   finalRect:
        //     Die endgültige Größe, das übergeordnete Element für das untergeordnete Element
        //     im Layout, als Windows.Foundation.Rect Wert angegeben wird.
        public void Arrange(Rect finalRect)
        {
            try
            {
                ArrangeInProgress = true;

                // Should not happen, Should we ignore it or throw an exception
                //if (!IsVisible)
                //{
                //    DesiredSize = Size.Empty;
                //    return;
                //}

                //No reason to calc the same thing.
                if (finalRect == this.ArrangedPosition)
                    return;

                this.InvalidateRender();

                ArrangedPosition = finalRect;
                ArrangeCore(finalRect.Size);
                WasNeverrArranged = false;
            }
            finally
            {
                ArrangeInProgress = false;
            }
        }

        protected virtual void ArrangeCore(Size size)
        {
        }

        internal void RenderWithLastAvailableSize()
        {
            Render(lastFrame);
        }

        public void Render(IRenderFrame frame)
        {
            try
            {
                RenderInProgress = true;
                RootWindow.RequestDraw();
                lastFrame = frame;
                RenderCore(frame);
                WasNeverrRendered = false;
            }
            finally
            {
                RenderInProgress = false;
            }
        }

        protected virtual void RenderCore(IRenderFrame frame)
        {

        }

        //
        // Zusammenfassung:
        //     Ruft die Größe ab, die diese Windows.UI.Xaml. UIElement berechnet, der während
        //     des messdurchlaufs des Layoutvorgangs.
        //
        // Rückgabewerte:
        //     Die Größe, die diese Windows.UI.Xaml. UIElement berechnet, der während des messdurchlaufs
        //     des Layoutvorgangs.
        public Size DesiredSize { get; private set; }

        public bool IsVisible { get; set; } = true;
        public bool MeasureDirty { get; private set; } = true;
        internal bool MeasureInProgress { get; private set; }
        public bool ArrangeInProgress { get; private set; }
        public bool RenderInProgress { get; private set; }
        internal Rect ArrangedPosition { get; private set; }

        //
        // Zusammenfassung:
        //     Wird den Status der Messung (Layout) für eine Windows.UI.Xaml ungültig. UIElement.
        public void InvalidateMeasure()
        {
            if (WasNeverrMeasured)
                return;
            this.MeasureDirty = true;
            RootWindow.RegisterMeasureDirty(this);
        }
        //
        // Zusammenfassung:
        //     Wird den Anordnungszustand (Layout) für eine Windows.UI.Xaml ungültig. UIElement.
        //     Nach dem Durchführen der Windows.UI.Xaml. UIElement haben das Layout aktualisiert,
        //     die asynchron ausgeführt wird.
        public void InvalidateArrange()
        {
            if (WasNeverrArranged)
                return;
            RootWindow.RegisterArrangeDirty(this);
        }
        public void InvalidateRender()
        {
            if (WasNeverrRendered)
                return;
            RootWindow.RegisterRenderDirty(this);
        }

        /// <summary>
        /// Returns the Position and Size of the Controle relative to this Controls left Upper point.
        /// </summary>
        /// <param name="uIElement"></param>
        /// <returns></returns>
        public Rect GetLocation(UIElement uIElement)
        {
            if (uIElement.Parent == this)
                return uIElement.ArrangedPosition;
            throw new NotImplementedException();
        }

    }
}