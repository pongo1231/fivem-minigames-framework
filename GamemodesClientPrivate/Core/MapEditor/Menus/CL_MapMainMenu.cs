using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using GamemodesClient.Core;
using GamemodesClientMenuFw.GmMenuFw.Menu;

namespace GamemodesClientPrivate.Core.MapEditor.Menus
{
    /// <summary>
    /// Main map editor menu
    /// </summary>
    public class MapMainMenu : GmUserMenu
    {
        private const float m_lookMult = 480f;

        private const float m_moveMult = 12f;

        private Camera m_editorCamera = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public MapMainMenu() : base("Mappy The Editor")
        {
            ImmediateMode = false;

            var menu = new MapAboutMenu();

            AddChildMenuItem("About", menu);
        }

        public override void Close()
        {
            base.Close();

            // Reset camera
            API.RenderScriptCams(false, false, 0, false, false);
            m_editorCamera = null;

            PlayerControlManager.HasControl = true;

            Screen.Hud.IsRadarVisible = true;
        }

        protected override void Tick()
        {
            if (m_editorCamera == null)
            {
                m_editorCamera = World.CreateCamera(Game.PlayerPed.Position,
                    new Vector3(0f, 0f, Game.PlayerPed.Heading), 60);
                World.RenderingCamera = m_editorCamera;
            }

            PlayerControlManager.HasControl = false;

            Screen.Hud.IsRadarVisible = false;

            HandleCamera();
        }

        private void HandleCamera()
        {
            var newRot = m_editorCamera.Rotation;
            newRot.X -= Game.GetDisabledControlNormal(0,
                Control.LookUpDown) * m_lookMult * Game.LastFrameTime;
            newRot.Y = 0f;
            newRot.Z -= Game.GetDisabledControlNormal(0,
                Control.LookLeftRight) * m_lookMult * Game.LastFrameTime;
            m_editorCamera.Rotation = newRot;

            var newPos = m_editorCamera.Position;
            newPos -= m_editorCamera.UpVector * Game.GetDisabledControlNormal(0,
                Control.MoveUpDown) * m_moveMult * Game.LastFrameTime;
            newPos += m_editorCamera.RightVector * Game.GetDisabledControlNormal(0,
                Control.MoveLeftRight) * m_moveMult * Game.LastFrameTime;
            m_editorCamera.Position = newPos;
        }
    }
}
