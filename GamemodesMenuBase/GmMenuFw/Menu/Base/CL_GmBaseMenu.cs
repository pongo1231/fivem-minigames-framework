using CitizenFX.Core;
using CitizenFX.Core.Native;
using GamemodesClientMenuFw.GmMenuFw.Item;
using GamemodesClientMenuFw.GmMenuFw.Item.Base;
using System;
using System.Drawing;

using static GamemodesClientMenuFw.GmMenuFw.Item.GmMenuActionItem;

namespace GamemodesClientMenuFw.GmMenuFw.Menu.Base
{
    /// <summary>
    /// Menu base
    /// </summary>
    public abstract class GmBaseMenu
    {
        /// <summary>
        /// Currently selected menu item index
        /// </summary>
        public int SelectedIndex { get; protected set; } = 0;

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
        protected readonly GmMenuQueue<GmMenuBaseItem> m_menuItems
            = new GmMenuQueue<GmMenuBaseItem>();

        /// <summary>
        /// Starting X pos of menu this frame
        /// </summary>
        protected int PosX { get; set; } = m_posX;

        /// <summary>
        /// Starting Y pos of menu this frame
        /// </summary>
        protected int PosY { get; set; } = m_posY;

        /// <summary>
        /// Whether to use either immediate or retained mode
        /// Immediate = Menu items have to be added every frame
        /// Retained = Menu items only have to be added once
        /// </summary>
        protected bool ImmediateMode { get; set; } = true;

        /// <summary>
        /// Initial delay before doing repeated navgiation action after first user-initiated navigate
        /// </summary>
        private const float m_initialNavigateRepeatDelaySecs = 0.5f;

        /// <summary>
        /// Continued delay before doing repeated navgiation action after initial automated navigate
        /// </summary>
        private const float m_continualNativgateRepeatDelaySecs = 0.05f;

        /// <summary>
        /// Current delay before doing repeated automatic navigate action
        /// </summary>
        private float m_curNavigateRepeatDelaySecs = m_initialNavigateRepeatDelaySecs;

        /// <summary>
        /// Tick function to add menu content
        /// </summary>
        /// <returns>Whether Update function should complete this frame</returns>
        protected abstract bool MenuTick();

        /// <summary>
        /// Add a labeled menu item
        /// </summary>
        /// <param name="_label">Label to display</param>
        public void AddLabelItem(string _label)
        {
            m_menuItems.Enqueue(new GmMenuLabelItem(m_itemWidth, m_itemHeight, _label));
        }

        /// <summary>
        /// Add a labeled menu item with an callback
        /// </summary>
        /// <param name="_label">Label to display</param>
        /// <param name="_onClick">Callback to invoke on click</param>
        public void AddActionItem(string _label, GmMenuItemClick _onClick)
        {
            m_menuItems.Enqueue(new GmMenuActionItem(m_itemWidth, m_itemHeight, _label, _onClick));
        }

        /// <summary>
        /// Draw menu and handle input
        /// </summary>
        public void Update()
        {
            // Abort if Tick function didn't succeed
            if (!MenuTick())
            {
                return;
            }

            // Only run menu drawing logic if it actually contains items
            if (m_menuItems.Count > 0)
            {
                var itemsCount = m_menuItems.Count;

                var selectedMenuItem = DrawItems();

                // Clamp selected index between 0 and last menu item index
                SelectedIndex = Math.Max(0, Math.Min(itemsCount - 1, SelectedIndex));

                HandleInput(itemsCount - 1, selectedMenuItem);
            }

            // Reset menu item positions
            PosX = m_posX;
            PosY = m_posY;
        }

        /// <summary>
        /// Handle drawing of all menu items
        /// Only called if amount of items is over 0
        /// </summary>
        /// <returns>Currently selected menu item object</returns>
        protected virtual GmMenuBaseItem DrawItems()
        {
            GmMenuBaseItem selectedMenuItem = null;

            // Draw each menu item, also dequeue each of them while doing that if in immediate mode
            var menuItemNode = m_menuItems;
            for (var itemIdx = 0; ImmediateMode
                ? menuItemNode.Count > 0 : menuItemNode != null; itemIdx++)
            {
                var menuItem = menuItemNode.Content;

                menuItem.X = PosX;
                menuItem.Y = PosY;
                menuItem.Color = itemIdx == SelectedIndex ? m_itemSelectedColor : m_itemColor;

                menuItem.Draw();

                if (itemIdx == SelectedIndex)
                {
                    selectedMenuItem = menuItem;
                }

                PosY += m_itemHeight;

                // Only pop item if menu is in immediate mode
                if (ImmediateMode)
                {
                    menuItemNode.Dequeue();
                }
                else
                {
                    menuItemNode = menuItemNode.Next;
                }
            }

            return selectedMenuItem;
        }

        /// <summary>
        /// Handle input
        /// </summary>
        /// <param name="_max">Index of last menu item</param>
        /// <param name="_selectedItem">Currently selected menu item</param>
        protected virtual void HandleInput(int _max, GmMenuBaseItem _selectedItem)
        {
            // Arrow up
            if (Game.IsControlPressed(0, Control.PhoneUp))
            {
                // Ensure automated navigate only happens after specified time
                if (m_curNavigateRepeatDelaySecs > 0)
                {
                    m_curNavigateRepeatDelaySecs -= Game.LastFrameTime;

                    if (m_curNavigateRepeatDelaySecs
                        < m_initialNavigateRepeatDelaySecs - Game.LastFrameTime)
                    {
                        return;
                    }
                }
                else
                {
                    m_curNavigateRepeatDelaySecs = m_continualNativgateRepeatDelaySecs;
                }

                if (--SelectedIndex < 0)
                {
                    SelectedIndex = _max;
                }

                API.PlaySoundFrontend(-1, "NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET", false);
            }
            // Arrow down
            else if (Game.IsControlPressed(0, Control.PhoneDown))
            {
                // Ensure automated navigate only happens after specified time
                if (m_curNavigateRepeatDelaySecs > 0)
                {
                    m_curNavigateRepeatDelaySecs -= Game.LastFrameTime;

                    if (m_curNavigateRepeatDelaySecs
                        < m_initialNavigateRepeatDelaySecs - Game.LastFrameTime)
                    {
                        return;
                    }
                }
                else
                {
                    m_curNavigateRepeatDelaySecs = m_continualNativgateRepeatDelaySecs;
                }

                if (++SelectedIndex == _max + 1)
                {
                    SelectedIndex = 0;
                }

                API.PlaySoundFrontend(-1, "NAV_UP_DOWN", "HUD_FRONTEND_DEFAULT_SOUNDSET", false);
            }
            else
            {
                // Reset automated navigate action timer
                m_curNavigateRepeatDelaySecs = m_initialNavigateRepeatDelaySecs;

                // Enter key
                if (Game.IsControlJustPressed(0, Control.FrontendRdown))
                {
                    // Try executing the action if it's an action menu item
                    var actionItem = _selectedItem as GmMenuActionItem;
                    if (actionItem != null && actionItem.OnClick != null)
                    {
                        actionItem.OnClick(SelectedIndex, actionItem.Label);
                    }

                    API.PlaySoundFrontend(-1, "SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET", false);
                }
            }
        }
    }
}
