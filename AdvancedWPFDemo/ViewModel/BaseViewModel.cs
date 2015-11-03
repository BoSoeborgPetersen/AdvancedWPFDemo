using _02350AdvancedDemo.Model;
using _02350AdvancedDemo.Serialization;
using _02350AdvancedDemo.UndoRedo;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace _02350AdvancedDemo.ViewModel
{
    public abstract class BaseViewModel : ViewModelBase
    {
        protected UndoRedoController undoRedoController = UndoRedoController.Instance;
        [Dependency]
        public DialogViews dialogVM { get; set; }

        protected static bool isAddingLine;
        protected static Type addingLineType;
        protected static ShapeViewModel addingLineFrom;
        public double ModeOpacity => isAddingLine ? 0.4 : 1.0;

        public static ObservableCollection<ShapeViewModel> Shapes { get; set; }
        public static ObservableCollection<LineViewModel> Lines { get; set; }

        public ICommand NewDiagramCommand { get; }
        public ICommand OpenDiagramCommand { get; }
        public ICommand SaveDiagramCommand { get; }

        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }

        public ICommand CutCommand { get; set; }
        public ICommand CopyCommand { get; set; }
        public ICommand PasteCommand { get; set; }

        public ICommand ExitCommand { get; set; }

        public ICommand AddCircleCommand { get; }
        public ICommand AddSquareCommand { get; }
        public ICommand AddLineCommand { get; }
        public ICommand AddDashLineCommand { get; }
        public ICommand RemoveShapesCommand { get; }
        public ICommand RemoveLinesCommand { get; }

        public BaseViewModel()
        {
            NewDiagramCommand = new RelayCommand(NewDiagram);
            OpenDiagramCommand = new RelayCommand(OpenDiagram);
            SaveDiagramCommand = new RelayCommand(SaveDiagram);

            UndoCommand = new RelayCommand<string>(undoRedoController.Undo, undoRedoController.CanUndo);
            RedoCommand = new RelayCommand<string>(undoRedoController.Redo, undoRedoController.CanRedo);

            CutCommand = new RelayCommand(Cut);
            CopyCommand = new RelayCommand(Copy);
            PasteCommand = new RelayCommand(Paste);

            ExitCommand = new RelayCommand(Exit);

            AddCircleCommand = new RelayCommand(AddCircle);
            AddSquareCommand = new RelayCommand(AddSquare);
            AddLineCommand = new RelayCommand(AddLine);
            AddDashLineCommand = new RelayCommand(AddDashLine);
            RemoveShapesCommand = new RelayCommand<IList>(RemoveShapes, CanRemoveShapes);
            RemoveLinesCommand = new RelayCommand<IList>(RemoveLines, CanRemoveLines);
        }

        private void NewDiagram()
        {
            if (dialogVM.ShowNew())
            {
                Shapes.Clear();
                Lines.Clear();
            }
        }

        private async void OpenDiagram()
        {
            string path = dialogVM.ShowOpen();
            if (path != null)
            {
                Diagram diagram = await SerializerXML.Instance.AsyncDeserializeFromFile(path);

                Shapes.Clear();
                diagram.Shapes.Select(x => x is Circle ? (ShapeViewModel)new CircleViewModel(x) : new SquareViewModel(x)).ToList().ForEach(x => Shapes.Add(x));
                Lines.Clear();
                diagram.Lines.Select(x => new LineViewModel(x)).ToList().ForEach(x => Lines.Add(x));

                // Reconstruct object graph.
                foreach (LineViewModel line in Lines)
                {
                    line.From = Shapes.Single(s => s.Number == line.Line.FromNumber);
                    line.To = Shapes.Single(s => s.Number == line.Line.ToNumber);
                }
            }
        }

        private void SaveDiagram()
        {
            string path = dialogVM.ShowSave();
            if (path != null)
            {
                Diagram diagram = new Diagram() { Shapes = Shapes.Select(x => x.Shape).ToList(), Lines = Lines.Select(x => x.Line).ToList() };
                SerializerXML.Instance.AsyncSerializeToFile(diagram, path);
            }
        }

        private async void Cut()
        {
            var selectedShapes = Shapes.Where(x => x.IsMoveSelected).ToList();
            var selectedLines = Lines.Where(x => x.From.IsMoveSelected || x.To.IsMoveSelected).ToList();

            undoRedoController.AddAndExecute(new RemoveShapesCommand(Shapes, Lines, selectedShapes));

            Diagram diagram = new Diagram() { Shapes = selectedShapes.Select(x => x.Shape).ToList(), Lines = selectedLines.Select(x => x.Line).ToList() };

            var xml = await SerializerXML.Instance.AsyncSerializeToString(diagram);

            Clipboard.SetText(xml);
        }

        private async void Copy()
        {
            var selectedShapes = Shapes.Where(x => x.IsMoveSelected).ToList();
            var selectedLines = Lines.Where(x => x.From.IsMoveSelected || x.To.IsMoveSelected).ToList();

            Diagram diagram = new Diagram() { Shapes = selectedShapes.Select(x => x.Shape).ToList(), Lines = selectedLines.Select(x => x.Line).ToList() };

            var xml = await SerializerXML.Instance.AsyncSerializeToString(diagram);

            Clipboard.SetText(xml);
        }

        private async void Paste()
        {
            var xml = Clipboard.GetText();

            var diagram = await SerializerXML.Instance.AsyncDeserializeFromString(xml);

            var shapes = diagram.Shapes;
            var lines = diagram.Lines;

            // Unselect existing shapes.
            foreach (var s in Shapes)
                s.IsMoveSelected = false;

            // Change numbers for shapes if necessary and move them a little.
            foreach (var s in shapes)
                if (Shapes.Any(x => x.Number == s.Number))
                {
                    var oldNumber = s.Number;
                    s.NewNumber();
                    s.X += 50;
                    s.Y += 50;

                    // change referenced number for lines.
                    foreach (var l in lines.Where(x => x.FromNumber == oldNumber))
                        l.FromNumber = s.Number;
                    foreach (var l in lines.Where(x => x.ToNumber == oldNumber))
                        l.ToNumber = s.Number;
                }

            // Add shapes and lines (TODO: Should use undo/redo command).
            //undoRedoController.AddAndExecute(new AddShapeCommand(Shapes, new CircleViewModel(new Circle())));
            //undoRedoController.AddAndExecute(new AddLineCommand(Lines, lineToAdd));
            shapes.ForEach(s => Shapes.Add(s is Circle ? (ShapeViewModel)new CircleViewModel(s) { IsMoveSelected = true } : new SquareViewModel(s) { IsMoveSelected = true }));
            lines.ForEach(l => Lines.Add(new LineViewModel(l)));

            // Reconstruct object graph.
            foreach (LineViewModel line in Lines)
            {
                line.From = Shapes.Single(s => s.Number == line.Line.FromNumber);
                line.To = Shapes.Single(s => s.Number == line.Line.ToNumber);
            }
        }

        private void Exit()
        {
            Application.Current.Shutdown();
        }

        private void AddCircle()
        {
            undoRedoController.AddAndExecute(new AddShapeCommand(Shapes, new CircleViewModel(new Circle())));
        }

        private void AddSquare()
        {
            undoRedoController.AddAndExecute(new AddShapeCommand(Shapes, new SquareViewModel(new Square())));
        }

        private void AddLine()
        {
            isAddingLine = true;
            addingLineType = typeof(Line);
            RaisePropertyChanged(() => ModeOpacity);
        }

        private void AddDashLine()
        {
            isAddingLine = true;
            addingLineType = typeof(DashLine);
            RaisePropertyChanged(() => ModeOpacity);
        }

        private bool CanRemoveShapes(IList _shapes) => _shapes.Count == 1;

        private void RemoveShapes(IList _shapes)
        {
            undoRedoController.AddAndExecute(new RemoveShapesCommand(Shapes, Lines, _shapes.Cast<ShapeViewModel>().ToList()));
        }

        private bool CanRemoveLines(IList _edges) => _edges.Count >= 1;

        private void RemoveLines(IList _lines)
        {
            undoRedoController.AddAndExecute(new RemoveLinesCommand(Lines, _lines.Cast<LineViewModel>().ToList()));
        }
    }
}
