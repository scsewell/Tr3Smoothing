using System.Collections.Generic;

namespace SoSmooth
{
    /// <summary>
    /// Implements a global undo stack.
    /// </summary>
    public class UndoStack : Singleton<UndoStack>
    {
        /// <summary>
        /// The maximum number of previous operations that can be undone.
        /// </summary>
        private const int MAX_HISTORTY_SIZE = 50;

        private readonly DropOutStack<Operation> m_history = new DropOutStack<Operation>(MAX_HISTORTY_SIZE);
        private readonly Stack<Operation> m_undone = new Stack<Operation>();

        /// <summary>
        /// Adds an operation to the undo stack.
        /// </summary>
        /// <param name="op">The operation.</param>
        public void AddOperation(Operation op)
        {
            // remember this operation
            m_history.Push(op);
            // any undone operations are forgotten
            m_undone.Clear();
        }

        /// <summary>
        /// Undoes the most recent operation.
        /// </summary>
        public void Undo()
        {
            if (m_history.Count > 0)
            {
                Operation op = m_history.Pop();
                op.Unexcecute();
                m_undone.Push(op);
            }
        }

        /// <summary>
        /// Redoes the most recent operation.
        /// </summary>
        public void Redo()
        {
            if (m_undone.Count > 0)
            {
                Operation op = m_undone.Pop();
                op.Excecute();
                m_history.Push(op);
            }
        }
    }
}
