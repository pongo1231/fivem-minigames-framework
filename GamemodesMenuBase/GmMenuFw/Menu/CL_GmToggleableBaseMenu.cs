using CitizenFX.Core;
using CitizenFX.Core.Native;
using GamemodesClientMenuFw.GmMenuFw.Menu.Base;

namespace GamemodesClientMenuFw.GmMenuFw.Menu
{
    /// <summary>
    /// Menu base for user toggleable menus
    /// </summary>
    public abstract class GmToggleableBaseMenu : GmMenuAwareBaseMenu
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
                // Don't run if both values are equal to avoid recursion
                if (!value ^ !m_visible)
                {
                    m_visible = value;

                    // Close and play sound if both are false
                    if (!value)
                    {
                        Close();

                        API.PlaySoundFrontend(-1, "NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET", false);
                    }
                }
            }
        }

        /// <summary>
        /// Parent menu
        /// </summary>
        protected override GmMenuAwareBaseMenu ParentMenu
        {
            get
            {
                return m_parentMenu;
            }
            set
            {
                m_parentMenu = value;

                // Force visibility on if parent is set
                if (m_parentMenu != null)
                {
                    Visible = true;
                }
            }
        }

        /// <summary>
        /// Parent menu
        /// </summary>
        protected GmMenuAwareBaseMenu m_parentMenu = null;

        /// <summary>
        /// Whether menu is visible
        /// </summary>
        private bool m_visible = false;

        /// <summary>
        /// Close the menu
        /// </summary>
        public override void Close()
        {
            base.Close();

            // Reset visibility state
            Visible = false;
        }

        /// <summary>
        /// Menu tick function
        /// </summary>
        protected override bool MenuTick()
        {
            if (!base.MenuTick())
            {
                return false;
            }

            if (Visible && Game.IsControlJustPressed(0, Control.FrontendRright))
            {
                Close();
            }

            if (!Visible)
            {
                return false;
            }

            return true;
        }
    }
}
