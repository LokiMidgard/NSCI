using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using NSCI.UI;
using NSCI.UI.Controls;

namespace NSCI.UI
{
    public partial class RootWindow : UI.Controls.ContentControl
    {

        internal readonly OrderedList<Control> tabList = new OrderedList<Control>(TabComparer.Instance);

        public RootWindow()
        {
            Width = Console.WindowWidth;
            Height = Console.WindowHeight;
            Background = ConsoleColor.DarkBlue;
            Foreground = ConsoleColor.White;
        }

        public event Action BeforeStart;

        private int tabSelectedIndex;
        [NDProperty.NDP(Settigns = NDProperty.NDPropertySettings.CallOnChangedHandlerOnEquals)]
        protected virtual void OnActiveControlChanged(NDProperty.OnChangedArg<Control> arg)
        {
            arg.ExecuteAfterChange += () =>
            {
                int newindex;
                if (this.tabSelectedIndex < this.tabList.Count && arg.NewValue == this.tabList[this.tabSelectedIndex])
                    newindex = this.tabSelectedIndex;
                else if (arg.NewValue != null)
                {

                    newindex = this.tabList.IndexOf(arg.NewValue);
                    if (newindex == -1)
                        arg.Reject = true;
                }
                else
                {
                    newindex = -1;
                }
                if (arg.OldValue != arg.NewValue)
                {
                    if (arg.OldValue != null)
                        arg.OldValue.HasFocus = false;

                    this.tabSelectedIndex = newindex;
                    if (arg.NewValue != null)
                        arg.NewValue.HasFocus = true;
                }
            };
        }


        public bool Running => this.running;
        private bool running = false;

        /// <summary>
        /// Stop displaying the UI and handling keyboard input. Can only be used in response to an in-UI event.
        /// </summary>
        public void Detach()
        {
            Console.CursorVisible = true;
            this.running = false;
            Console.Clear();
        }

        /// <summary>
        /// Start displaying the UI and handling keyboard input.
        /// </summary>
        public void Run()
        {
            this.running = true;
            Console.CursorVisible = false;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Console.OutputEncoding = Encoding.UTF8;

            var queue = new System.Collections.Concurrent.ConcurrentQueue<ConsoleKeyInfo>();
            Task.Factory.StartNew(() =>
            {
                while (true)
                    queue.Enqueue(Console.ReadKey(true));
            }, TaskCreationOptions.LongRunning);
            //g.DrawRect(3, 3, 3, 3, ConsoleColor.Red, ConsoleColor.Red, UI.SpecialChars.Shade);
            Nito.AsyncEx.AsyncContext.Run(() => Loop(queue));
            var uiThread = new Nito.AsyncEx.AsyncContextThread();
            uiThread.Factory.StartNew(Loop, queue, TaskCreationOptions.LongRunning);
            uiThread.Join();
            Console.ResetColor(); // We do not want to have spooky colors
        }

        private Task Loop(object inputQueue) => Loop((System.Collections.Concurrent.ConcurrentQueue<ConsoleKeyInfo>)inputQueue);

        private async Task Loop(System.Collections.Concurrent.ConcurrentQueue<ConsoleKeyInfo> inputQueue)
        {

            BeforeStart?.Invoke();

            var g = new UI.Graphics(this);
            Measure(new Size(g.Width, g.Height));

            Arrange(new Rect(0, 0, g.Width, g.Height));
            Render(g.GraphicsBuffer);

            //g.Draw();



            while (this.running)
            {
                if (Width != Console.WindowWidth || Height != Console.WindowHeight)
                {
                    Width = Console.WindowWidth;
                    Height = Console.WindowHeight;
                    g.Resize();

                    //this.InvalidateMeasure();
                    // We ned to render evything new. :(
                    Measure(new Size(g.Width, g.Height));
                    Arrange(new Rect(0, 0, g.Width, g.Height));
                    Render(g.GraphicsBuffer);
                }

                if (ActiveControl == null && this.tabList.Count > 0)
                    ActiveControl = this.tabList[0];

                foreach (var item in this.elementsMeasureDirty.ConsumableEnumerator())
                    item.MeasureWithLastAvailableSize();

                foreach (var item in this.elementsArrangeDirty.ConsumableEnumerator())
                    item.ArrangeWithLastAvailableSize();

                foreach (var item in this.elementsRenderDirty.ConsumableEnumerator())
                    item.RenderWithLastAvailableSize();

                if (this.needToDraw)
                    g.Draw();
                this.needToDraw = false;
                await Task.Delay(100);
                while (this.running && inputQueue.TryDequeue(out var k))
                {
                    Console.CursorVisible = false;
                    
                    var active = ActiveControl;
                    if (active != null)
                    {
                        var path = active.GetPathToRoot().ToArray();
                        bool handled = false;
                        for (int i = path.Length - 1; i >= 0 && !handled; i--)
                            if (path[i].PreviewHandleInput(active, k))
                                handled = true;

                        for (int i = 0; i < path.Length && !handled; i++)
                            if (path[i].HandleInput(active, k))
                                handled = true;
                    }
                    else
                    {
                        if (!PreviewHandleInput(this, k))
                            HandleInput(this, k);
                    }

                }


            }
        }

        public override bool HandleInput(Control originalTarget, ConsoleKeyInfo keyInfo)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.Tab:
                    if (keyInfo.Modifiers == ConsoleModifiers.Shift)
                        TabPrevious();
                    else
                        TabNext();
                    return true;

                case ConsoleKey.RightArrow:
                    MoveFocus(SearchDirection.Right);
                    return true;

                case ConsoleKey.LeftArrow:
                    MoveFocus(SearchDirection.Left);
                    return true;

                case ConsoleKey.UpArrow:
                    MoveFocus(SearchDirection.Top);
                    return true;

                case ConsoleKey.DownArrow:
                    MoveFocus(SearchDirection.Bottom);
                    return true;

                //case ConsoleKey.Spacebar:
                //case ConsoleKey.Enter:
                //    EnterPressed();
                //    break;
                case ConsoleKey.Escape:
                    this.running = false;
                    return true;
            }
            return false;

        }


        public void TabNext()
        {
            if (this.tabList.Count == 0)
                return;
            if (this.tabSelectedIndex < 0)
                this.tabSelectedIndex = 0;
            else
                this.tabSelectedIndex = (this.tabSelectedIndex + 1) % this.tabList.Count;
            ActiveControl = this.tabList[this.tabSelectedIndex];
        }
        public void TabPrevious()
        {
            if (this.tabList.Count == 0)
                return;
            if (this.tabSelectedIndex < 0)
                this.tabSelectedIndex = this.tabList.Count - 1;
            else
                this.tabSelectedIndex = (this.tabSelectedIndex - 1 + this.tabList.Count) % this.tabList.Count;
            ActiveControl = this.tabList[this.tabSelectedIndex];
        }

        internal void UnRegisterArrangeDirty(UIElement uIElement)
        {
            this.elementsArrangeDirty.Remove(uIElement);
        }

        internal void RequestDraw()
        {
            this.needToDraw = true;
        }

        private readonly OrderedList<UIElement> elementsRenderDirty = new OrderedList<UIElement>(DepthComparer.Instance);
        private readonly OrderedList<UIElement> elementsMeasureDirty = new OrderedList<UIElement>(DepthComparer.Instance);
        private readonly OrderedList<UIElement> elementsArrangeDirty = new OrderedList<UIElement>(DepthComparer.Instance);
        private bool needToDraw;

        internal void RegisterRenderDirty(UIElement uIElement)
        {

            this.elementsRenderDirty.Add(uIElement);
        }

        internal void RegisterArrangeDirty(UIElement uIElement)
        {
            this.elementsArrangeDirty.Add(uIElement);
        }

        internal void RegisterMeasureDirty(UIElement uIElement)
        {
            this.elementsMeasureDirty.Add(uIElement);
        }

        private void MoveFocus(SearchDirection direction)
        {
            if (ActiveControl == null)
                return;
            var w = FindFocusableControl(ActiveControl, direction);
            if (w != null)
            {
                ActiveControl = w;
            }
        }


        private Control FindFocusableControl(Control from, SearchDirection direction)
        {
            // First search directly in the direction
            return FindFocusableControl(IntEx.PositiveInfinity, from, direction)
                // Then search 15 deegree
                ?? FindFocusableControl(3, from, direction)
                // Then search 45 deegree
                ?? FindFocusableControl(1, from, direction);
        }

        private enum SearchDirection
        {
            Left,
            Top,
            Bottom,
            Right
        }

        private Control FindFocusableControl(IntEx factor, Control from, SearchDirection direction)
        {
            var fromLocation = GetLocation(from);
            var locationLost = this.tabList.Select(c => new { Location = GetLocation(c), Control = c }).ToArray();
            return locationLost.Where(c =>
            {
                switch (direction)
                {
                    case SearchDirection.Left:
                        {
                            var bottomGreaterControlTop = fromLocation.Top - MathEx.Abs(fromLocation.Left - c.Location.Left) / factor < c.Location.Bottom;
                            var topGreaterControlTop = fromLocation.Top - MathEx.Abs(fromLocation.Left - c.Location.Left) / factor < c.Location.Top;

                            var topLessthenControlBotom = fromLocation.Bottom + MathEx.Abs(fromLocation.Left - c.Location.Left) / factor > c.Location.Top;
                            var bottomLessthenControlBotom = fromLocation.Bottom + MathEx.Abs(fromLocation.Left - c.Location.Left) / factor > c.Location.Bottom;
                            return fromLocation.Left > c.Location.Left
                               && (
                                   bottomGreaterControlTop && !topGreaterControlTop
                                   || topLessthenControlBotom && !bottomLessthenControlBotom
                                   || topGreaterControlTop && bottomLessthenControlBotom
                                   || !topGreaterControlTop && !bottomLessthenControlBotom
                                   );
                        }

                    case SearchDirection.Top:
                        {

                            var rightGreaterControlLeft = fromLocation.Left - MathEx.Abs(fromLocation.Top - c.Location.Top) / factor < c.Location.Right;
                            var leftGreaterControlLeft = fromLocation.Left - MathEx.Abs(fromLocation.Top - c.Location.Top) / factor < c.Location.Left;

                            var leftLessthenControlRight = fromLocation.Right + MathEx.Abs(fromLocation.Top - c.Location.Top) / factor > c.Location.Right;
                            var rightLessthenControlRight = fromLocation.Right + MathEx.Abs(fromLocation.Top - c.Location.Top) / factor > c.Location.Left;
                            return fromLocation.Top > c.Location.Top
                               && (
                                   rightGreaterControlLeft && !leftGreaterControlLeft
                                   || leftLessthenControlRight && !rightLessthenControlRight
                                   || leftGreaterControlLeft && rightLessthenControlRight
                                   || !leftGreaterControlLeft && !rightLessthenControlRight
                                   );
                        }

                    case SearchDirection.Bottom:
                        {
                            var rightGreaterControlLeft = fromLocation.Left - MathEx.Abs(fromLocation.Bottom - c.Location.Bottom) / factor < c.Location.Right;
                            var leftGreaterControlLeft = fromLocation.Left - MathEx.Abs(fromLocation.Bottom - c.Location.Bottom) / factor < c.Location.Left;

                            var leftLessthenControlRight = fromLocation.Right + MathEx.Abs(fromLocation.Bottom - c.Location.Bottom) / factor > c.Location.Right;
                            var rightLessthenControlRight = fromLocation.Right + MathEx.Abs(fromLocation.Bottom - c.Location.Bottom) / factor > c.Location.Left;
                            return fromLocation.Bottom < c.Location.Bottom
                               && (
                                   rightGreaterControlLeft && !leftGreaterControlLeft
                                   || leftLessthenControlRight && !rightLessthenControlRight
                                   || leftGreaterControlLeft && rightLessthenControlRight
                                   || !leftGreaterControlLeft && !rightLessthenControlRight
                                   );
                        }

                    case SearchDirection.Right:
                        {
                            var bottomGreaterControlTop = fromLocation.Top - MathEx.Abs(fromLocation.Right - c.Location.Right) / factor < c.Location.Bottom;
                            var topGreaterControlTop = fromLocation.Top - MathEx.Abs(fromLocation.Right - c.Location.Right) / factor < c.Location.Top;

                            var topLessthenControlBotom = fromLocation.Bottom + MathEx.Abs(fromLocation.Right - c.Location.Right) / factor > c.Location.Top;
                            var bottomLessthenControlBotom = fromLocation.Bottom + MathEx.Abs(fromLocation.Right - c.Location.Right) / factor > c.Location.Bottom;
                            return fromLocation.Right < c.Location.Right
                               && (
                                   bottomGreaterControlTop && !topGreaterControlTop
                                   || topLessthenControlBotom && !bottomLessthenControlBotom
                                   || topGreaterControlTop && bottomLessthenControlBotom
                                   || !topGreaterControlTop && !bottomLessthenControlBotom
                                   );
                        }

                    default:
                        throw new ArgumentException();
                }
            })
            .OrderBy(c => c.Location.Center.X)
            .FirstOrDefault()?.Control;
        }


        private class DepthComparer : IComparer<UIElement>
        {
            private DepthComparer()
            {

            }
            public static readonly DepthComparer Instance = new DepthComparer();
            public int Compare(UIElement x, UIElement y) => x.Depth.CompareTo(y.Depth);
        }
        private class TabComparer : IComparer<Control>
        {
            private TabComparer()
            {

            }
            public static readonly TabComparer Instance = new TabComparer();
            public int Compare(Control x, Control y)
            {
                var xChain = GetChainToRoot(x).Reverse();
                var yChain = GetChainToRoot(y).Reverse();

                var compare = xChain.Zip(yChain, (first, seccond) => first.TabIndex.CompareTo(seccond.TabIndex)).FirstOrDefault(c => c != 0);

                if (compare == 0)
                {
                    // the longer shorter Chain will winn.
                    // this case should not happen offten.
                    return yChain.Count().CompareTo(xChain.Count());
                }
                return compare;
            }

            private IEnumerable<Control> GetChainToRoot(Control c)
            {
                while (c != null)
                {
                    yield return c;
                    c = c.Parent as Control;
                }
            }
        }
    }
}
