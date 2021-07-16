using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Drawing;

using Rectangle = CitizenFX.Core.UI.Rectangle;
using Text = CitizenFX.Core.UI.Text;
using Font = CitizenFX.Core.UI.Font;
using Alignment = CitizenFX.Core.UI.Alignment;
using static GamemodesClientMenuBase.Menu.GamemodeMenuItem;

namespace GamemodesClientMenuBase.Menu
{
    public class GamemodeMenu
    {
        public bool Visible { get; set; } = false;

        public string Title { get; private set; } = null;

        public int SelectedIndex { get; private set; } = 0;

        public bool ShouldToggle { get; private set; } = false;

        private readonly int m_posX = 200;

        private readonly int m_posY = 125;

        private readonly int m_itemWidth = 300;

        private readonly int m_itemHeight = 25;

        private readonly int m_headerHeight = 50;

        private readonly Color m_itemColor = Color.FromArgb(200, 0, 0, 0);

        private readonly Color m_itemSelectedColor = Color.FromArgb(60, 60, 60);

        private readonly Color m_headerColor = Color.FromArgb(40, 40, 120);

        private readonly Queue<GamemodeMenuItem> m_menuItems = new Queue<GamemodeMenuItem>();

        public GamemodeMenu(string _title)
        {
            Title = _title;
        }

        public void AddLabelItem(string _label, GamemodeMenuItemClick _onClick = null)
        {
            m_menuItems.Enqueue(new GamemodeMenuItem(m_itemWidth, m_itemHeight, _label, _onClick));
        }

        public void Update()
        {
            ShouldToggle = Game.IsControlJustPressed(0, Control.InteractionMenu)
                || (Visible && Game.IsControlJustPressed(0, Control.PhoneCancel));

            if (!Visible)
            {
                return;
            }

            int posX = m_posX;
            int posY = m_posY;

            Rectangle titleRect = new Rectangle(new PointF(posX, posY), new SizeF(m_itemWidth, m_headerHeight), m_headerColor, true);
            titleRect.Draw();

            Text titleText = new Text(Title, new PointF(posX, posY - m_headerHeight * 0.5f), 1f, Color.FromArgb(255, 255, 255), Font.Monospace, Alignment.Center, true, true);
            titleText.Draw();

            posY += (int)(m_headerHeight - m_itemHeight * 0.5f);

            if (m_menuItems.Count > 0)
            {
                GamemodeMenuItem selectedMenuItem = null;

                int itemsCount = m_menuItems.Count;
                for (int itemIdx = 0; m_menuItems.Count > 0; itemIdx++)
                {
                    GamemodeMenuItem menuItem = m_menuItems.Dequeue();

                    menuItem.X = posX;
                    menuItem.Y = posY;
                    menuItem.Color = itemIdx == SelectedIndex ? m_itemSelectedColor : m_itemColor;

                    menuItem.Draw();

                    if (itemIdx == SelectedIndex)
                    {
                        selectedMenuItem = menuItem;
                    }

                    posY += m_itemHeight;
                }

                SelectedIndex = Math.Max(0, Math.Min(itemsCount - 1, SelectedIndex));

                HandleInput(itemsCount, selectedMenuItem);
            }

            m_menuItems.Clear();
        }

        private void HandleInput(int _max, GamemodeMenuItem _selectedItem)
        {
            if (Game.IsControlJustPressed(0, Control.PhoneUp))
            {
                if (--SelectedIndex < 0)
                {
                    SelectedIndex = _max - 1;
                }
            }
            else if (Game.IsControlJustPressed(0, Control.PhoneDown))
            {
                if (++SelectedIndex == _max)
                {
                    SelectedIndex = 0;
                }
            }
            else if (Game.IsControlJustPressed(0, Control.PhoneSelect))
            {
                if (_selectedItem.OnClick != null)
                {
                    _selectedItem.OnClick(SelectedIndex, _selectedItem.Label);
                }
            }
        }
    }
}
