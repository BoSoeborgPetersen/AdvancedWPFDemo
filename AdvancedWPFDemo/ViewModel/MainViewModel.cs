using _02350AdvancedDemo.Model;
using _02350AdvancedDemo.Serialization;
using _02350AdvancedDemo.UndoRedo;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace _02350AdvancedDemo.ViewModel
{
    public class MainViewModel : BaseViewModel
    {   
        private Point initialMousePosition;
        private Dictionary<int, Point> initialShapePositions = new Dictionary<int, Point>();

        private Point SelectionBoxStart;

        public double SelectionBoxX { get; set; }
        public double SelectionBoxY { get; set; }
        public double SelectionBoxWidth { get; set; }
        public double SelectionBoxHeight { get; set; }
        
        public ICommand MouseDownShapeCommand { get; }
        public ICommand MouseMoveShapeCommand { get; }
        public ICommand MouseUpShapeCommand { get; }

        public ICommand MouseDownCanvasCommand { get; }
        public ICommand MouseMoveCanvasCommand { get; }
        public ICommand MouseUpCanvasCommand { get; }

        public MainViewModel() : base()
        {
            Shapes = new ObservableCollection<ShapeViewModel>() {
                new CircleViewModel(new Circle() { X = 30, Y = 40, Width = 80, Height = 80, Data = new List<string> { "text1", "text2", "text3" } }),
                new SquareViewModel(new Square() { X = 140, Y = 230, Width = 200, Height = 100, Data = new List<string> { "text1", "text2", "text3" } })
            };

            Lines = new ObservableCollection<LineViewModel>() {
                new LineViewModel(new Line() {  Label = "Line Text" }) { From = Shapes[0], To = Shapes[1] }
            };
            
            MouseDownShapeCommand = new RelayCommand<MouseButtonEventArgs>(MouseDownShape);
            MouseMoveShapeCommand = new RelayCommand<MouseEventArgs>(MouseMoveShape);
            MouseUpShapeCommand = new RelayCommand<MouseButtonEventArgs>(MouseUpShape);

            MouseDownCanvasCommand = new RelayCommand<MouseButtonEventArgs>(MouseDownCanvas);
            MouseMoveCanvasCommand = new RelayCommand<MouseEventArgs>(MouseMoveCanvas);
            MouseUpCanvasCommand = new RelayCommand<MouseButtonEventArgs>(MouseUpCanvas);
        }

        private void MouseDownShape(MouseButtonEventArgs e)
        {
            if (!isAddingLine)
            {
                var shape = TargetShape(e);
                var mousePosition = RelativeMousePosition(e);

                initialMousePosition = mousePosition;

                var selectedShapes = Shapes.Where(x => x.IsMoveSelected);
                if (!selectedShapes.Any()) selectedShapes = new List<ShapeViewModel>() { TargetShape(e) };

                foreach (var s in selectedShapes)
                    initialShapePositions.Add(s.Number, new Point(s.CanvasCenterX, s.CanvasCenterY));

                e.MouseDevice.Target.CaptureMouse();
            }
            e.Handled = true;
        }

        private void MouseMoveShape(MouseEventArgs e)
        {
            if (Mouse.Captured != null && !isAddingLine)
            {
                var mousePosition = RelativeMousePosition(e);

                var selectedShapes = Shapes.Where(x => x.IsMoveSelected);
                if (!selectedShapes.Any()) selectedShapes = new List<ShapeViewModel>() { TargetShape(e) };
                
                foreach(var s in selectedShapes)
                {
                    var originalPosition = initialShapePositions[s.Number];
                    s.CanvasCenterX = (originalPosition.X + (mousePosition.X - initialMousePosition.X));
                    s.CanvasCenterY = (originalPosition.Y + (mousePosition.Y - initialMousePosition.Y));
                }

                e.Handled = true;
            }
        }

        private void MouseUpShape(MouseButtonEventArgs e)
        {
            if (isAddingLine)
            {
                var shape = TargetShape(e);

                if (addingLineFrom == null) { addingLineFrom = shape; addingLineFrom.IsSelected = true; }
                else if (addingLineFrom.Number != shape.Number)
                {
                    LineViewModel lineToAdd = new LineViewModel(
                        addingLineType == typeof(Line) ? new Line() :
                        new DashLine()
                    ){ From = addingLineFrom, To = shape };
                    undoRedoController.AddAndExecute(new AddLineCommand(Lines, lineToAdd));
                    addingLineFrom.IsSelected = false;
                    isAddingLine = false;
                    addingLineType = null;
                    addingLineFrom = null;
                    RaisePropertyChanged(() => ModeOpacity);
                }
            }
            else
            {
                var mousePosition = RelativeMousePosition(e);

                var selectedShapes = Shapes.Where(x => x.IsMoveSelected).ToList();
                if (!selectedShapes.Any()) selectedShapes = new List<ShapeViewModel>() { TargetShape(e) };

                foreach (var s in selectedShapes)
                {
                    var originalPosition = initialShapePositions[s.Number];
                    s.CanvasCenterX = originalPosition.X;
                    s.CanvasCenterY = originalPosition.Y;
                }
                undoRedoController.AddAndExecute(new MoveShapesCommand(selectedShapes, (mousePosition.X - initialMousePosition.X), (mousePosition.Y - initialMousePosition.Y)));

                initialMousePosition = new Point();
                initialShapePositions.Clear();
                e.MouseDevice.Target.ReleaseMouseCapture();
            }
            e.Handled = true;
        }

        private void MouseDownCanvas(MouseButtonEventArgs e)
        {
            if (!isAddingLine)
            {
                SelectionBoxStart = Mouse.GetPosition(e.MouseDevice.Target);
                e.MouseDevice.Target.CaptureMouse();
            }
        }

        private void MouseMoveCanvas(MouseEventArgs e)
        {
            if (Mouse.Captured != null && !isAddingLine)
            {
                var SelectionBoxNow = Mouse.GetPosition(e.MouseDevice.Target);
                SelectionBoxX = Math.Min(SelectionBoxStart.X, SelectionBoxNow.X);
                SelectionBoxY = Math.Min(SelectionBoxStart.Y, SelectionBoxNow.Y);
                SelectionBoxWidth = Math.Abs(SelectionBoxNow.X - SelectionBoxStart.X);
                SelectionBoxHeight = Math.Abs(SelectionBoxNow.Y - SelectionBoxStart.Y);
                RaisePropertyChanged(() => SelectionBoxX);
                RaisePropertyChanged(() => SelectionBoxY);
                RaisePropertyChanged(() => SelectionBoxWidth);
                RaisePropertyChanged(() => SelectionBoxHeight);
            }
        }

        private void MouseUpCanvas(MouseButtonEventArgs e)
        {
            if (!isAddingLine)
            {
                var SelectionBoxEnd = Mouse.GetPosition(e.MouseDevice.Target);
                var smallX = Math.Min(SelectionBoxStart.X, SelectionBoxEnd.X);
                var smallY = Math.Min(SelectionBoxStart.Y, SelectionBoxEnd.Y);
                var largeX = Math.Max(SelectionBoxStart.X, SelectionBoxEnd.X);
                var largeY = Math.Max(SelectionBoxStart.Y, SelectionBoxEnd.Y);
                foreach (var s in Shapes)
                    s.IsMoveSelected = s.CanvasCenterX > smallX && s.CanvasCenterX < largeX && s.CanvasCenterY > smallY && s.CanvasCenterY < largeY;

                SelectionBoxX = SelectionBoxY = SelectionBoxWidth = SelectionBoxHeight = 0;
                RaisePropertyChanged(() => SelectionBoxX);
                RaisePropertyChanged(() => SelectionBoxY);
                RaisePropertyChanged(() => SelectionBoxWidth);
                RaisePropertyChanged(() => SelectionBoxHeight);
                e.MouseDevice.Target.ReleaseMouseCapture();
            }
        }

        private ShapeViewModel TargetShape(MouseEventArgs e)
        {
            var shapeVisualElement = (FrameworkElement)e.MouseDevice.Target;
            return (ShapeViewModel)shapeVisualElement.DataContext;
        }

        private Point RelativeMousePosition(MouseEventArgs e)
        {
            var shapeVisualElement = (FrameworkElement)e.MouseDevice.Target;
            var canvas = FindParentOfType<Canvas>(shapeVisualElement);
            return Mouse.GetPosition(canvas);
        }

        private static T FindParentOfType<T>(DependencyObject o)
        {
            dynamic parent = VisualTreeHelper.GetParent(o);
            return parent.GetType().IsAssignableFrom(typeof(T)) ? parent : FindParentOfType<T>(parent);
        }
    }
}