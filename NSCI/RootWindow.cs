using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using NSCI.UI;

namespace NSCI.Widgets
{
    public class RootWindow : UI.Controls.ContentControl
    {


        [XmlIgnore]
        public Object ViewModel { get; set; }

        public RootWindow()
        {
            Width = Console.WindowWidth;
            Height = Console.WindowHeight;
            Background = ConsoleColor.DarkBlue;
            Foreground = ConsoleColor.White;
            //SelectedBackground = ConsoleColor.Magenta;
            //ActiveBackground = ConsoleColor.DarkMagenta;
            //ActiveWidget = null;
            //AllowDraw = false;
            //this.AllChildren = new List<Widget>();
        }

        public event Action BeforeStart;


        private Widget _activeWidget;
        public Widget ActiveWidget
        {
            get
            {
                return this._activeWidget;
            }
            set
            {
                if (value != null && value != this._activeWidget)
                {
                    if (this._activeWidget != null)
                    {
                        this._activeWidget.HasFocus = false;
                        this._activeWidget.Draw();
                    }

                    this._activeWidget = value;

                    this._activeWidget.HasFocus = true;
                    this._activeWidget.Draw();
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

                foreach (var item in elementsMeasureDirty.ConsumableEnumerator())
                    item.MeasureWithLastAvailableSize();

                foreach (var item in elementsArrangeDirty.ConsumableEnumerator())
                    item.ArrangeWithLastAvailableSize();

                foreach (var item in elementsRenderDirty.ConsumableEnumerator())
                    item.RenderWithLastAvailableSize();

                if (needToDraw)
                    g.Draw();
                needToDraw = false;
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
                            //case ConsoleKey.Tab:
                            //    CycleFocus((k.Modifiers == ConsoleModifiers.Shift) ? -1 : 1);
                            //    break;
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

        internal void UnRegisterArrangeDirty(UIElement uIElement)
        {
            elementsArrangeDirty.Remove(uIElement);
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
            return (ActiveWidget as IAcceptInput).Keypress(k);
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
    }
}
