using System.Drawing;

using Rectangle = CitizenFX.Core.UI.Rectangle;

namespace GamemodesClientMenuFw.GmMenuFw.Item.Base
{
    /// <summary>
    /// Menu item base
    /// </summary>
    public abstract class GmMenuBaseItem
    {
        /// <summary>
        /// X position
        /// </summary>
        public int X { get; set; } = 0;

        /// <summary>
        /// Y position
        /// </summary>
        public int Y { get; set; } = 0;

        /// <summary>
        /// Rect color
        /// </summary>
        public Color Color { get; set; } = Color.FromArgb(255, 255, 0, 255);

        /// <summary>
        /// Size of rectangle
        /// </summary>
        protected SizeF RectSize
        {
            get
            {
                return m_rect.Size;
            }
            set
            {
                m_rect.Size = value;
            }
        }

        /// <summary>
        /// Rectangle to draw
        /// </summary>
        private Rectangle m_rect = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_width">Width</param>
        /// <param name="_height">Height</param>
        public GmMenuBaseItem(int _width, int _height)
        {
            m_rect = new Rectangle(new PointF(), new SizeF(_width, _height), Color, true);
        }

        /// <summary>
        /// Handle drawing of all elements of this item
        /// </summary>
        public virtual void Draw()
        {
            m_rect.Position = new PointF(X, Y);
            m_rect.Color = Color;

            m_rect.Draw();
        }
    }
}
