using System.Drawing;
using GamemodesClientMenuFw.GmMenuFw.Menu.Base;

using Rectangle = CitizenFX.Core.UI.Rectangle;
using Text = CitizenFX.Core.UI.Text;
using Font = CitizenFX.Core.UI.Font;
using Alignment = CitizenFX.Core.UI.Alignment;
using GamemodesClientMenuFw.GmMenuFw.Item.Base;

namespace GamemodesClientMenuFw.GmMenuFw.Menu
{
    /// <summary>
    /// User menu with title bar
    /// </summary>
    public abstract class GmUserMenu : GmToggleableBaseMenu
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
        public GmUserMenu(string _title)
        {
            Title = _title;
        }

        /// <summary>
        /// Tick function to add menu items and run custom logic
        /// </summary>
        protected virtual void Tick()
        {

        }

        /// <summary>
        /// Sealed draw items function
        /// </summary>
        protected sealed override GmMenuBaseItem DrawItems()
        {
           return base.DrawItems();
        }

        /// <summary>
        /// Sealed menu tick function
        /// Override Tick instead of this method
        /// </summary>
        protected sealed override bool MenuTick()
        {
            if (!base.MenuTick())
            {
                return false;
            }

            Rectangle titleRect = new Rectangle(new PointF(PosX, PosY), new SizeF(m_itemWidth, m_headerHeight), m_headerColor, true);
            titleRect.Draw();

            Text titleText = new Text(Title,
                new PointF(PosX, PosY - m_headerHeight * 0.5f), 1f, Color.FromArgb(255, 255, 255), Font.Monospace, Alignment.Center, true, true);
            titleText.Draw();

            // Take title bar in consideration for Y position
            PosY += (int)(m_headerHeight - m_itemHeight * 0.5f);

            Tick();

            return true;
        }
    }
}
