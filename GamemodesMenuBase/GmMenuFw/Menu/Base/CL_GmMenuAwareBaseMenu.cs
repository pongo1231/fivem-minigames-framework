using GamemodesClientMenuFw.GmMenuFw.Item;

namespace GamemodesClientMenuFw.GmMenuFw.Menu.Base
{
    /// <summary>
    /// Menu base for menus with awareness for child and parent menus
    /// </summary>
    public abstract class GmMenuAwareBaseMenu : GmOverflowAwareBaseMenu
    {
        /// <summary>
        /// Parent menu
        /// </summary>
        protected virtual GmMenuAwareBaseMenu ParentMenu { get; set; } = null;

        /// <summary>
        /// Child menu
        /// </summary>
        protected virtual GmMenuAwareBaseMenu ChildMenu { get; set; } = null;

        /// <summary>
        /// Add a labeled menu item which opens a new menu
        /// </summary>
        /// <param name="_label">Label to display</param>
        /// <param name="_childMenu">Instance of menu to open</param>
        public void AddChildMenuItem(string _label, GmMenuAwareBaseMenu _childMenu)
        {
            m_menuItems.Enqueue(new GmMenuActionItem(m_itemWidth, m_itemHeight, _label,
                (_idx, _thisLabel) =>
            {
                ChildMenu = _childMenu;
                _childMenu.ParentMenu = this;
            }, ">"));
        }

        /// <summary>
        /// Closes this menu
        /// </summary>
        public virtual void Close()
        {
            if (ParentMenu != null)
            {
                ParentMenu.ChildMenu = null;
                ParentMenu = null;
            }

            // Reset selected index
            SelectedIndex = 0;
        }

        /// <summary>
        /// Menu tick function
        /// </summary>
        protected override bool MenuTick()
        {
            // Redirect to child menu if existing
            if (ChildMenu != null)
            {
                ChildMenu.Update();

                return false;
            }

            return true;
        }
    }
}
