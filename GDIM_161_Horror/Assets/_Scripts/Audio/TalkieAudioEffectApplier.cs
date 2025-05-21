using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Dissonance;
using Dissonance.Audio.Playback;

[RequireComponent(typeof(VoicePlayback))]
[RequireComponent(typeof(AudioSource))]
public class TalkieAudioEffectApplier : MonoBehaviour
{
    [Header("Mixer Groups")]
    [Tooltip("Clean/proximity chat")]
    public AudioMixerGroup globalMixerGroup;
    [Tooltip("Distorted/talkie chat")]
    public AudioMixerGroup talkieMixerGroup;

    [Header("Talkie Room Name")]
    [Tooltip("Must match your VoiceBroadcastTrigger/ReceiptTrigger Room name")]
    public string talkieRoomName = "Walkie";

    private AudioSource _audioSource;
    private VoicePlayback _playback;
    private DissonanceComms _comms;
    private VoicePlayerState _playerState;
    private readonly List<RemoteChannel> _channels = new List<RemoteChannel>();

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _playback = GetComponent<VoicePlayback>();
        _comms = FindFirstObjectByType<DissonanceComms>();
        // Default routing
        _audioSource.outputAudioMixerGroup = globalMixerGroup;
    }

    private void OnEnable()
    {
        // Look up the player state for this playback instance
        _playerState = _comms.FindPlayer(_playback.PlayerName);
    }

    private void OnDisable()
    {
        _playerState = null;
    }

    private void Update()
    {
        if (_playerState == null || !_playerState.IsSpeaking)
            return;

        // Get all channels this voice is currently speaking on
        _channels.Clear();
        _playerState.GetSpeakingChannels(_channels);

        // If any of those channels is our talkie room, switch to talkie group
        var onTalkie = _channels.Exists(c => c.Type == ChannelType.Room
                                          && c.TargetName == talkieRoomName);

        _audioSource.outputAudioMixerGroup = onTalkie
            ? talkieMixerGroup
            : globalMixerGroup;
    }
}
