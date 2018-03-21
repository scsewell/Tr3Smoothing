namespace SoSmooth
{
    /// <summary>
    /// Represents an action that can be done and undone.
    /// </summary>
    public abstract class Operation
    {
        /// <summary>
        /// Creates a new operation instance, and adding
        /// the operation to the undo stack.
        /// </summary>
        public Operation()
        {
            UndoStack.Instance.AddOperation(this);
        }

        public abstract void Excecute();
        public abstract void Unexcecute();
    }
}
