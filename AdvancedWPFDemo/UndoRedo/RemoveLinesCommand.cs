using _02350AdvancedDemo.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02350AdvancedDemo.UndoRedo
{
    public class RemoveLinesCommand : IUndoRedoCommand
    {
        private ObservableCollection<LineViewModel> lines;
        
        private List<LineViewModel> linesToRemove;
        
        public RemoveLinesCommand(ObservableCollection<LineViewModel> _lines, List<LineViewModel> _linesToRemove) 
        {
            lines = _lines;
            linesToRemove = _linesToRemove;
        }
        
        public void Execute()
        {
            linesToRemove.ForEach(x => lines.Remove(x));
        }
        
        public void UnExecute()
        {
            linesToRemove.ForEach(x => lines.Add(x));
        }
    }
}
