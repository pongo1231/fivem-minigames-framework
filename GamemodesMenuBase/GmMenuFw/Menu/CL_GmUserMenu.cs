using System.Drawing;
using GamemodesClientMenuFw.GmMenuFw.Menu.Base;
using System;
using System.Threading.Tasks;
using System.Linq;

using Rectangle = CitizenFX.Core.UI.Rectangle;
using Text = CitizenFX.Core.UI.Text;
using Font = CitizenFX.Core.UI.Font;
using Alignment = CitizenFX.Core.UI.Alignment;
using GamemodesShared.Utils;

namespace GamemodesClientMenuFw.GmMenuFw.Menu
{
    /// <summary>
    /// User menu with title bar
    /// </summary>
    public abstract class GmUserMenu : GmToggleableBaseMenu
    {
        /// <summary>
        /// Attribute for functions which should be called every tick (if open)
        /// </summary>
        [AttributeUsage(AttributeTargets.Method)]
        protected class GmUserMenuTick : Attribute
        {

        }

        /// <summary>
        /// Functions to invoke on close
        /// </summary>
        public event Action OnClose = null;

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
        /// Event for custom menu tick functions of inheritee
        /// </summary>
        private event Func<Task> m_onTick = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_title">Label to show in title bar</param>
        public GmUserMenu(string _title)
        {
            Title = _title;

            // Add menu tick functions
            ReflectionUtils.GetAllMethodsWithAttributeForClass<Func<Task>, GmUserMenuTick>(this,
                ref m_onTick);
        }

        /// <summary>
        /// Close function
        /// </summary>
        public override void Close()
        {
            base.Close();

            OnClose?.Invoke();
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

            var titleRect = new Rectangle(new PointF(PosX, PosY),
                new SizeF(m_itemWidth, m_headerHeight), m_headerColor, true);
            titleRect.Draw();

            var titleText = new Text(Title,
               new PointF(PosX, PosY - m_headerHeight * 0.5f), 1f, Color.FromArgb(255, 255, 255),
               Font.Monospace, Alignment.Center, true, true);
            titleText.Draw();

            // Take title bar in consideration for Y position
            PosY += (int)(m_headerHeight - m_itemHeight * 0.5f);

            // Call all custom menu tick functions
            m_onTick?.Invoke();

            return true;
        }
    }
}
