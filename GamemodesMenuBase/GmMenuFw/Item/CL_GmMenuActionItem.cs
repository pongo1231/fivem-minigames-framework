namespace GamemodesClientMenuFw.GmMenuFw.Item
{
    /// <summary>
    /// Labeled menu item with action on click
    /// </summary>
    public class GmMenuActionItem : GmMenuLabelItem
    {
        /// <summary>
        /// On Item Click action
        /// </summary>
        /// <param name="_idx">Index of selected item</param>
        /// <param name="_label">Label of selected item</param>
        public delegate void GmMenuItemClick(int _idx, string _label);

        /// <summary>
        /// On Click action
        /// </summary>
        public GmMenuItemClick OnClick { get; private set; } = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_width">Width</param>
        /// <param name="_height">Height</param>
        /// <param name="_label">Label of item</param>
        /// <param name="_onClickAction">Action to execute on click</param>
        /// /// <param name="_suffix">Label to draw on the right side</param>
        public GmMenuActionItem(int _width, int _height, string _label,
            GmMenuItemClick _onClickAction = null, string _suffix = null)
            : base(_width, _height, _label, _suffix)
        {
            OnClick = _onClickAction;
        }
    }
}
