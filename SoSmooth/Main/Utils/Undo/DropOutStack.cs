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
        /// The number of items in the stack.
        /// </summary>
        public int Count => m_count;

        public delegate void ItemDroppedHandler(T item);

        /// <summary>
        /// Triggered when an item is dropped from the bottom of the stack.
        /// </summary>
        public event ItemDroppedHandler ItemDropped;

        /// <summary>
        /// Creates a new stack instance.
        /// </summary>
        /// <param name="capacity">The capacity of the stack.</param>
        public DropOutStack(int capacity)
        {
            m_items = new T[capacity];
        }

        /// <summary>
        /// Pushes an item onto the stack.
        /// </summary>
        /// <param name="item">The new item.</param>
        public void Push(T item)
        {
            // if the stack is full notify that the bottom item is being dropped.
            if (m_count == m_items.Length)
            {
                ItemDropped?.Invoke(m_items[m_top]);
            }
            else
            {
                m_count++;
            }

            m_items[m_top] = item;
            m_top = (m_top + 1) % m_items.Length;
        }

        /// <summary>
        /// Removes an item from the stack.
        /// </summary>
        /// <returns>The removed item.</returns>
        public T Pop()
        {
            if (m_count > 0)
            {
                m_count--;
                m_top = (m_items.Length + m_top - 1) % m_items.Length;
                T item = m_items[m_top];
                m_items[m_top] = default(T);
                return item;
            }
            else
            {
                throw new InvalidOperationException("Stack must contain an item to pop.");
            }
        }
    }
}
