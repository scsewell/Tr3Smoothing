using System;

namespace SoSmooth
{
    /// <summary>
    /// Implements a stack that will remove elements from the bottom of the stack after
    /// reaching a maximum size.
    /// </summary>
    /// <typeparam name="T">The type of element stored in the stack.</typeparam>
    public class DropOutStack<T>
    {
        private readonly T[] m_items;
        private int m_top = 0;
        private int m_count = 0;

        /// <summary>
        /// The number of elements in the stack.
        /// </summary>
        public int Count => m_count;

        /// <summary>
        /// Creates a new stack instance.
        /// </summary>
        /// <param name="capacity">The capacity of the stack.</param>
        public DropOutStack(int capacity)
        {
            m_items = new T[capacity];
        }

        /// <summary>
        /// Pushes an element onto the stack.
        /// </summary>
        /// <param name="item">The new element.</param>
        public void Push(T item)
        {
            m_items[m_top] = item;
            m_top = (m_top + 1) % m_items.Length;
            m_count = Math.Min(m_count + 1, m_items.Length);
        }

        /// <summary>
        /// removes an element from the stack.
        /// </summary>
        /// <returns>The removed element.</returns>
        public T Pop()
        {
            if (m_count > 0)
            {
                m_top = (m_items.Length + m_top - 1) % m_items.Length;
                m_count--; 
                T item = m_items[m_top];
                m_items[m_top] = default(T);
                return item;
            }
            else
            {
                throw new InvalidOperationException("Stack must contain an element to pop.");
            }
        }
    }
}
