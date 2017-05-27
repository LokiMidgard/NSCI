namespace NSCI.UI
{
    public struct Thickness
    {
        //
        // Zusammenfassung:
        //     Initialisiert eine Windows.UI.Xaml.Thickness-Struktur, die die angegebene einheitliche
        //     Länge auf jeder Seite aufweist.
        //
        // Parameter:
        //   uniformLength:
        //     Die einheitliche Länge, die auf alle vier Seiten des umgebenden Rechtecks angewendet
        //     wird.
        public Thickness(double uniformLength) : this(uniformLength, uniformLength, uniformLength, uniformLength) { }
        //
        // Zusammenfassung:
        //     Initialisiert eine Windows.UI.Xaml.Thickness-Struktur, bei der bestimmte Längen
        //     (als System.double angegeben) auf alle Seiten des Rechtecks angewendet werden.
        //
        // Parameter:
        //   left:
        //     Die Stärke für den linken Rand des Rechtecks.
        //
        //   top:
        //     Die Stärke für den oberen Rand des Rechtecks.
        //
        //   right:
        //     Die Stärke für den rechten Rand des Rechtecks.
        //
        //   bottom:
        //     Die Stärke für den unteren Rand des Rechtecks.
        public Thickness(double left, double top, double right, double bottom)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }

        //
        // Zusammenfassung:
        //     Ruft die Breite, in Pixeln, des unteren Rands des umgebenden Rechtecks ab oder
        //     legt sie fest.
        //
        // Rückgabewerte:
        //     Ein System.double, der die Breite, in Pixeln, des unteren Rands des umgebenden
        //     Rechtecks für diese Instanz von Windows.UI.Xaml.Thickness darstellt.Der Standard
        //     ist 0.
        public double Bottom { get; set; }
        //
        // Zusammenfassung:
        //     Ruft die Breite, in Pixeln, des linken Rands des umgebenden Rechtecks ab oder
        //     legt sie fest.
        //
        // Rückgabewerte:
        //     Ein System.double, das die Breite, in Pixeln, des linken Rands des umgebenden
        //     Rechtecks für diese Instanz von Windows.UI.Xaml.Thickness darstellt.Der Standard
        //     ist 0.
        public double Left { get; set; }
        //
        // Zusammenfassung:
        //     Ruft die Breite, in Pixeln, des rechten Rands des umgebenden Rechtecks ab oder
        //     legt sie fest.
        //
        // Rückgabewerte:
        //     Ein System.double, der die Breite, in Pixeln, des rechten Rands des umgebenden
        //     Rechtecks für diese Instanz von Windows.UI.Xaml.Thickness darstellt.Der Standard
        //     ist 0.
        public double Right { get; set; }
        //
        // Zusammenfassung:
        //     Ruft die Breite, in Pixeln, des oberen Rands des umgebenden Rechtecks ab oder
        //     legt sie fest.
        //
        // Rückgabewerte:
        //     Ein System.double, der die Breite, in Pixeln, des oberen Rands des umgebenden
        //     Rechtecks für diese Instanz von Windows.UI.Xaml.Thickness darstellt.Der Standard
        //     ist 0.
        public double Top { get; set; }

        //
        // Zusammenfassung:
        //     Vergleicht diese Windows.UI.Xaml.Thickness-Struktur mit einem anderen System.Object
        //     auf Gleichheit.
        //
        // Parameter:
        //   obj:
        //     Das zu vergleichende Objekt.
        //
        // Rückgabewerte:
        //     true, wenn die beiden Objekte gleich sind, andernfalls false.
        public override bool Equals(object obj)
        {
            if (obj is Thickness other)
                return Equals(other);
            return false;
        }
        //
        // Zusammenfassung:
        //     Vergleicht diese Windows.UI.Xaml.Thickness-Struktur mit einer anderen Windows.UI.Xaml.Thickness-Struktur
        //     auf Gleichheit.
        //
        // Parameter:
        //   thickness:
        //     Eine Instanz von Windows.UI.Xaml.Thickness, die auf Gleichheit überprüft werden
        //     soll.
        //
        // Rückgabewerte:
        //     true, wenn die beiden Instanzen von Windows.UI.Xaml.Thickness gleich sind, andernfalls
        //     false.
        public bool Equals(Thickness other)
        {
            return Bottom == other.Bottom && Left == other.Left && Right == other.Right && Top == other.Top;
        }
        //
        // Zusammenfassung:
        //     Gibt den Hashcode der Struktur zurück.
        //
        // Rückgabewerte:
        //     Ein Hashcode für diese Instanz von Windows.UI.Xaml.Thickness.
        public override int GetHashCode()
        {
            var hash = 13;
            hash = (hash * 31) ^ Bottom.GetHashCode();
            hash = (hash * 31) ^ Left.GetHashCode();
            hash = (hash * 31) ^ Right.GetHashCode();
            hash = (hash * 31) ^ Top.GetHashCode();
            return hash;
        }
        //
        // Zusammenfassung:
        //     Gibt die Zeichenfolgendarstellung der Windows.UI.Xaml.Thickness-Struktur zurück.
        //
        // Rückgabewerte:
        //     Ein System.String, der den Windows.UI.Xaml.Thickness-Wert darstellt.
        public override string ToString() => $"({Top}, {Left}, {Bottom}, {Right})";

        //
        // Zusammenfassung:
        //     Vergleicht den Wert von zweier Windows.UI.Xaml.Thickness-Strukturen auf Gleichheit.
        //
        // Parameter:
        //   t1:
        //     Die erste zu vergleichende Struktur.
        //
        //   t2:
        //     Die andere zu vergleichende Struktur.
        //
        // Rückgabewerte:
        //     true, wenn die beiden Instanzen von Windows.UI.Xaml.Thickness gleich sind, andernfalls
        //     false.
        public static bool operator ==(Thickness t1, Thickness t2) => t1.Equals(t2);
        //
        // Zusammenfassung:
        //     Vergleicht zwei Windows.UI.Xaml.Thickness-Strukturen auf Ungleichheit.
        //
        // Parameter:
        //   t1:
        //     Die erste zu vergleichende Struktur.
        //
        //   t2:
        //     Die andere zu vergleichende Struktur.
        //
        // Rückgabewerte:
        //     true, wenn die beiden Instanzen von Windows.UI.Xaml.Thickness ungleich sind,
        //     andernfalls false.
        public static bool operator !=(Thickness t1, Thickness t2) => !t1.Equals(t2);

    }
}