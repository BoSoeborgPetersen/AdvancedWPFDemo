using _02350AdvancedDemo.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02350AdvancedDemo.UndoRedo
{
    public class MoveShapesCommand : IUndoRedoCommand
    {
        private List<ShapeViewModel> shapes;
        
        private double offsetX;
        private double offsetY;
        
        public MoveShapesCommand(List<ShapeViewModel> _shapes, double _offsetX, double _offsetY) 
        {
            shapes = _shapes;
            offsetX = _offsetX;
            offsetY = _offsetY;
        }
        
        public void Execute()
        {
            foreach(var s in shapes)
            {
                s.CanvasCenterX += offsetX;
                s.CanvasCenterY += offsetY;
            }
        }
        
        public void UnExecute()
        {
            foreach (var s in shapes)
            {
                s.CanvasCenterX -= offsetX;
                s.CanvasCenterY -= offsetY;
            }
        }
    }
}
