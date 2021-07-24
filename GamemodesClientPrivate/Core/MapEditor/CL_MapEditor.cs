using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using GamemodesClient.Core;
using GamemodesClientPrivate.Core.MapEditor.Menus;
using System.Threading.Tasks;

namespace GamemodesClientPrivate.Core.MapEditor
{
    public class MapEditor : GmScript
    {
        private const float m_lookMult = 480f;

        private const float m_moveMult = 12f;

        private MapMainMenu m_mainMenu = new MapMainMenu();

        private Camera m_editorCamera = null;

        public MapEditor()
        {
            m_mainMenu.OnClose += Cleanup;
        }

        [Tick]
        private async Task OnTick()
        {
            if (Game.IsControlJustPressed(0, Control.InteractionMenu))
            {
                m_mainMenu.Visible = !m_mainMenu.Visible;
            }

            m_mainMenu.Update();

            if (m_mainMenu.Visible)
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

            await Task.FromResult(0);
        }

        private void Cleanup()
        {
            // Reset camera
            API.RenderScriptCams(false, false, 0, false, false);
            m_editorCamera = null;

            PlayerControlManager.HasControl = true;

            Screen.Hud.IsRadarVisible = true;
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
