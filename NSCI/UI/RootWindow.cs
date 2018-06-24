using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using NDProperty;
using NDProperty.Propertys;
using NDProperty.Providers.Binding;
using NSCI.Propertys;
using NSCI.UI;
using NSCI.UI.Controls;

namespace NSCI.UI
{
    public partial class RootWindow : UI.Controls.ContentControl
    {

        internal readonly OrderedList<FrameworkElement> tabList = new OrderedList<FrameworkElement>(TabComparer.Instance);
        private readonly System.Collections.Concurrent.ConcurrentQueue<PostAction> postQueue = new System.Collections.Concurrent.ConcurrentQueue<PostAction>();
        private readonly NsciOptions options;

        public RootWindow() : this(null) { }
        public RootWindow(NsciOptions options)
        {
            if (options == null)
                options = new NsciOptions();
            this.options = options;

            Width = Console.WindowWidth;
            Height = Console.WindowHeight;

            RootCurserPositionProperty.Bind(this, ActiveControlProperty.Of(this).Over(FrameworkElement.CurserPositionReadOnlyProperty).ConvertOneWay(p => p));

        }

        public Task RunOnUIThread(Action action)
        {
            var wrapper = new PostAction()
            {
                Action = action,
                TaskSource = new TaskCompletionSource<bool>()
            };
            this.postQueue.Enqueue(wrapper);
            return wrapper.TaskSource.Task;

        }

        private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            if (this.options.OnCancelPressed == null)
            {
                e.Cancel = false; // terminate process
            }
            else
            {
                e.Cancel = true; // Do not cancel. User will handle it.
                RunOnUIThread(() => this.options.OnCancelPressed(this));
            }

        }

        public event Action BeforeStart;

        [NDP(Settings = NDPropertySettings.ReadOnly | NDPropertySettings.CallOnChangedHandlerOnEquals)]
        protected virtual void OnRootCurserPositionChanging(NDProperty.Propertys.OnChangingArg<NDPConfiguration, Point?> arg)
        {
            //if (arg.Property.IsObjectValueChanging)
            {
                arg.ExecuteAfterChange += (sender, v) =>
                {

                    if (arg.Property.NewValue.HasValue)
                    {
                        var newPosition = arg.Property.NewValue.Value;
                        newPosition = this.GetLocation(ActiveControl).Location + (Size)newPosition;
                        Console.SetCursorPosition((int)newPosition.X, (int)newPosition.Y);
                        Console.CursorVisible = true;
                    }
                    else
                    {
                        Console.CursorVisible = false;
                    }
                };
            }
        }


        private int tabSelectedIndex;
        [NDP(Settings = NDPropertySettings.CallOnChangedHandlerOnEquals)]
        protected virtual void OnActiveControlChanging(NDProperty.Propertys.OnChangingArg<NDPConfiguration, FrameworkElement> arg)
        {
            if (arg.Provider.HasNewValue && arg.Provider.NewValue != null)
            {
                var indexToCheck = this.tabList.IndexOf(arg.Provider.NewValue);
                if (indexToCheck == -1)
                    arg.Provider.Reject = true;
            }

            if (arg.Property.IsObjectValueChanging)
                arg.ExecuteAfterChange += (sender, args) =>
                {
                    var newindex = this.tabList.IndexOf(arg.Provider.NewValue);
                    var newValue = args.Property.NewValue;
                    if (newindex == -1 && newValue != null)
                        throw new ArgumentException("No valid ActiveControl");


                    if (args.Property.OldValue != null)
                        args.Property.OldValue.HasFocus = false;

                    this.tabSelectedIndex = newindex;
                    if (newValue != null)
                    {
                        newValue.HasFocus = true;
                        foreach (var scrollPane in arg.Property.NewValue.GetPathToRoot().OfType<ScrollPane>())
                        {
                            scrollPane.BrinIntoView(newValue);
                        }
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
        }

        /// <summary>
        /// Start displaying the UI and handling keyboard input.
        /// </summary>
        public void Run()
        {
            this.running = true;



            Console.CancelKeyPress += Console_CancelKeyPress;
            Console.TreatControlCAsInput = options.TreatControlCAsInput;


            Console.CursorVisible = false;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Console.OutputEncoding = Encoding.UTF8;

            var inputQueue = new System.Collections.Concurrent.ConcurrentQueue<ConsoleKeyInfo>();
            Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.CurrentThread.Name = "Input Thread";
                while (true)
                    inputQueue.Enqueue(Console.ReadKey(true));
            }, TaskCreationOptions.LongRunning);


            //g.DrawRect(3, 3, 3, 3, ConsoleColor.Red, ConsoleColor.Red, UI.SpecialChars.Shade);
            Nito.AsyncEx.AsyncContext.Run(() => Loop(inputQueue));
            //var uiThread = new Nito.AsyncEx.AsyncContextThread();
            //uiThread.Factory.StartNew(Loop, inputQueue, TaskCreationOptions.LongRunning).Wait();
            //uiThread.Join();
            Console.Clear();
            Console.ResetColor(); // We do not want to have spooky colors
        }

        private Task Loop(object inputQueue) => Loop((System.Collections.Concurrent.ConcurrentQueue<ConsoleKeyInfo>)inputQueue);

        private async Task Loop(System.Collections.Concurrent.ConcurrentQueue<ConsoleKeyInfo> inputQueue)
        {
            // Running in PS Core the thread has already the name of `Pipeline Execution Thread`
            if (System.Threading.Thread.CurrentThread.Name == null)
                System.Threading.Thread.CurrentThread.Name = "UI Thread";
            
            
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
                    var active = ActiveControl;
                    if (active != null)
                    {
                        var path = active.GetPathToRoot().OfType<FrameworkElement>().ToArray();
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
                while (this.running && postQueue.TryDequeue(out var action))
                {
                    action.Action();
                    action.TaskSource.SetResult(true);
                }
            }
        }

        public override bool HandleInput(FrameworkElement originalTarget, ConsoleKeyInfo keyInfo)
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
        internal void UnRegisterRenderDirty(UIElement uIElement)
        {
            this.elementsRenderDirty.Remove(uIElement);
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


        private FrameworkElement FindFocusableControl(FrameworkElement from, SearchDirection direction)
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

        private FrameworkElement FindFocusableControl(IntEx factor, FrameworkElement from, SearchDirection direction)
        {
            var fromLocation = GetLocation(from);
            return this.tabList
                .Select(c => new { Location = GetLocation(c), Control = c })
                .OrderBy(x =>
                {
                    switch (direction)
                    {
                        case SearchDirection.Left:
                            return -x.Location.Right;
                        case SearchDirection.Top:
                            return -x.Location.Bottom;
                        case SearchDirection.Bottom:
                            return x.Location.Top;
                        case SearchDirection.Right:
                            return x.Location.Left;
                        default:
                            throw new ArgumentException();

                    }
                })
            .Where(c =>
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
        private class TabComparer : IComparer<FrameworkElement>
        {
            private TabComparer()
            {

            }
            public static readonly TabComparer Instance = new TabComparer();
            public int Compare(FrameworkElement x, FrameworkElement y)
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

            private IEnumerable<FrameworkElement> GetChainToRoot(FrameworkElement c)
            {
                while (c != null)
                {
                    yield return c;
                    c = c.VisualParent as FrameworkElement;
                }
            }
        }
    }
}
