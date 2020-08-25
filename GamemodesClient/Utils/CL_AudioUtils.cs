using CitizenFX.Core.Native;

namespace GamemodesClient.Utils
{
    public static class AudioUtils
    {
        public static void PlayFrontendAudio(string _audioBank, string _audioName)
        {
            API.RequestScriptAudioBank(_audioBank, false);

            API.PlaySoundFrontend(-1, _audioName, _audioBank, false);

            API.ReleaseScriptAudioBank();
        }
    }
}
