using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02350AdvancedDemo.UndoRedo
{
    public class UndoRedoController
    {
        public static UndoRedoController Instance { get; } = new UndoRedoController();

        private readonly Stack<IUndoRedoCommand> undoStack = new Stack<IUndoRedoCommand>();
        private readonly Stack<IUndoRedoCommand> redoStack = new Stack<IUndoRedoCommand>();

        private UndoRedoController() { }

        public void AddAndExecute(IUndoRedoCommand command)
        {
            undoStack.Push(command);
            redoStack.Clear();
            command.Execute();
        }

        public bool CanUndo(string steps) => undoStack.Count() >= (steps == null ? 1 : int.Parse(steps));

        public void Undo(string steps)
        {
            if (!CanUndo(steps)) throw new InvalidOperationException();
            int s = steps == null ? 1 : int.Parse(steps);
            for (int i = 0; i < s; i++)
            {
                IUndoRedoCommand command = undoStack.Pop();
                redoStack.Push(command);
                command.UnExecute();
            }
        }

        public bool CanRedo(string steps) => redoStack.Count() >= (steps == null ? 1 : int.Parse(steps));

        public void Redo(string steps)
        {
            if (!CanRedo(steps)) throw new InvalidOperationException();
            int s = steps == null ? 1 : int.Parse(steps);
            for(int i = 0; i < s; i++)
            {
                IUndoRedoCommand command = redoStack.Pop();
                undoStack.Push(command);
                command.Execute();
            }
        }
    }
}
