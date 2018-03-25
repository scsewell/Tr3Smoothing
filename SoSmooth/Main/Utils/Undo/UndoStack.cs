using System.Collections.Generic;

namespace SoSmooth
{
    /// <summary>
    /// Implements a global undo/redo stack.
    /// </summary>
    public class UndoStack : Singleton<UndoStack>
    {
        /// <summary>
        /// The maximum number of previous operations that can be undone.
        /// </summary>
        private const int MAX_HISTORTY_SIZE = 50;

        private readonly DropOutStack<Operation> m_undoStack;
        private readonly Stack<Operation> m_redoStack;

        public UndoStack()
        {
            m_undoStack = new DropOutStack<Operation>(MAX_HISTORTY_SIZE);
            m_redoStack = new Stack<Operation>(MAX_HISTORTY_SIZE);

            m_undoStack.ItemDropped += OnItemDropped;
        }

        /// <summary>
        /// Adds an operation to the undo stack.
        /// </summary>
        /// <param name="op">The operation.</param>
        public void AddOperation(Operation op)
        {
            // remember this operation
            m_undoStack.Push(op);

            // any undone operations can't be redone now
            while (m_redoStack.Count > 0)
            {
                m_redoStack.Pop().OnRedoClear();
            }
        }

        /// <summary>
        /// Undoes the most recent operation.
        /// </summary>
        public void Undo()
        {
            if (m_undoStack.Count > 0)
            {
                Operation op = m_undoStack.Pop();
                op.Unexcecute();
                m_redoStack.Push(op);
            }
        }

        /// <summary>
        /// Redoes the most recent operation.
        /// </summary>
        public void Redo()
        {
            if (m_redoStack.Count > 0)
            {
                Operation op = m_redoStack.Pop();
                op.Excecute();
                m_undoStack.Push(op);
            }
        }

        /// <summary>
        /// Called when an item is dropped from the bottom of the undo stack.
        /// </summary>
        /// <param name="item">The dropped item.</param>
        private void OnItemDropped(Operation item)
        {
            // the operation can no longer be undone
            item.OnUndoDropped();
        }
    }
}
