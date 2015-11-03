using _02350AdvancedDemo.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02350AdvancedDemo.UndoRedo
{
    public class AddLineCommand : IUndoRedoCommand
    {
        private ObservableCollection<LineViewModel> lines;
        private LineViewModel line;
        
        public AddLineCommand(ObservableCollection<LineViewModel> _lines, LineViewModel _line) 
        { 
            lines = _lines;
            line = _line;
        }
        
        public void Execute()
        {
            lines.Add(line);
        }
        
        public void UnExecute()
        {
            lines.Remove(line);
        }
    }
}
