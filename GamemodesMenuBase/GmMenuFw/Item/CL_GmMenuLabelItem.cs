using System.Drawing;
using GamemodesClientMenuFw.GmMenuFw.Item.Base;

using Text = CitizenFX.Core.UI.Text;
using Font = CitizenFX.Core.UI.Font;
using Alignment = CitizenFX.Core.UI.Alignment;

namespace GamemodesClientMenuFw.GmMenuFw.Item
{
    /// <summary>
    /// Labeled menu item
    /// </summary>
    public class GmMenuLabelItem : GmMenuBaseItem
    {
        /// <summary>
        /// Caption of drawn text element
        /// </summary>
        public string Label
        {
            get
            {
                return m_text.Caption;
            }
        }

        /// <summary>
        /// Text to draw
        /// </summary>
        private Text m_text = null;

        /// <summary>
        /// Text to draw on the right side of the rect
        /// </summary>
        private Text m_suffixText = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_width">Width</param>
        /// <param name="_height">Height</param>
        /// <param name="_label">Label of item</param>
        /// <param name="_suffix">Label to draw on the right side</param>
        public GmMenuLabelItem(int _width, int _height, string _label, string _suffix = null)
            : base(_width, _height)
        {
            m_text = new Text(_label, new PointF(), 0.35f, Color.FromArgb(255, 255, 255),
                Font.ChaletLondon, Alignment.Left, false, false);

            if (_suffix != null)
            {
                m_suffixText = new Text(_suffix, new PointF(), 0.35f,
                    Color.FromArgb(255, 255, 255), Font.ChaletLondon, Alignment.Right, true, true);
            }
        }

        /// <summary>
        /// Draw function
        /// </summary>
        public override void Draw()
        {
            // Draw text(s) first since rect will cover it / them otherwise

            // Draw suffix if existing
            if (m_suffixText != null)
            {
                m_suffixText.Position = new PointF(X + RectSize.Width * 0.5f - 5,
                    Y - RectSize.Height * 0.5f);

                m_suffixText.Draw();
            }

            m_text.Position = new PointF(X - RectSize.Width * 0.5f + 5, Y - RectSize.Height * 0.5f);

            m_text.Draw();

            base.Draw();
        }
    }
}
