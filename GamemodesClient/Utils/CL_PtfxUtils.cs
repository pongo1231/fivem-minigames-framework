using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace GamemodesClient.Utils
{
    public static class PtfxUtils
    {
        public static async void PlayPtfxOnEntity(this Entity _entity, string _asset, string _effectName, bool _networked = false, float _scale = 1f)
        {
            API.RequestNamedPtfxAsset(_asset);

            while (!API.HasNamedPtfxAssetLoaded(_asset))
            {
                await BaseScript.Delay(0);
            }

            API.UseParticleFxAssetNextCall(_asset);

            if (_networked)
            {
                API.StartNetworkedParticleFxNonLoopedOnEntity(_effectName, _entity.Handle, 0f, 0f, 0f, 0f, 0f, 0f, _scale, false, false, false);
            }
            else
            {
                API.StartParticleFxNonLoopedOnEntity(_effectName, _entity.Handle, 0f, 0f, 0f, 0f, 0f, 0f, _scale, false, false, false);
            }

            API.RemoveNamedPtfxAsset(_asset);
        }

        public static async void PlayPtfxAtPos(Vector3 _pos, string _asset, string _effectName, bool _networked = false, float _scale = 1f)
        {
            API.RequestNamedPtfxAsset(_asset);

            while (!API.HasNamedPtfxAssetLoaded(_asset))
            {
                await BaseScript.Delay(0);
            }

            API.UseParticleFxAssetNextCall(_asset);

            if (_networked)
            {
                API.StartNetworkedParticleFxNonLoopedAtCoord(_effectName, _pos.X, _pos.Y, _pos.Z, 0f, 0f, 0f, _scale, false, false, false);
            }
            else
            {
                API.StartParticleFxNonLoopedAtCoord(_effectName, _pos.X, _pos.Y, _pos.Z, 0f, 0f, 0f, _scale, false, false, false);
            }

            API.RemoveNamedPtfxAsset(_asset);
        }
    }
}
