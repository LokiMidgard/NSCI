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
    public class RootWindow : UI.Controls.ContentControl
    {

        internal readonly OrderedList<Control> tabList = new OrderedList<Control>(TabComparer.Instance);

        public RootWindow()
        {
            Width = Console.WindowWidth;
            Height = Console.WindowHeight;
            Background = Color.DarkBlue;
            Foreground = Color.White;
            RootWindow = this;
        }

        public event Action BeforeStart;


        private Control activeControl;
        private int tabSelectedIndex;
        public Control ActiveControl
        {
            get => this.activeControl;
            set
            {
                int newindex;
                if (this.tabSelectedIndex < this.tabList.Count && value == this.tabList[this.tabSelectedIndex])
                    newindex = this.tabSelectedIndex;
                else if (value != null)
                {

                    newindex = this.tabList.IndexOf(value);
                    if (newindex == -1)
                        throw new ArgumentException("The Contrle can't be selected or is not a decendent of this RootWindow");
                }
                else
                {
                    newindex = -1;
                }
                if (value != this.activeControl)
                {
                    if (this.activeControl != null)
                        this.activeControl.HasFocus = false;

                    this.activeControl = value;
                    this.tabSelectedIndex = newindex;
                    if (this.activeControl != null)
                        this.activeControl.HasFocus = true;
                }
            }
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
            //ActiveControl = this.RootFocusableChildren.FirstOrDefault();

            //Draw();





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

                    bool ProcessKey = true;

                    if (ProcessKey)
                        ProcessKey = !ActiveControl?.HandleInput(k) ?? true;

                    //if (ActiveControl is IAcceptInput)
                    //{
                    //    ProcessKey = false;
                    //    switch (k.Key)
                    //    {
                    //        case ConsoleKey.Tab:
                    //            CycleFocus();
                    //            break;
                    //        default:
                    //            ProcessKey = HandleWidgetInput(k);
                    //            break;
                    //    }
                    //}

                    if (ProcessKey)
                    {
                        switch (k.Key)
                        {
                            case ConsoleKey.Tab:
                                if (k.Modifiers == ConsoleModifiers.Shift)
                                    TabPrevious();
                                else
                                    TabNext();
                                break;
                            case ConsoleKey.RightArrow:
                                MoveRight();
                                break;
                            case ConsoleKey.LeftArrow:
                                MoveLeft();
                                break;
                            case ConsoleKey.UpArrow:
                                MoveUp();
                                break;
                            case ConsoleKey.DownArrow:
                                MoveDown();
                                break;
                            //case ConsoleKey.Spacebar:
                            //case ConsoleKey.Enter:
                            //    EnterPressed();
                            //    break;
                            case ConsoleKey.Escape:
                                this.running = false;
                                break;
                        }
                    }
                }


            }
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



        private void MoveDown()
        {
            if (ActiveControl == null)
                return;
            var w = FindFocusableWidgetBelow(ActiveControl);
            if (w != null)
                ActiveControl = w;
        }

        private void MoveUp()
        {
            if (ActiveControl == null)
                return;
            var w = FindFocusableWidgetAbove(ActiveControl);
            if (w != null)
            {
                ActiveControl = w;
            }
        }

        private void MoveLeft()
        {
            if (ActiveControl == null)
                return;
            var w = FindFocusableWidgetToLeftOf(ActiveControl);
            if (w != null)
            {
                ActiveControl = w;
            }
        }

        private void MoveRight()
        {
            if (ActiveControl == null)
                return;
            var w = FindFocusableControlToRightOf(ActiveControl);
            if (w != null)
            {
                ActiveControl = w;
            }
        }


        private Control FindFocusableControlToRightOf(Control from)
        {
            var fromLocation = GetLocation(from);
            var locationLost = this.tabList.Select(c => new { Location = GetLocation(c), Control = c }).ToArray();
            // First search 15 deegree
            var ImmediateRight = locationLost.Where(c => c.Location.Center.X > fromLocation.Center.X && Math.Abs(c.Location.Center.Y - fromLocation.Center.Y) < Math.Abs(c.Location.Center.X - fromLocation.Center.X) / 3).OrderBy(c => c.Location.Center.X).FirstOrDefault();

            if (ImmediateRight == null)
            {
            // Then search 45 deegree
                var RoughRight = locationLost.Where(c => c.Location.Center.X > fromLocation.Center.X && Math.Abs(c.Location.Center.Y - fromLocation.Center.Y) < Math.Abs(c.Location.Center.X - fromLocation.Center.X)).OrderBy(c => c.Location.Center.X).FirstOrDefault();
                return RoughRight?.Control;
            }

            return ImmediateRight.Control;
        }

        private Control FindFocusableWidgetToLeftOf(Control from)
        {
            var fromLocation = GetLocation(from);
            var locationLost = this.tabList.Select(c => new { Location = GetLocation(c), Control = c }).ToArray();
            // First search 15 deegree
            var ImmediateRight = locationLost.Where(c => c.Location.Center.X < fromLocation.Center.X && Math.Abs(c.Location.Center.Y - fromLocation.Center.Y) < Math.Abs(c.Location.Center.X - fromLocation.Center.X) / 3).OrderByDescending(c => c.Location.Center.X).FirstOrDefault();

            if (ImmediateRight == null)
            {
            // Then search 45 deegree
                var RoughRight = locationLost.Where(c => c.Location.Center.X < fromLocation.Center.X && Math.Abs(c.Location.Center.Y - fromLocation.Center.Y) < Math.Abs(c.Location.Center.X - fromLocation.Center.X)).OrderByDescending(c => c.Location.Center.X).FirstOrDefault();
                return RoughRight?.Control;
            }

            return ImmediateRight.Control;
        }

        private Control FindFocusableWidgetAbove(Control from)
        {
            var fromLocation = GetLocation(from);
            var locationLost = this.tabList.Select(c => new { Location = GetLocation(c), Control = c }).ToArray();
            // First search 15 deegree
            var ImmediateRight = locationLost.Where(c => c.Location.Center.Y < fromLocation.Center.Y && Math.Abs(c.Location.Center.X - fromLocation.Center.X) < Math.Abs(c.Location.Center.Y - fromLocation.Center.Y) / 3).OrderByDescending(c => c.Location.Center.Y).FirstOrDefault();

            if (ImmediateRight == null)
            {
            // Then search 45 deegree
                var RoughRight = locationLost.Where(c => c.Location.Center.Y < fromLocation.Center.Y && Math.Abs(c.Location.Center.X - fromLocation.Center.X) < Math.Abs(c.Location.Center.Y - fromLocation.Center.Y)).OrderByDescending(c => c.Location.Center.Y).FirstOrDefault();
                return RoughRight?.Control;
            }

            return ImmediateRight.Control;
        }

        private Control FindFocusableWidgetBelow(Control from)
        {
            var fromLocation = GetLocation(from);
            var locationLost = this.tabList.Select(c => new { Location = GetLocation(c), Control = c }).ToArray();
            // First search 15 deegree
            var ImmediateRight = locationLost.Where(c => c.Location.Center.Y > fromLocation.Center.Y && Math.Abs(c.Location.Center.X - fromLocation.Center.X) < Math.Abs(c.Location.Center.Y - fromLocation.Center.Y) / 3).OrderBy(c => c.Location.Center.Y).FirstOrDefault();

            if (ImmediateRight == null)
            {
            // Then search 45 deegree
                var RoughRight = locationLost.Where(c => c.Location.Center.Y > fromLocation.Center.Y && Math.Abs(c.Location.Center.X - fromLocation.Center.X) < Math.Abs(c.Location.Center.Y - fromLocation.Center.Y)).OrderBy(c => c.Location.Center.Y).FirstOrDefault();
                return RoughRight?.Control;
            }

            return ImmediateRight.Control;
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
