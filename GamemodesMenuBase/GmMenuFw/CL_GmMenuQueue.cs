namespace GamemodesClientMenuFw.GmMenuFw
{
    /// <summary>
    /// Queue data type
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    public class GmMenuQueue<T>
    {
        /// <summary>
        /// Next node, null if not existing
        /// </summary>
        public GmMenuQueue<T> Next { get; private set; } = null;

        /// <summary>
        /// Content of this node
        /// </summary>
        public T Content { get; private set; } = default(T);

        /// <summary>
        /// Queue size
        /// </summary>
        public int Count { get; private set; } = 0;

        /// <summary>
        /// Enqueue a new item
        /// </summary>
        /// <param name="_item">Item to enqueue</param>
        public void Enqueue(T _item)
        {
            // Not the last node?
            if (Next != null)
            {
                Next.Enqueue(_item);
            }
            // Last node?
            else if (Count > 0)
            {
                GmMenuQueue<T> newNext = new GmMenuQueue<T>();
                newNext.Content = _item;
                newNext.Count = 1;

                Next = newNext;
            }
            // Uninitialized last node?
            else
            {
                Content = _item;
            }

            Count++;
        }

        /// <summary>
        /// Dequeue this node
        /// </summary>
        /// <returns>Content of this node</returns>
        public T Dequeue()
        {
            T content = Content;

            Content = Next != null ? Next.Content : default(T);
            Count = Next != null ? Next.Count : 0;
            Next = Next != null ? Next.Next : null;

            return content;
        }
    }
}
