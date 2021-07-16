using System.Drawing;

using Rectangle = CitizenFX.Core.UI.Rectangle;
using Text = CitizenFX.Core.UI.Text;
using Font = CitizenFX.Core.UI.Font;
using Alignment = CitizenFX.Core.UI.Alignment;

namespace GamemodesClientMenuBase.Menu
{
    /// <summary>
    /// Menu item base
    /// </summary>
    public class GamemodeMenuItem
    {
        /// <summary>
        /// On Item Click action
        /// </summary>
        /// <param name="_idx">Index of selected item</param>
        /// <param name="_label">Label of selected item</param>
        public delegate void GamemodeMenuItemClick(int _idx, string _label);

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
        /// On Click action
        /// </summary>
        public GamemodeMenuItemClick OnClick { get; private set; } = null;

        /// <summary>
        /// Label to show
        /// </summary>
        public string Label { get; private set; } = null;

        /// <summary>
        /// Rectangle to draw
        /// </summary>
        private Rectangle m_rect = null;

        /// <summary>
        /// Text to draw
        /// </summary>
        private Text m_text = null;

        /// <summary>
        /// Width of rect
        /// </summary>
        private int m_rectWidth = 0;

        /// <summary>
        /// Height of rect
        /// </summary>
        private int m_rectHeight = 0;

        /// <summary>
        /// Creates a new item
        /// </summary>
        /// <param name="_width">Width</param>
        /// <param name="_height">Height</param>
        /// <param name="_label">Label of item</param>
        /// <param name="_onClickAction">Action to execute on click</param>
        public GamemodeMenuItem(int _width, int _height, string _label, GamemodeMenuItemClick _onClickAction = null)
        {
            m_rect = new Rectangle(new PointF(), new SizeF(_width, _height), Color, true);

            m_text = new Text(_label, new PointF(), 0.35f, Color.FromArgb(255, 255, 255, 255), Font.ChaletLondon, Alignment.Left, false, false);

            m_rectWidth = _width;

            m_rectHeight = _height;

            Label = _label;

            OnClick = _onClickAction;
        }

        /// <summary>
        /// Draw this item
        /// </summary>
        public void Draw()
        {
            // Assign rect pos
            m_rect.Position = new PointF(X, Y);

            // Assign rect color
            m_rect.Color = Color;

            // Assign text pos
            m_text.Position = new PointF(X - m_rectWidth * 0.5f + 5, Y - m_rectHeight * 0.5f);

            // Draaaaaaw
            m_rect.Draw();
            m_text.Draw();
        }
    }
}
