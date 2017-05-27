namespace NSCI.UI
{
    public struct Size
    {
        //
        // Zusammenfassung:
        //     Initialisiert eine neue Instanz der Windows.Foundation.Size-Struktur und weist
        //     dieser eine ursprüngliche width und height zu.
        //
        // Parameter:
        //   width:
        //     Die Ausgangsbreite der Instanz von Windows.Foundation.Size.
        //
        //   height:
        //     Die Ausgangshöhe der Instanz von Windows.Foundation.Size.
        //
        // Ausnahmen:
        //   T:System.ArgumentException:
        //     width oder height ist kleiner als 0.
        public Size(double width, double height)
        {
            Width = width;
            Height = height;
        }

        //
        // Zusammenfassung:
        //     Ruft einen Wert ab, der eine statische leere Windows.Foundation.Size darstellt.
        //
        // Rückgabewerte:
        //     Eine leere Instanz von Windows.Foundation.Size.
        public static Size Empty { get; } = new Size(0, 0);
        //
        // Zusammenfassung:
        //     Ruft die Höhe dieser Instanz von Windows.Foundation.Size ab oder legt diese fest.
        //
        // Rückgabewerte:
        //     Windows.Foundation.Size.Height dieser Instanz von Windows.Foundation.Size in
        //     Pixel.Der Standard ist 0.Der Wert kann nicht negativ sein.
        //
        // Ausnahmen:
        //   T:System.ArgumentException:
        //     Es wurde ein Wert kleiner als 0 angegeben.
        public double Height { get; set; }
        //
        // Zusammenfassung:
        //     Ruft einen Wert ab, der angibt, ob diese Instanz von Windows.Foundation.Size
        //     gleich Windows.Foundation.Size.Empty ist.
        //
        // Rückgabewerte:
        //     true, wenn diese Instanz der Größe Windows.Foundation.Size.Empty ist, andernfalls
        //     false.
        public bool IsEmpty => Width == 0 && Height == 0;
        //
        // Zusammenfassung:
        //     Ruft die Breite dieser Instanz von Windows.Foundation.Size ab oder legt diese
        //     fest.
        //
        // Rückgabewerte:
        //     Windows.Foundation.Size.Width dieser Instanz von Windows.Foundation.Size in Pixel.Der
        //     Standardwert ist 0.Der Wert kann nicht negativ sein.
        //
        // Ausnahmen:
        //   T:System.ArgumentException:
        //     Es wurde ein Wert kleiner als 0 angegeben.
        public double Width { get; set; }

        //
        // Zusammenfassung:
        //     Vergleicht ein Objekt mit einer Instanz von Windows.Foundation.Size auf Gleichheit.
        //
        // Parameter:
        //   o:
        //     Das System.Object, das verglichen werden soll.
        //
        // Rückgabewerte:
        //     true, wenn die Größen gleich sind, andernfalls false.
        public override bool Equals(object o)
        {
            if (o is Size other)
                return Equals(other);
            return false;
        }
        //
        // Zusammenfassung:
        //     Vergleicht einen Wert mit einer Instanz von Windows.Foundation.Size auf Gleichheit.
        //
        // Parameter:
        //   value:
        //     Die Größe, die mit dieser aktuellen Instanz von Windows.Foundation.Size verglichen
        //     werden soll.
        //
        // Rückgabewerte:
        //     true, wenn die Instanzen von Windows.Foundation.Size gleich sind, andernfalls
        //     false.
        public bool Equals(Size other)
        {
            return Width == other.Width && Height == other.Height;
        }
        //
        // Zusammenfassung:
        //     Ruft den Hashcode für diese Instanz von Windows.Foundation.Size ab.
        //
        // Rückgabewerte:
        //     Der Hashcode für diese Instanz von Windows.Foundation.Size.
        public override int GetHashCode()
        {
            var hash = 13;
            hash = (hash * 31) ^ Width.GetHashCode();
            hash = (hash * 31) ^ Height.GetHashCode();
            return hash;

        }
        //
        // Zusammenfassung:
        //     Gibt eine Zeichenfolgendarstellung für diese Windows.Foundation.Size zurück.
        //
        // Rückgabewerte:
        //     Eine Zeichenfolgendarstellung für diese Windows.Foundation.Size.
        public override string ToString() => $"({Width}, {Height})";

        //
        // Zusammenfassung:
        //     Prüft zwei Instanzen von Windows.Foundation.Size auf Gleichheit.
        //
        // Parameter:
        //   size1:
        //     Die erste zu vergleichende Instanz von Windows.Foundation.Size.
        //
        //   size2:
        //     Die zweite zu vergleichende Instanz von Windows.Foundation.Size.
        //
        // Rückgabewerte:
        //     true, wenn die beiden Instanzen von Windows.Foundation.Size gleich sind, andernfalls
        //     false.
        public static bool operator ==(Size size1, Size size2) => size1.Equals(size2);
        //
        // Zusammenfassung:
        //     Vergleicht zwei Instanzen von Windows.Foundation.Size auf Ungleichheit.
        //
        // Parameter:
        //   size1:
        //     Die erste zu vergleichende Instanz von Windows.Foundation.Size.
        //
        //   size2:
        //     Die zweite zu vergleichende Instanz von Windows.Foundation.Size.
        //
        // Rückgabewerte:
        //     true, wenn die Instanzen von Windows.Foundation.Size ungleich sind, andernfalls
        //     false.
        public static bool operator !=(Size size1, Size size2) => !size1.Equals(size2);

    }
}