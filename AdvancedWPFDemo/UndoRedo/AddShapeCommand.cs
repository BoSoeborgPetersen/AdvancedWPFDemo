using _02350AdvancedDemo.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02350AdvancedDemo.UndoRedo
{
    public class AddShapeCommand : IUndoRedoCommand
    {
        private ObservableCollection<ShapeViewModel> shapes;
        private ShapeViewModel shape;

        public AddShapeCommand(ObservableCollection<ShapeViewModel> _shapes, ShapeViewModel _shape) 
        { 
            shapes = _shapes;
            shape = _shape;
        }

        public void Execute()
        {
            shapes.Add(shape);
        }
        
        public void UnExecute()
        {
            shapes.Remove(shape);
        }
    }
}
