using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace _02350AdvancedDemo.Model
{
    [XmlInclude(typeof(Circle))]
    [XmlInclude(typeof(Square))]
    public abstract class Shape
    {
        private static int counter = 0;
        public int Number { get; set; } = ++counter;

        public double X { get; set; } = 200;
        public double Y { get; set; } = 200;
        public double Width { get; set; } = 100;
        public double Height { get; set; } = 100;

        public List<string> Data { get; set; }

        public void NewNumber()
        {
            Number = ++counter;
        }

        public override string ToString() => Number.ToString();
    }
}
