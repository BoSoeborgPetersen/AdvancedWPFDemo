using _02350AdvancedDemo.Model;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace _02350AdvancedDemo.ViewModel
{
    public class LineViewModel : BaseViewModel
    {
        private ShapeViewModel from;
        private ShapeViewModel to;
        public Line Line { get; set; }
        public ShapeViewModel From { get { return from; } set { from = value; Line.FromNumber = value?.Number ?? 0; RaisePropertyChanged(); } }
        public ShapeViewModel To { get { return to; } set { to = value; Line.ToNumber = value?.Number ?? 0; RaisePropertyChanged(); } }
        public string Label { get { return Line.Label; } set { Line.Label = value; RaisePropertyChanged(); } }
        public DoubleCollection DashLength => Line is DashLine ? new DoubleCollection() { 2 }  : new DoubleCollection() { 1, 0 };

        public LineViewModel(Line _line) : base()
        {
            Line = _line;
        }
    }
}
