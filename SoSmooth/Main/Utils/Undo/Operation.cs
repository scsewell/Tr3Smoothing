namespace SoSmooth
{
    /// <summary>
    /// Represents an action that can be done and undone on the undo/redo stacks.
    /// </summary>
    public abstract class Operation
    {
        public abstract void Excecute();
        public abstract void Unexcecute();

        /// <summary>
        /// Called when the operation is no longer able to be undone.
        /// </summary>
        public virtual void OnUndoDropped()
        {
        }

        /// <summary>
        /// Called when the operation is cleared from the redo stack.
        /// </summary>
        public virtual void OnRedoClear()
        {
        }
    }
}
