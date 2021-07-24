using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace GamemodesClientMenuFw.GmMenuFw.Menu.Base
{
    /// <summary>
    /// Menu base for user toggleable menus
    /// </summary>
    public abstract class GmToggleableBaseMenu : GmMenuAwareBaseMenu
    {
        /// <summary>
        /// Whether the menu is / should be visible
        /// </summary>
        public virtual bool Visible
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

                        PlayCloseSound();
                    }

                    // Propagate to potential child menus
                    /*if (ChildMenu != null && ChildMenu is GmToggleableBaseMenu)
                    {
                        ((GmToggleableBaseMenu)ChildMenu).m_visible = value;
                    }*/
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
            m_visible = false;

            PlayCloseSound();
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

        private void PlayCloseSound()
        {
            API.PlaySoundFrontend(-1, "NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET",  false);
        }
    }
}
