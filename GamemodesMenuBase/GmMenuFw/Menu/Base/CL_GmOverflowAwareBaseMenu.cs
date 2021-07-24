using GamemodesClientMenuFw.GmMenuFw.Item.Base;
using System;

namespace GamemodesClientMenuFw.GmMenuFw.Menu.Base
{
    /// <summary>
    /// Menu base for menu item overflow aware menus
    /// </summary>
    public abstract class GmOverflowAwareBaseMenu : GmBaseMenu
    {
        /// <summary>
        /// Max amount of displayed items on screen at once
        /// </summary>
        private const int m_maxVisibleItems = 15;

        /// <summary>
        /// Draw items function
        /// </summary>
        protected override GmMenuBaseItem DrawItems()
        {
            GmMenuBaseItem selectedMenuItem = null;
            var overflowUpBias = -Math.Min(0,
                (int)(m_menuItems.Count - 1 - SelectedIndex - m_maxVisibleItems * 0.5f));
            var overflowDownBias = -Math.Min(0, (int)(SelectedIndex - m_maxVisibleItems * 0.5f));

            // Draw each menu item (if visible), also dequeue each of them while doing that
            // if in immediate mode
            var menuItemNode = m_menuItems;
            for (var itemIdx = 0; ImmediateMode
                ? menuItemNode.Count > 0 : menuItemNode != null; itemIdx++)
            {
                // Only display if in range
                if (itemIdx >= SelectedIndex - m_maxVisibleItems * 0.5f - overflowUpBias
                    && itemIdx <= SelectedIndex + m_maxVisibleItems * 0.5f + overflowDownBias)
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
                }

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
    }
}
