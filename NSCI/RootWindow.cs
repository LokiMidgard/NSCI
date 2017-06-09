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

namespace NSCI.Widgets
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
            //SelectedBackground = ConsoleColor.Magenta;
            //ActiveBackground = ConsoleColor.DarkMagenta;
            //ActiveWidget = null;
            //AllowDraw = false;
            //this.AllChildren = new List<Widget>();
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
                else
                    newindex = this.tabList.IndexOf(value);
                if (newindex == -1)
                    throw new ArgumentException("The Contrle can't be selected or is not a decendent of this RootWindow");
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
            //ActiveWidget = this.RootFocusableChildren.FirstOrDefault();

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
            this.Measure(new Size(g.Width, g.Height));

            this.Arrange(new Rect(0, 0, g.Width, g.Height));
            this.Render(g.GraphicsBuffer);

            g.Draw();



            while (this.running)
            {
                if (Width != Console.WindowWidth || Height != Console.WindowHeight)
                {
                    Width = Console.WindowWidth;
                    Height = Console.WindowHeight;
                    g.Resize();

                    //this.InvalidateMeasure();
                    // We ned to render evything new. :(
                    this.Measure(new Size(g.Width, g.Height));
                    this.Arrange(new Rect(0, 0, g.Width, g.Height));
                    this.Render(g.GraphicsBuffer);

                    //Draw();
                }

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



                    //if (ActiveWidget is IAcceptInput)
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
                            //case ConsoleKey.RightArrow:
                            //    MoveRight();
                            //    break;
                            //case ConsoleKey.LeftArrow:
                            //    MoveLeft();
                            //    break;
                            //case ConsoleKey.UpArrow:
                            //    MoveUp();
                            //    break;
                            //case ConsoleKey.DownArrow:
                            //    MoveDown();
                            //    break;
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

            elementsRenderDirty.Add(uIElement);
        }

        internal void RegisterArrangeDirty(UIElement uIElement)
        {
            elementsArrangeDirty.Add(uIElement);
        }

        internal void RegisterMeasureDirty(UIElement uIElement)
        {
            elementsMeasureDirty.Add(uIElement);
        }

        private bool HandleWidgetInput(ConsoleKeyInfo k)
        {
            return (ActiveControl as IAcceptInput).Keypress(k);
        }

        //private void MoveDown()
        //{
        //    var w = FindFocusableWidgetBelow(ActiveWidget);
        //    if (w != null)
        //    {
        //        ActiveWidget = w;
        //        this.lastIndex = this.RootFocusableChildren.IndexOf(ActiveWidget);
        //    }
        //}

        //private void MoveUp()
        //{
        //    var w = FindFocusableWidgetAbove(ActiveWidget);
        //    if (w != null)
        //    {
        //        ActiveWidget = w;
        //        this.lastIndex = this.RootFocusableChildren.IndexOf(ActiveWidget);
        //    }
        //}

        //private void MoveLeft()
        //{
        //    var w = FindFocusableWidgetToLeftOf(ActiveWidget);
        //    if (w != null)
        //    {
        //        ActiveWidget = w;
        //        this.lastIndex = this.RootFocusableChildren.IndexOf(ActiveWidget);
        //    }
        //}

        //private void MoveRight()
        //{
        //    var w = FindFocusableWidgetToRightOf(ActiveWidget);
        //    if (w != null)
        //    {
        //        ActiveWidget = w;
        //        this.lastIndex = this.RootFocusableChildren.IndexOf(ActiveWidget);
        //    }
        //}

        //private void EnterPressed()
        //{
        //    if (ActiveWidget != null && ActiveWidget.Enabled)
        //    {
        //        ActiveWidget.FireClickedAsync();
        //    }
        //}

        //private int lastIndex = 0;

        //private Widget FindFocusableWidgetToRightOf(Widget from)
        //{
        //    var ImmediateRight = ActivableChildren.Where(c => c.Top == from.Top && c.Left > from.Left).OrderBy(c => c.Left).FirstOrDefault();

        //    if (ImmediateRight == null)
        //    {
        //        var RoughRight = ActivableChildren.Where(c => c.Left > from.Left && Math.Abs(c.Top - from.Top) < 2).OrderBy(c => c.Left).OrderBy(c => Math.Abs(c.Top - from.Top)).FirstOrDefault();

        //        if (RoughRight == null)
        //        {
        //            return ActivableChildren.Where(c => c.Top == from.Top).OrderBy(c => c.Left).FirstOrDefault();
        //        }

        //        return RoughRight;
        //    }

        //    return ImmediateRight;
        //}

        //private Widget FindFocusableWidgetToLeftOf(Widget from)
        //{
        //    var ImmediateLeft = ActivableChildren.Where(c => c.Top == from.Top && c.Left < from.Left).OrderByDescending(c => c.Left).FirstOrDefault();

        //    if (ImmediateLeft == null)
        //    {
        //        var RoughLeft = ActivableChildren.Where(c => c.Left < from.Left && Math.Abs(c.Top - from.Top) < 2).OrderByDescending(c => c.Left).OrderBy(c => Math.Abs(c.Top - from.Top)).FirstOrDefault();

        //        if (RoughLeft == null)
        //        {
        //            return ActivableChildren.Where(c => c.Top == from.Top).OrderByDescending(c => c.Left).FirstOrDefault();
        //        }

        //        return RoughLeft;
        //    }

        //    return ImmediateLeft;
        //}

        //private Widget FindFocusableWidgetAbove(Widget from)
        //{
        //    var ImmediateAbove = ActivableChildren.Where(c => c.Left == from.Left && c.Top < from.Top).OrderByDescending(c => c.Top).FirstOrDefault();

        //    if (ImmediateAbove == null)
        //    {
        //        return ActivableChildren.Where(c => c.Top < from.Top).OrderByDescending(c => c.Top).OrderBy(c => c.Left).FirstOrDefault();
        //    }

        //    return ImmediateAbove;
        //}

        //private Widget FindFocusableWidgetBelow(Widget from)
        //{
        //    var ImmediateBelow = ActivableChildren.Where(c => c.Left == from.Left && c.Top > from.Top).OrderBy(c => c.Top).FirstOrDefault();

        //    if (ImmediateBelow == null)
        //    {
        //        return ActivableChildren.Where(c => c.Top > from.Top).OrderBy(c => c.Left).OrderBy(c => c.Top).FirstOrDefault();
        //    }

        //    return ImmediateBelow;
        //}

        //private void CycleFocus(int Direction = 1)
        //{
        //    if (ActiveWidget == null)
        //    {
        //        this.lastIndex = 0;
        //        ActiveWidget = ActivableChildren.FirstOrDefault();
        //    }
        //    else
        //    {
        //        this.lastIndex = (this.lastIndex + Direction) % ActivableChildren.Count;
        //        if (this.lastIndex == -1) { this.lastIndex = ActivableChildren.Count - 1; }
        //        ActiveWidget = ActivableChildren[this.lastIndex];
        //    }
        //}

        //private static Type[] GetWidgetTypes()
        //{
        //    return Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsClass && t.IsSubclassOf(typeof(Widget))).ToArray();
        //}

        //private XmlSerializerNamespaces GetNS()
        //{
        //    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
        //    ns.Add("", "");
        //    return ns;
        //}

        //public void Save(Stream stream)
        //{
        //    var ser = new XmlSerializer(typeof(RootWindow), GetWidgetTypes());
        //    ser.Serialize(stream, this, GetNS());
        //}

        //public void Save(string Filename)
        //{
        //    var ser = new XmlSerializer(typeof(RootWindow), GetWidgetTypes());

        //    using (var stream = new StreamWriter(Filename))
        //    {
        //        ser.Serialize(stream, this, GetNS());
        //    }
        //}

        //public String Save()
        //{
        //    var ser = new XmlSerializer(typeof(RootWindow), GetWidgetTypes());

        //    using (var stream = new StringWriter())
        //    {
        //        ser.Serialize(stream, this, GetNS());
        //        return stream.ToString();
        //    }
        //}

        //public static RootWindow LoadFromStream(Stream stream)
        //{
        //    var ser = new XmlSerializer(typeof(RootWindow), GetWidgetTypes());

        //    var obj = (RootWindow)ser.Deserialize(stream);
        //    obj.FixChildParents(obj);
        //    obj.BuildLookup();
        //    return obj;
        //}

        //public static RootWindow LoadFromString(String data)
        //{
        //    var ser = new XmlSerializer(typeof(RootWindow), GetWidgetTypes());

        //    var obj = (RootWindow)ser.Deserialize(new StringReader(data));
        //    obj.FixChildParents(obj);
        //    obj.BuildLookup();
        //    return obj;
        //}

        //public static RootWindow LoadFromFile(string Filename)
        //{
        //    var ser = new XmlSerializer(typeof(RootWindow), GetWidgetTypes());

        //    using (var stream = new StreamReader(Filename))
        //    {
        //        var obj = (RootWindow)ser.Deserialize(stream);
        //        obj.FixChildParents(obj);
        //        obj.BuildLookup();
        //        return obj;
        //    }
        //}
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
