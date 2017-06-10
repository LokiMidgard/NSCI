using System;
using NSCI.Widgets;

namespace NSCI.UI
{
    public abstract class UIElement
    {
        private Size lastAvailableSize;
        private IRenderFrame lastFrame;

        private RootWindow rootWindow;

        private UIElement parent;

        public event EventHandler<EventArgs<(RootWindow oldRoot, RootWindow newRoot)>> RootWindowChanged;
        public event EventHandler<EventArgs<(int oldRoot, int newRoot)>> DepthChanged;

        public RootWindow RootWindow
        {
            get => this.rootWindow; set
            {
                var oldWindow = this.rootWindow;
                if (oldWindow != value)
                {
                    this.rootWindow = value;
                    OnRootWindowChanged(oldWindow, this.rootWindow);
                }
            }
        }
        public UIElement Parent
        {
            get => parent;
            internal set
            {
                if (this.parent != value)
                {
                    if (this.parent != null)
                        this.parent.RootWindowChanged -= ParentRootChanged;
                    this.parent = value;
                    Depth = this.parent?.Depth + 1 ?? 0;

                    if (this.parent is RootWindow r)
                        RootWindow = r;
                    else
                        RootWindow = this.parent?.RootWindow;
                    if (this.parent != null)
                        this.parent.RootWindowChanged += ParentRootChanged;
                }
            }
        }

        //
        // Zusammenfassung:
        //     Ruft oder legt den äußeren Rand einer Windows.UI.Xaml. FrameworkElement.
        //
        // Rückgabewerte:
        //     Stellt Randwerte für das Objekt bereit. Der Standardwert ist eine Standard-Windows.UI.Xaml.
        //     Breite gleich 0 alle Eigenschaften (Dimensionen).
        private Thickness margin;
        public Thickness Margin
        {
            get => margin; set
            {
                if (value != this.margin)
                {
                    var oldValue = this.margin;
                    this.margin = value;
                    OnMarginChanged(oldValue, value);
                }
            }
        }

        protected virtual void OnMarginChanged(Thickness oldValue, Thickness value)
        {
            InvalidateMeasure();
        }

        //
        // Zusammenfassung:
        //     Ruft oder legt die vertikale Ausrichtung Merkmale, die auf eine Windows.UI.Xaml
        //     angewendet werden. FrameworkElement, wenn er in ein übergeordnetes Objekt z.
        //     B. ein Bereich oder Elemente besteht.
        //
        // Rückgabewerte:
        //     Eine Einstellung für die vertikale Ausrichtung als Enumerationswert. Der Standardwert
        //     ist ** Stretch **.
        private VerticalAlignment verticalAlignment = VerticalAlignment.Strech;
        public VerticalAlignment VerticalAlignment
        {
            get => verticalAlignment; set
            {
                if (value != this.verticalAlignment)
                {
                    var oldValue = this.verticalAlignment;
                    this.verticalAlignment = value;
                    OnVerticalAlignmentChanged(oldValue, value);
                }
            }
        }

        protected virtual void OnVerticalAlignmentChanged(VerticalAlignment oldValue, VerticalAlignment newValue)
        {
            InvalidateArrange();
        }

        //
        // Zusammenfassung:
        //     Ruft oder legt die horizontale Ausrichtung Merkmale, die auf eine Windows.UI.Xaml
        //     angewendet werden. FrameworkElement, wenn es in einem übergeordneten Layoutelement,
        //     z. B. einem Panel oder Elemente besteht.
        //
        // Rückgabewerte:
        //     Eine Einstellung für die horizontale Ausrichtung als Wert der Enumeration. Der
        //     Standardwert ist ** Stretch **.
        private HorizontalAlignment horizontalAlignment = HorizontalAlignment.Strech;
        public HorizontalAlignment HorizontalAlignment
        {
            get => horizontalAlignment; set
            {
                if (this.horizontalAlignment != value)
                {
                    var oldValue = this.horizontalAlignment;
                    this.horizontalAlignment = value;
                    OnHorizontalAlignmentChanged(oldValue, value);
                }
            }
        }

        protected virtual void OnHorizontalAlignmentChanged(HorizontalAlignment oldValue, HorizontalAlignment newValue)
        {
            InvalidateArrange();
        }


        /// <summary>
        /// Gets the depth in the UI Tree.
        /// </summary>
        /// <remarks>
        /// A direct Child of the RootWindow has depth 1. <para/>
        /// The RootWindow has depth 0 and an UIElement with no parent has also depth 0.
        /// </remarks>
        private int depth;
        public int Depth
        {
            get => depth; private set
            {
                if (this.depth != value)
                {
                    var oldValue = this.depth;
                    this.depth = value;
                    OnDepthChanged(oldValue, value);
                }
            }
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

        private bool isVisible = true;
        public bool IsVisible
        {
            get => isVisible; set
            {
                if (value != this.isVisible)
                {
                    this.isVisible = value;
                    OnIsVisibleChanged(value);
                }
            }
        }

        protected virtual void OnIsVisibleChanged(bool newValue)
        {
            InvalidateMeasure();
        }

        public bool MeasureDirty { get; private set; } = true;

        public bool ArrangeInProgress { get; private set; }

        public bool RenderInProgress { get; private set; }

        internal bool MeasureInProgress { get; private set; }

        internal Rect ArrangedPosition { get; private set; }

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
                    MeasureDirty = false;
                    return;
                }

                // you can Arrange without measure but not measure without arrange
                InvalidateArrange();

                var desiredSize = new Size(0, 0);
                desiredSize = MeasureCore(availableSize);
                MeasureDirty = false;
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

                InvalidateRender();
                RootWindow.UnRegisterArrangeDirty(this);
                ArrangedPosition = finalRect;
                ArrangeCore(finalRect.Size);
            }
            finally
            {
                ArrangeInProgress = false;
            }
        }

        public void Render(IRenderFrame frame)
        {
            try
            {
                RenderInProgress = true;
                RootWindow.RequestDraw();
                this.lastFrame = frame;
                RenderCore(frame);
            }
            finally
            {
                RenderInProgress = false;
            }
        }

        //
        // Zusammenfassung:
        //     Wird den Status der Messung (Layout) für eine Windows.UI.Xaml ungültig. UIElement.
        public void InvalidateMeasure()
        {
            MeasureDirty = true;
            RootWindow?.RegisterMeasureDirty(this);
        }

        //
        // Zusammenfassung:
        //     Wird den Anordnungszustand (Layout) für eine Windows.UI.Xaml ungültig. UIElement.
        //     Nach dem Durchführen der Windows.UI.Xaml. UIElement haben das Layout aktualisiert,
        //     die asynchron ausgeführt wird.
        public void InvalidateArrange()
        {
            RootWindow?.RegisterArrangeDirty(this);
        }

        public void InvalidateRender()
        {
            RootWindow?.RegisterRenderDirty(this);
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

            var translation = GetTranslation(uIElement);

            return uIElement.ArrangedPosition.Translate(translation);
        }

        public Point TranslateToOtherUICordinates(UIElement uIElement, Point p) => p + GetTranslation(uIElement);

        internal void MeasureWithLastAvailableSize() => Measure(this.lastAvailableSize);

        internal void ArrangeWithLastAvailableSize() => Arrange(ArrangedPosition);

        internal void RenderWithLastAvailableSize() => Render(this.lastFrame);

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

        protected virtual void OnRootWindowChanged(RootWindow oldWindow, RootWindow newWindow)
        {
            RootWindowChanged?.Invoke(this, new EventArgs<(RootWindow oldRoot, RootWindow newRoot)>((oldWindow, newWindow)));
        }
        protected virtual void OnDepthChanged(int oldDepth, int newDepth)
        {
            DepthChanged?.Invoke(this, new EventArgs<(int oldDepth, int newDepth)>((oldDepth, newDepth)));
        }

        protected virtual Size MeasureCore(Size availableSize) => Size.Empty;

        protected virtual void ArrangeCore(Size size)
        {
        }

        protected virtual void RenderCore(IRenderFrame frame)
        {

        }

        private void ParentRootChanged(object sender, EventArgs<(RootWindow oldRoot, RootWindow newRoot)> e)
        {
            RootWindow = e.Argument.newRoot;
        }
        private Size GetTranslation(UIElement uIElement)
        {
            var commoneAcestor = FindCommonAcestor(uIElement);

            var negativeTranslation = Point.Empty;
            for (var current = this; current != commoneAcestor; current = current.Parent)
                negativeTranslation += (Size)current.ArrangedPosition.Location;
            var positiveTranslation = Point.Empty;
            for (var current = this; current != commoneAcestor; current = current.Parent)
                positiveTranslation += (Size)current.ArrangedPosition.Location;

            var translation = positiveTranslation - (Size)negativeTranslation;
            return (Size)translation;
        }

        private UIElement FindCommonAcestor(UIElement uIElement)
        {
            var currentOther = uIElement;
            var currentThis = this;

            // first get to the same depth or it can't be working.
            while (currentOther.Depth > currentThis.Depth)
                currentOther = currentOther.Parent;
            while (currentThis.Depth > currentOther.Depth)
                currentThis = currentThis.Parent;

            while (currentThis!= currentOther)
            {
                currentThis = currentThis.Parent;
                currentOther = currentOther.Parent;
            }

            return currentOther ?? throw new ArgumentException("No Common Acestor found.");
        }
    }
}