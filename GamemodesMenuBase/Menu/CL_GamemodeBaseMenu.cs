using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace GamemodesClientMenuBase.Menu
{
    /// <summary>
    /// Menu base
    /// </summary>
    public abstract class GamemodeBaseMenu
    {
        /// <summary>
        /// Currently selected menu item index
        /// </summary>
        public int SelectedIndex { get; private set; } = 0;

        /// <summary>
        /// Default starting X pos of menu
        /// </summary>
        protected const int m_posX = 200;

        /// <summary>
        /// Default starting Y pos of menu
        /// </summary>
        protected const int m_posY = 125;

        /// <summary>
        /// Width of menu items
        /// </summary>
        protected const int m_itemWidth = 300;

        /// <summary>
        /// Height of menu items
        /// </summary>
        protected const int m_itemHeight = 25;

        /// <summary>
        /// Color of menu items
        /// </summary>
        protected readonly Color m_itemColor = Color.FromArgb(200, 0, 0, 0);

        /// <summary>
        /// Color of selected menu items
        /// </summary>
        protected readonly Color m_itemSelectedColor = Color.FromArgb(60, 60, 60);

        /// <summary>
        /// Collection of menu items
        /// </summary>
        protected readonly Queue<GamemodeMenuItem> m_menuItems = new Queue<GamemodeMenuItem>();

        /// <summary>
        /// Draw menu and handle input
        /// </summary>
        /// <param name="_posX">Starting X position of menu</param>
        /// <param name="_posY">Starting Y position of menu</param>
        public virtual void Update(int _posX = m_posX, int _posY = m_posY)
        {
            if (m_menuItems.Count > 0)
            {
                // Store currently selected item to pass to HandleInput later
                GamemodeMenuItem selectedMenuItem = null;

                // Draw each menu item, also dequeue each of them while doing that
                int itemsCount = m_menuItems.Count;
                for (int itemIdx = 0; m_menuItems.Count > 0; itemIdx++)
                {
                    GamemodeMenuItem menuItem = m_menuItems.Dequeue();

                    menuItem.X = _posX;
                    menuItem.Y = _posY;
                    menuItem.Color = itemIdx == SelectedIndex ? m_itemSelectedColor : m_itemColor;

                    menuItem.Draw();

                    if (itemIdx == SelectedIndex)
                    {
                        selectedMenuItem = menuItem;
                    }

                    _posY += m_itemHeight;
                }

                // Clamp selected index between 0 and last menu item index
                SelectedIndex = Math.Max(0, Math.Min(itemsCount - 1, SelectedIndex));

                HandleInput(itemsCount - 1, selectedMenuItem);
            }
        }

        /// <summary>
        /// Handle input
        /// </summary>
        /// <param name="_max">Index of last menu item</param>
        /// <param name="_selectedItem">Currently selected menu item</param>
        private void HandleInput(int _max, GamemodeMenuItem _selectedItem)
        {
            // Arrow up
            if (Game.IsControlJustPressed(0, Control.PhoneUp))
            {
                if (--SelectedIndex < 0)
                {
                    SelectedIndex = _max;
                }

                API.PlaySoundFrontend(-1, "NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET", false);
            }
            // Arrow down
            else if (Game.IsControlJustPressed(0, Control.PhoneDown))
            {
                if (++SelectedIndex == _max + 1)
                {
                    SelectedIndex = 0;
                }

                API.PlaySoundFrontend(-1, "NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET", false);
            }
            // Enter key
            else if (Game.IsControlJustPressed(0, Control.FrontendRdown))
            {
                if (_selectedItem.OnClick != null)
                {
                    _selectedItem.OnClick(SelectedIndex, _selectedItem.Label);
                }

                API.PlaySoundFrontend(-1, "SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET", false);
            }
        }
    }
}
