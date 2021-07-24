using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Threading.Tasks;

namespace GamemodesClient.Utils
{
    /// <summary>
    /// Ptfx utils
    /// </summary>
    public static class PtfxUtils
    {
        /// <summary>
        /// Play ptfx on entity
        /// </summary>
        /// <param name="_entity">Entity</param>
        /// <param name="_asset">Asset of ptfx</param>
        /// <param name="_effectName">Name of ptfx</param>
        /// <param name="_networked">Whether it should be networked</param>
        /// <param name="_scale">Scale of ptfx</param>
        /// <param name="_boneIndex">Bone index of entity to play ptfx on</param>
        public static async void PlayPtfxOnEntity(this Entity _entity, string _asset,
            string _effectName, bool _networked = false, float _scale = 1f, int _boneIndex = -1)
        {
            // Request ptfx asset
            API.RequestNamedPtfxAsset(_asset);

            // Wait for ptfx asset to load
            while (!API.HasNamedPtfxAssetLoaded(_asset))
            {
                await BaseScript.Delay(0);
            }

            // Use ptfx asset for next call
            API.UseParticleFxAssetNextCall(_asset);

            // Play ptfx
            if (_networked)
            {
                API.StartNetworkedParticleFxNonLoopedOnPedBone(_effectName, _entity.Handle, 0f, 0f,
                    0f, 0f, 0f, 0f, _boneIndex, _scale, false, false, false);
            }
            else
            {
                API.StartParticleFxNonLoopedOnPedBone(_effectName, _entity.Handle, 0f, 0f, 0f,
                    0f, 0f, 0f, _boneIndex, _scale, false, false, false);
            }

            // Unload ptfx asset
            API.RemoveNamedPtfxAsset(_asset);
        }

        /// <summary>
        /// Play looped ptfx on entity
        /// </summary>
        /// <param name="_entity">Entity</param>
        /// <param name="_asset">Asset of ptfx</param>
        /// <param name="_effectName">Name of ptfx</param>
        /// <param name="_networked">Whether it should be networked</param>
        /// <param name="_scale">Scale of ptfx</param>
        /// <param name="_boneIndex">Bone index of entity to play ptfx on</param>
        /// <returns>Handle of looped ptfx</returns>
        public static async Task<int> PlayLoopedPtfxOnEntity(this Entity _entity, string _asset,
            string _effectName, bool _networked = false, float _scale = 1f, int _boneIndex = -1)
        {
            // Request ptfx asset
            API.RequestNamedPtfxAsset(_asset);

            // Wait for ptfx asset to load
            while (!API.HasNamedPtfxAssetLoaded(_asset))
            {
                await BaseScript.Delay(0);
            }

            // Use ptfx asset for next call
            API.UseParticleFxAssetNextCall(_asset);

            // Used for storing the ptfx handle
            int ptfxHandle;

            // Play ptfx and store handle
            if (_networked)
            {
                ptfxHandle = API.StartNetworkedParticleFxLoopedOnEntityBone(_effectName,
                    _entity.Handle, 0f, 0f, 0f, 0f, 0f, 0f, _boneIndex, _scale, false, false,
                    false);
            }
            else
            {
                ptfxHandle = API.StartParticleFxLoopedOnEntityBone(_effectName, _entity.Handle,
                    0f, 0f, 0f, 0f, 0f, 0f, _boneIndex, _scale, false, false, false);
            }

            // Unload ptfx asset
            API.RemoveNamedPtfxAsset(_asset);

            // Return ptfx handle
            return ptfxHandle;
        }

        /// <summary>
        /// Stop looped ptfx
        /// </summary>
        /// <param name="_ptfxHandle">Handle of ptfx</param>
        public static void StopLoopedPtfx(int _ptfxHandle)
        {
            // Stop looped ptfx
            API.StopParticleFxLooped(_ptfxHandle, false);
        }

        /// <summary>
        /// Play ptfx at position
        /// </summary>
        /// <param name="_pos">Position</param>
        /// <param name="_asset">Asset of ptfx</param>
        /// <param name="_effectName">Name of ptfx</param>
        /// <param name="_networked">Whether ptfx should be networked</param>
        /// <param name="_scale">Scale of ptfx</param>
        public static async void PlayPtfxAtPos(Vector3 _pos, string _asset, string _effectName,
            bool _networked = false, float _scale = 1f)
        {
            // Request ptfx asset
            API.RequestNamedPtfxAsset(_asset);

            // Wait for ptfx asset to load
            while (!API.HasNamedPtfxAssetLoaded(_asset))
            {
                await BaseScript.Delay(0);
            }

            // Use ptfx asset for next call
            API.UseParticleFxAssetNextCall(_asset);

            // Play ptfx
            if (_networked)
            {
                API.StartNetworkedParticleFxNonLoopedAtCoord(_effectName, _pos.X, _pos.Y, _pos.Z,
                    0f, 0f, 0f, _scale, false, false, false);
            }
            else
            {
                API.StartParticleFxNonLoopedAtCoord(_effectName, _pos.X, _pos.Y, _pos.Z, 0f, 0f,
                    0f, _scale, false, false, false);
            }

            // Unload ptfx asset
            API.RemoveNamedPtfxAsset(_asset);
        }
    }
}
