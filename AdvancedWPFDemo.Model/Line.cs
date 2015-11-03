using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace _02350AdvancedDemo.Model
{
    [XmlInclude(typeof(DashLine))]
    public class Line
    {
        public int FromNumber { get; set; }
        public int ToNumber { get; set; }
        public string Label { get; set; }
    }
}
