using CitizenFX.Core.Native;

namespace GamemodesClient.Utils
{
    /// <summary>
    /// Audio utils
    /// </summary>
    public static class AudioUtils
    {
        /// <summary>
        /// Play a frontend sound
        /// </summary>
        /// <param name="_audioBank">Audio bank</param>
        /// <param name="_audioName">Audio name</param>
        public static void PlayFrontendAudio(string _audioBank, string _audioName)
        {
            // Request the audio bank
            API.RequestScriptAudioBank(_audioBank, false);

            // Play the sound
            API.PlaySoundFrontend(-1, _audioName, _audioBank, false);

            // Release the audio bank
            API.ReleaseScriptAudioBank();
        }
    }
}
