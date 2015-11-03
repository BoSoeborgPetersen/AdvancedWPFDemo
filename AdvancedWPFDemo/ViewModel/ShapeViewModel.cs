using _02350AdvancedDemo.Model;
using _02350AdvancedDemo.UndoRedo;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace _02350AdvancedDemo.ViewModel
{
    public abstract class ShapeViewModel : BaseViewModel
    {
        public ICommand RemoveCommand { get; }

        public Shape Shape { get; set; }

        public int Number { get { return Shape.Number; } set { Shape.Number = value; RaisePropertyChanged(); } }
        public double X { get { return Shape.X; } set { Shape.X = value; RaisePropertyChanged(); RaisePropertyChanged(() => CanvasCenterX); } }
        public double Y { get { return Shape.Y; } set { Shape.Y = value; RaisePropertyChanged(); RaisePropertyChanged(() => CanvasCenterY); } }
        public double Width { get { return Shape.Width; } set { Shape.Width = value; RaisePropertyChanged(); RaisePropertyChanged(() => CanvasCenterX); RaisePropertyChanged(() => CenterX); } }
        public double Height { get { return Shape.Height; } set { Shape.Height = value; RaisePropertyChanged(); RaisePropertyChanged(() => CanvasCenterY); RaisePropertyChanged(() => CenterY); } }
        public List<string> Data { get { return Shape.Data; } set { Shape.Data = value; } }
        public double CanvasCenterX { get { return X + Width / 2; } set { X = value - Width / 2; RaisePropertyChanged(() => X); } }
        public double CanvasCenterY { get { return Y + Height / 2; } set { Y = value - Height / 2; RaisePropertyChanged(() => Y); } }
        public double CenterX => Width / 2;
        public double CenterY => Height / 2;
        private bool isSelected;
        public bool IsSelected { get { return isSelected; } set { isSelected = value; RaisePropertyChanged(); RaisePropertyChanged(() => SelectedColor); } }
        public Brush SelectedColor => IsSelected ? Brushes.Red : Brushes.Yellow;
        private bool isMoveSelected;
        public bool IsMoveSelected { get { return isMoveSelected; } set { isMoveSelected = value;  RaisePropertyChanged(); RaisePropertyChanged(() => BackgroundColor); } }
        public Brush BackgroundColor => IsMoveSelected ? Brushes.SkyBlue : Brushes.Navy;

        public ShapeViewModel(Shape _shape) : base()
        {
            Shape = _shape;

            RemoveCommand = new RelayCommand(Remove);
        }

        private void Remove()
        {
            undoRedoController.AddAndExecute(new RemoveShapesCommand(Shapes, Lines, new List<ShapeViewModel>() { this }));
        }

        public override string ToString() => Number.ToString();
    }
}
