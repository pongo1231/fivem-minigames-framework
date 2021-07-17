using System.Drawing;

using Rectangle = CitizenFX.Core.UI.Rectangle;
using Text = CitizenFX.Core.UI.Text;
using Font = CitizenFX.Core.UI.Font;
using Alignment = CitizenFX.Core.UI.Alignment;

namespace GamemodesClientMenuBase.Menu
{
    /// <summary>
    /// User menu with title bar
    /// </summary>
    public class GamemodeTitledUserMenu : GamemodeToggleableBaseMenu
    {
        /// <summary>
        /// Label to display in title bar
        /// </summary>
        public string Title { get; private set; } = null;

        /// <summary>
        /// Height of title bar
        /// </summary>
        private readonly int m_headerHeight = 50;

        /// <summary>
        /// Color of title bar
        /// </summary>
        private readonly Color m_headerColor = Color.FromArgb(40, 40, 120);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_title">Label to show in title bar</param>
        public GamemodeTitledUserMenu(string _title)
        {
            Title = _title;
        }

        /// <summary>
        /// Update function
        /// </summary>
        /// <param name="_posX">Starting X position to use for menu</param>
        /// <param name="_posY">Starting Y position to use for menu</param>
        protected override void _Update(ref int _posX, ref int _posY)
        {
            int posX = m_posX;
            int posY = m_posY;

            Rectangle titleRect = new Rectangle(new PointF(posX, posY), new SizeF(m_itemWidth, m_headerHeight), m_headerColor, true);
            titleRect.Draw();

            Text titleText = new Text(Title,
                new PointF(posX, posY - m_headerHeight * 0.5f), 1f, Color.FromArgb(255, 255, 255), Font.Monospace, Alignment.Center, true, true);
            titleText.Draw();

            // Take title bar in consideration for Y position
            posY += (int)(m_headerHeight - m_itemHeight * 0.5f);

            _posX = posX;
            _posY = posY;
        }
    }
}
