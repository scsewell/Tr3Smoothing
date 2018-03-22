namespace SoSmooth
{
    /// <summary>
    /// Represents an action that can be done and undone on the undo stack.
    /// </summary>
    public abstract class Operation
    {
        public abstract void Excecute();
        public abstract void Unexcecute();
    }
}
