using UnityEngine;
using Mirror;
using Steamworks;

namespace Multiplayer.Demo
{
    public class MultiplayerDemoPlayer : NetworkBehaviour
    {
        public static MultiplayerDemoPlayer myPlayer;

        public AudioSource audioSource;

        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public override void OnStartLocalPlayer()
        {
            myPlayer = this;
            SteamUser.StartVoiceRecording(); // Start recording automatically
        }

        void Update()
        {
            if (isLocalPlayer)
            {
                EVoiceResult voiceResult = SteamUser.GetAvailableVoice(out uint compressed);
                if (voiceResult == EVoiceResult.k_EVoiceResultOK && compressed > 1024)
                {
                    byte[] byteBuffer = new byte[1024];
                    voiceResult = SteamUser.GetVoice(true, byteBuffer, 1024, out uint bufferSize);
                    if (voiceResult == EVoiceResult.k_EVoiceResultOK && bufferSize > 0)
                    {
                        CmdSendVoiceData(byteBuffer, bufferSize);
                    }
                }
            }
        }

        [Command]
        void CmdSendVoiceData(byte[] byteBuffer, uint byteCount)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 50, LayerMask.GetMask("Player"));
            foreach (Collider collider in colliders)
            {
                NetworkIdentity targetIdentity = collider.GetComponent<NetworkIdentity>();
                if (targetIdentity != null && targetIdentity.connectionToClient != connectionToClient) // Prevent own voice playback
                {
                    TargetClientPlaySound(targetIdentity.connectionToClient, byteBuffer, byteCount);
                }
            }
        }

        [TargetRpc]
        void TargetClientPlaySound(NetworkConnection target, byte[] byteBuffer, uint byteCount)
        {
            byte[] destBuffer = new byte[22050 * 2];
            EVoiceResult voiceResult = SteamUser.DecompressVoice(byteBuffer, byteCount, destBuffer, (uint)destBuffer.Length, out uint bytesWritten, 22050);

            if (voiceResult == EVoiceResult.k_EVoiceResultOK && bytesWritten > 0)
            {
                audioSource.clip = AudioClip.Create(Random.Range(100, 1000000).ToString(), 22050, 1, 22050, false);
                float[] audioData = new float[22050];

                for (int i = 0; i < audioData.Length; ++i)
                {
                    audioData[i] = (short)(destBuffer[i * 2] | destBuffer[i * 2 + 1] << 8) / 32768.0f;
                }

                audioSource.clip.SetData(audioData, 0);
                audioSource.Play();
            }
        }
    }
}
