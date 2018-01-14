using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using NDProperty;
using NDProperty.Propertys;
using NSCI.Propertys;

namespace NSCI.UI
{ 
    public abstract partial class UIElement
    {
        private Size lastAvailableSize;
        private IRenderFrame lastFrame;
        private readonly ObservableCollection<UIElement> visualChildrean = new ObservableCollection<UIElement>();
        public ReadOnlyObservableCollection<UIElement> VisualChildrean { get; }

        private readonly ObservableCollection<UIElement> logicalChildrean = new ObservableCollection<UIElement>();
        public ReadOnlyObservableCollection<UIElement> LogicalChildrean { get; }

        public UIElement()
        {
            VisualChildrean = new ReadOnlyObservableCollection<UIElement>(this.visualChildrean);
            LogicalChildrean = new ReadOnlyObservableCollection<UIElement>(this.logicalChildrean);

            if (this is RootWindow)
                RootWindow = this as RootWindow;

        }


        [NDP(Settings = NDPropertySettings.ReadOnly)]
        protected virtual void OnRootWindowChanging(OnChangingArg<NDPConfiguration, RootWindow> arg)
        {
            if (this is RootWindow && arg.Provider.NewValue == null)
                arg.Provider.MutatedValue = this as RootWindow;
        }

        [NDP(Settings = NDPropertySettings.ParentReference)]
        protected virtual void OnVisualParentChanging(global::NDProperty.Propertys.OnChangingArg<NDPConfiguration, UIElement> arg)
        {
            if (arg.Property.IsObjectValueChanging)
                arg.ExecuteAfterChange += (sender, args) =>
                {
                    if (args.Property.OldValue != null)
                    {
                        args.Property.OldValue.RootWindowChanged -= ParentRootChanged;
                        args.Property.OldValue.visualChildrean.Remove(this);
                    }
                    Depth = args.Property.NewValue?.Depth + 1 ?? 0;

                    if (args.Property.NewValue is RootWindow r)
                        RootWindow = r;
                    else
                        RootWindow = arg.Property.NewValue?.RootWindow;
                    if (args.Property.NewValue != null)
                    {
                        args.Property.NewValue.RootWindowChanged += ParentRootChanged;
                        args.Property.NewValue.visualChildrean.Add(this);
                    }
                };

        }

        [NDP]
        protected virtual void OnLogicalParentChanging(global::NDProperty.Propertys.OnChangingArg<NDPConfiguration, UIElement> arg)
        {
            if (arg.Property.IsObjectValueChanging)
                arg.ExecuteAfterChange += (sender, args) =>
                {
                    if (args.Property.OldValue != null)
                        args.Property.OldValue.logicalChildrean.Remove(this);
                    if (args.Property.NewValue != null)
                        args.Property.NewValue.logicalChildrean.Add(this);
                };

        }


        //
        // Zusammenfassung:
        //     Ruft oder legt den äußeren Rand einer Windows.UI.Xaml. FrameworkElement.
        //
        // Rückgabewerte:
        //     Stellt Randwerte für das Objekt bereit. Der Standardwert ist eine Standard-Windows.UI.Xaml.
        //     Breite gleich 0 alle Eigenschaften (Dimensionen).
        [NDP]
        protected virtual void OnMarginChanging(OnChangingArg<NDPConfiguration, Thickness> arg)
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
        [NDP]
        [DefaultValue(VerticalAlignment.Strech)]
        protected virtual void OnVerticalAlignmentChanging(OnChangingArg<NDPConfiguration, VerticalAlignment> arg)
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

        [NDP]
        [DefaultValue(HorizontalAlignment.Strech)]
        protected virtual void OnHorizontalAlignmentChanging(OnChangingArg<NDPConfiguration, HorizontalAlignment> arg)
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


        [NDP(Settings = NDPropertySettings.ReadOnly)]
        protected virtual void OnDepthChanging(global::NDProperty.Propertys.OnChangingArg<NDPConfiguration, int> arg)
        {

        }



        [NDP]
        [DefaultValue(true)]
        protected virtual void OnIsVisibleChanging(global::NDProperty.Propertys.OnChangingArg<NDPConfiguration, bool> arg)
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
        //     Ruft die Größe ab, die diese Windows.UI.Xaml. UIElement berechnet, der während
        //     des messdurchlaufs des Layoutvorgangs.
        //
        // Rückgabewerte:
        //     Die Größe, die diese Windows.UI.Xaml. UIElement berechnet, der während des messdurchlaufs
        //     des Layoutvorgangs.
        public Size DesiredSize { get; private set; }



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
                    System.Diagnostics.Debug.Assert(desiredSize.Height >= 0 && desiredSize.Height.IsNumber);
                    System.Diagnostics.Debug.Assert(desiredSize.Width >= 0 && desiredSize.Width.IsNumber);
                    DesiredSize = desiredSize;
                    var p = VisualParent;
                    if (!p?.MeasureInProgress ?? false)
                        p.OnChildDesiredSizeChanging(this);
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
                System.Diagnostics.Debug.Assert(finalRect.Width >= 0);
                System.Diagnostics.Debug.Assert(finalRect.Height >= 0);
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
                RootWindow.UnRegisterRenderDirty(this);
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
            if (uIElement.VisualParent == this)
                return uIElement.ArrangedPosition;

            var translation = GetTranslation(uIElement);

            return uIElement.ArrangedPosition.Translate(translation);
        }

        public Point TranslateToOtherUICordinates(UIElement uIElement, Point p) => p + GetTranslation(uIElement);

        internal void MeasureWithLastAvailableSize() => Measure(this.lastAvailableSize);

        internal void ArrangeWithLastAvailableSize() => Arrange(ArrangedPosition);

        internal void RenderWithLastAvailableSize()
        {
            if (this.lastFrame != null)
                Render(this.lastFrame);
        }

        /// <summary>
        /// Notification that is called by Measure of a child when
        /// it ends up with different desired size then before.
        /// </summary>
        /// <remarks>
        /// This method will only be called when Measure was not called by the Parent Measure.
        /// </remarks>
        protected virtual void OnChildDesiredSizeChanging(UIElement child)
        {
            if (!MeasureDirty)
                InvalidateMeasure();
        }


        protected virtual Size MeasureCore(Size availableSize) => Size.Empty;

        protected virtual void ArrangeCore(Size size)
        {
        }

        protected virtual void RenderCore(IRenderFrame frame)
        {

        }

        private void ParentRootChanged(object sender, ChangedEventArgs<NDPConfiguration, UIElement, RootWindow> e)
        {
            RootWindow = e.NewValue;
        }

        private Size GetTranslation(UIElement uIElement)
        {
            var commoneAcestor = FindCommonAcestor(uIElement);

            var negativeTranslation = Point.Empty;
            for (var current = this; current != commoneAcestor; current = current.VisualParent)
                negativeTranslation += (Size)current.ArrangedPosition.Location;
            var positiveTranslation = Point.Empty;
            for (var current = this; current != commoneAcestor; current = current.VisualParent)
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
                currentOther = currentOther.VisualParent;
            while (currentThis.Depth > currentOther.Depth)
                currentThis = currentThis.VisualParent;

            while (currentThis != currentOther)
            {
                currentThis = currentThis.VisualParent;
                currentOther = currentOther.VisualParent;
            }

            return currentOther ?? throw new ArgumentException("No Common Acestor found.");
        }



    }
}