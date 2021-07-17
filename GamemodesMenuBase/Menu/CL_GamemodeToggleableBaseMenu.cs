using CitizenFX.Core;
using CitizenFX.Core.Native;

using static GamemodesClientMenuBase.Menu.GamemodeMenuItem;

namespace GamemodesClientMenuBase.Menu
{
    /// <summary>
    /// Menu base for toggleable menus
    /// </summary>
    public abstract class GamemodeToggleableBaseMenu : GamemodeBaseMenu
    {
        /// <summary>
        /// Whether the menu is / should be visible
        /// </summary>
        public bool Visible
        {
            get
            {
                return m_visible;
            }
            set
            {
                m_visible = value;

                API.PlaySoundFrontend(-1, "NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET", false);
            }
        }

        /// <summary>
        /// Whether menu toggle key was pressed this frame
        /// </summary>
        public bool ShouldToggle { get; private set; } = false;

        /// <summary>
        /// Whether menu is visible
        /// </summary>
        private bool m_visible = false;

        /// <summary>
        /// Add a simple labeled menu item with an optional callback
        /// </summary>
        /// <param name="_label">Label to display</param>
        /// <param name="_onClick">Callback to invoke on click</param>
        public void AddLabelItem(string _label, GamemodeMenuItemClick _onClick = null)
        {
            m_menuItems.Enqueue(new GamemodeMenuItem(m_itemWidth, m_itemHeight, _label, _onClick));
        }

        /// <summary>
        /// Draw menu items and handle inputs if menu is visible, doesn't do anything otherwise
        /// </summary>
        /// <param name="_posX">Do not use</param>
        /// <param name="_posY">Do not use</param>
        public override void Update(int _posX = m_posX, int _posY = m_posY)
        {
            ShouldToggle = Game.IsControlJustPressed(0, Control.InteractionMenu)
                || (Visible && Game.IsControlJustPressed(0, Control.FrontendRright));

            if (!Visible)
            {
                return;
            }

            _Update(ref _posX, ref _posY);

            base.Update(_posX, _posY);
        }

        /// <summary>
        /// Update function to call if menu is visible and Update was called
        /// </summary>
        /// <param name="_posX">Starting X position to use for menu</param>
        /// <param name="_posY">Starting Y position to use for menu</param>
        protected abstract void _Update(ref int _posX, ref int _posY);
    }
}
