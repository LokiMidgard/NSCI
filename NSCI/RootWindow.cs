using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NSCI.Widgets
{
    public class RootWindow : Widget
    {
        [XmlIgnore]
        public List<Widget> AllChildren;
        private List<Widget> RootFocusableChildren;
        private List<Widget> ActivableChildren
        {
            get
            {
                return this.RootFocusableChildren.Where(c => c.IsVisible).ToList();
            }
        }

        [XmlIgnore]
        public Object ViewModel { get; set; }

        public RootWindow()
            : base(null)
        {
            Top = 0;
            Left = 0;
            Width = Console.WindowWidth;
            Height = Console.WindowHeight;
            Background = ConsoleColor.DarkBlue;
            Foreground = ConsoleColor.White;
            SelectedBackground = ConsoleColor.Magenta;
            ActiveBackground = ConsoleColor.DarkMagenta;
            ActiveWidget = null;
            AllowDraw = false;
            this.AllChildren = new List<Widget>();
        }

        internal override void Render()
        {
            ConsoleHelper.DrawRectSolid(DisplayLeft, DisplayTop, Width, Height, Background);
        }

        internal bool AllowDraw { get; private set; }

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


        private Dictionary<String, Widget> NameLookup;

        private void BuildLookup()
        {
            this.NameLookup = new Dictionary<string, Widget>();
            this.AllChildren.ForEach(c =>
            {
                if (!String.IsNullOrEmpty(c.Id))
                {
                    this.NameLookup.Add(c.Id, c);
                }
            });
        }

        /// <summary>
        /// Find a widget by its ID.
        /// </summary>
        /// <param name="Id">The ID of the widget to search for.</param>
        /// <returns>A widget object if it is found, or null if no such widget exists.</returns>
        public Widget Find(string Id)
        {
            if (this.NameLookup == null) { BuildLookup(); }

            if (this.NameLookup.ContainsKey(Id))
            {
                return this.NameLookup[Id];
            }
            else
            {
                return null;
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
            AllowDraw = true;
            Console.CursorVisible = false;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            this.RootFocusableChildren = this.AllChildren.Where(c => c is IFocusable).OrderBy(c => c.TabStop).ToList();
            ActiveWidget = this.RootFocusableChildren.FirstOrDefault();

            //Draw();





            var queue = new System.Collections.Concurrent.ConcurrentQueue<ConsoleKeyInfo>();
            Task.Factory.StartNew(() =>
            {
                while (true)
                    queue.Enqueue(Console.ReadKey(true));
            }, TaskCreationOptions.LongRunning);
            //g.DrawRect(3, 3, 3, 3, ConsoleColor.Red, ConsoleColor.Red, UI.SpecialChars.Shade);
            Nito.AsyncEx.AsyncContext.Run(async () =>
            {
                var g = new UI.Graphics(RootWindow);
                //Console.BackgroundColor = ConsoleColor.Magenta;

                //ConsoleHelper.DrawBlockOutline(3, 3, 6, 6, ConsoleColor.Green);

                g.DrawLine(UI.Pen.SingelLine, ConsoleColor.Red, ConsoleColor.Black, (3, 3), (3, 5), (2, 5), (2, 4), (1, 4), (1, 6), (6, 6), (6, 4), (8, 4), (8, 3), (7, 3));
                g.Draw();
                Console.ReadKey(true);

                //Console.BackgroundColor = ConsoleColor.Black;
                //Console.Clear();
                //Console.BackgroundColor = ConsoleColor.Magenta;
                //Console.ForegroundColor= ConsoleColor.White;
                //for (char i = (char)0; i < char.MaxValue; i++)
                //{
                //    Console.Write((int)i);
                //    Console.Write(i);
                //    //Console.WriteLine();
                //    if (i % 512 == 0)
                //    {
                //        Console.BackgroundColor = ConsoleColor.Black;
                //        Console.Clear();
                //        Console.BackgroundColor = ConsoleColor.Magenta;
                //        Console.ForegroundColor = ConsoleColor.White;
                //    }
                //    await Task.Delay(200);
                //}
                //Console.ReadKey(false);

                var xPos = 3;
                var yPos = 3;
                while (true)
                {
                    while (queue.TryDequeue(out var k))
                    {
                        g.DrawRect(xPos, yPos, 3, 3, ConsoleColor.Black, ConsoleColor.Black, UI.SpecialChars.Fill);
                        switch (k.Key)
                        {
                            case ConsoleKey.LeftArrow:
                                xPos--;
                                break;
                            case ConsoleKey.UpArrow:
                                yPos--;
                                break;
                            case ConsoleKey.RightArrow:
                                xPos++;
                                break;
                            case ConsoleKey.DownArrow:
                                yPos++;
                                break;

                        }
                        g.DrawRect(xPos, yPos, 3, 3, ConsoleColor.Black, ConsoleColor.Red, UI.SpecialChars.Shade);

                    }
                    g.Draw();
                    await Task.Delay(50);
                }




                var childQueue = new Queue<Widget>(Children);
                while (childQueue.Count != 0)
                {
                    var item = childQueue.Dequeue();
                    item.InitilizeAsync();

                    foreach (var toEnqueue in item.Children)
                        childQueue.Enqueue(toEnqueue);
                }


                while (this.running)
                {
                    if (Width != Console.WindowWidth || Height != Console.WindowHeight)
                    {
                        Width = Console.WindowWidth;
                        Height = Console.WindowHeight;

                        Draw();
                    }


                    await Task.Delay(100);
                    while (this.running && queue.TryDequeue(out var k))
                    {
                        Console.CursorVisible = false;

                        bool ProcessKey = true;

                        if (ActiveWidget is IAcceptInput)
                        {
                            ProcessKey = false;
                            switch (k.Key)
                            {
                                case ConsoleKey.Tab:
                                    CycleFocus();
                                    break;
                                default:
                                    ProcessKey = HandleWidgetInput(k);
                                    break;
                            }
                        }

                        if (ProcessKey)
                        {
                            switch (k.Key)
                            {
                                case ConsoleKey.Tab:
                                    CycleFocus((k.Modifiers == ConsoleModifiers.Shift) ? -1 : 1);
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
                                case ConsoleKey.Spacebar:
                                case ConsoleKey.Enter:
                                    EnterPressed();
                                    break;
                                case ConsoleKey.Escape:
                                    this.running = false;
                                    break;
                            }
                        }
                    }
                }
            });

        }

        private bool HandleWidgetInput(ConsoleKeyInfo k)
        {
            return (ActiveWidget as IAcceptInput).Keypress(k);
        }

        private void MoveDown()
        {
            var w = FindFocusableWidgetBelow(ActiveWidget);
            if (w != null)
            {
                ActiveWidget = w;
                this.lastIndex = this.RootFocusableChildren.IndexOf(ActiveWidget);
            }
        }

        private void MoveUp()
        {
            var w = FindFocusableWidgetAbove(ActiveWidget);
            if (w != null)
            {
                ActiveWidget = w;
                this.lastIndex = this.RootFocusableChildren.IndexOf(ActiveWidget);
            }
        }

        private void MoveLeft()
        {
            var w = FindFocusableWidgetToLeftOf(ActiveWidget);
            if (w != null)
            {
                ActiveWidget = w;
                this.lastIndex = this.RootFocusableChildren.IndexOf(ActiveWidget);
            }
        }

        private void MoveRight()
        {
            var w = FindFocusableWidgetToRightOf(ActiveWidget);
            if (w != null)
            {
                ActiveWidget = w;
                this.lastIndex = this.RootFocusableChildren.IndexOf(ActiveWidget);
            }
        }

        private void EnterPressed()
        {
            if (ActiveWidget != null && ActiveWidget.Enabled)
            {
                ActiveWidget.FireClickedAsync();
            }
        }

        private int lastIndex = 0;

        private Widget FindFocusableWidgetToRightOf(Widget from)
        {
            var ImmediateRight = ActivableChildren.Where(c => c.Top == from.Top && c.Left > from.Left).OrderBy(c => c.Left).FirstOrDefault();

            if (ImmediateRight == null)
            {
                var RoughRight = ActivableChildren.Where(c => c.Left > from.Left && Math.Abs(c.Top - from.Top) < 2).OrderBy(c => c.Left).OrderBy(c => Math.Abs(c.Top - from.Top)).FirstOrDefault();

                if (RoughRight == null)
                {
                    return ActivableChildren.Where(c => c.Top == from.Top).OrderBy(c => c.Left).FirstOrDefault();
                }

                return RoughRight;
            }

            return ImmediateRight;
        }

        private Widget FindFocusableWidgetToLeftOf(Widget from)
        {
            var ImmediateLeft = ActivableChildren.Where(c => c.Top == from.Top && c.Left < from.Left).OrderByDescending(c => c.Left).FirstOrDefault();

            if (ImmediateLeft == null)
            {
                var RoughLeft = ActivableChildren.Where(c => c.Left < from.Left && Math.Abs(c.Top - from.Top) < 2).OrderByDescending(c => c.Left).OrderBy(c => Math.Abs(c.Top - from.Top)).FirstOrDefault();

                if (RoughLeft == null)
                {
                    return ActivableChildren.Where(c => c.Top == from.Top).OrderByDescending(c => c.Left).FirstOrDefault();
                }

                return RoughLeft;
            }

            return ImmediateLeft;
        }

        private Widget FindFocusableWidgetAbove(Widget from)
        {
            var ImmediateAbove = ActivableChildren.Where(c => c.Left == from.Left && c.Top < from.Top).OrderByDescending(c => c.Top).FirstOrDefault();

            if (ImmediateAbove == null)
            {
                return ActivableChildren.Where(c => c.Top < from.Top).OrderByDescending(c => c.Top).OrderBy(c => c.Left).FirstOrDefault();
            }

            return ImmediateAbove;
        }

        private Widget FindFocusableWidgetBelow(Widget from)
        {
            var ImmediateBelow = ActivableChildren.Where(c => c.Left == from.Left && c.Top > from.Top).OrderBy(c => c.Top).FirstOrDefault();

            if (ImmediateBelow == null)
            {
                return ActivableChildren.Where(c => c.Top > from.Top).OrderBy(c => c.Left).OrderBy(c => c.Top).FirstOrDefault();
            }

            return ImmediateBelow;
        }

        private void CycleFocus(int Direction = 1)
        {
            if (ActiveWidget == null)
            {
                this.lastIndex = 0;
                ActiveWidget = ActivableChildren.FirstOrDefault();
            }
            else
            {
                this.lastIndex = (this.lastIndex + Direction) % ActivableChildren.Count;
                if (this.lastIndex == -1) { this.lastIndex = ActivableChildren.Count - 1; }
                ActiveWidget = ActivableChildren[this.lastIndex];
            }
        }

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
    }
}
