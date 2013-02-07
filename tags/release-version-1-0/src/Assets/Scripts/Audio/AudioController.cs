using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Represents the base class for sounds and sound controls. Derived scripts define their audio 
/// </summary>
public abstract class AudioController : MonoBehaviour {
    /// <summary>
    /// Specifies the number of audio channels to create on the object. 
    /// More channels means more sounds can be played from the object simultaniously, but worse performance.
    /// </summary>
    public int NumberOfAudioChannels = 2;

    /// <summary>
    /// Specifies the global volume for this object. Is multiplied with the volume per sound and <see cref="LevelBehaviour.GlobalSoundVolume"/>
    /// </summary>
    public float ObjectVolume = 1f;

    private List<AudioSource> audioChannels;
    private float premultipliedVolume;

    private void Start() {
        // sanity check
        if (this.ObjectVolume < 0 || this.ObjectVolume > 1) {
            throw new UnityException("Global Sound Volume should be a number between 0 and 1");
        }

        // determine our premultiplied volume
        float globalVolume = LevelBehaviour.Instance.GlobalSoundVolume;
        this.premultipliedVolume = globalVolume*this.ObjectVolume;

        // create audio channels
        this.audioChannels = new List<AudioSource>(this.NumberOfAudioChannels);
        for (int i = 0; i < this.NumberOfAudioChannels; i++) {
            var audioChannel = this.gameObject.AddComponent<AudioSource>();

            // configure audio channel
            audioChannel.loop = false;

            this.audioChannels.Add(audioChannel);
        }

        // initialize each audio effect
        FieldInfo[] fields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (FieldInfo fieldInfo in fields) {
            // filter on 'AudioClip' 
            if (!fieldInfo.FieldType.IsAssignableFrom(typeof (AudioEffectConfiguration))) {
                continue;
            }

            // get value and initialize
            var effectConfiguration = fieldInfo.GetValue(this) as AudioEffectConfiguration;

            if (effectConfiguration == null) {
                if (Debug.isDebugBuild) {
                    throw new UnityException("One of the AudioEffectConfiguration objects was not assigned: " + fieldInfo.Name);
                }

                continue;
            }

            effectConfiguration.AudioControllerInstance = this;

            // perform optional validation
            if (Debug.isDebugBuild) {
                try {
                    effectConfiguration.SelfValidate();
                } catch(UnityException ex) {
                    Debug.LogWarning(
                        String.Format("One of the AudioEffectConfiguration objects was incorrectly configured: {0}{1}{2}", 
                                      fieldInfo.Name, Environment.NewLine, ex));
                }
            }

        }
    }

    private void Play(AudioEffectConfiguration audioEffectConfiguration) {
        if (this.enabled == false) {
            return;
        }

        // check if the sound effect can play
        if (audioEffectConfiguration.SoundEffect.Length == 0) {
            Debug.LogWarning("No sound clips found in a Sound Effect. Object: "+ this.gameObject.name);
            return;
        }

        // find an available audio channel
        AudioSource availableChannel = null;

        foreach (AudioSource channel in audioChannels) {
            if (!channel.isPlaying) {
                availableChannel = channel;
                break;
            }
        }

        if (availableChannel == null) {
            Debug.LogWarning("No audio channel available. All channels are in use. Game object: " + this.gameObject.name, this.gameObject);
            return;
        }

        // choose random audio effect
        int max = audioEffectConfiguration.SoundEffect.Length;
        int effectIndex = Random.Range(0, max - 1);
        availableChannel.clip = audioEffectConfiguration.SoundEffect[effectIndex];

        // set properties and play
        availableChannel.pitch = audioEffectConfiguration.Pitch;
        availableChannel.volume = audioEffectConfiguration.Volume * this.premultipliedVolume;
        availableChannel.bypassEffects = audioEffectConfiguration.BypassAudioEffects;
        availableChannel.loop = audioEffectConfiguration.Loop;

        availableChannel.Play();
    }

    #region Nested type: AudioEffectConfiguration

    /// <summary>
    /// Represents the single configuration of an audio effect
    /// </summary>
    [Serializable]
    public class AudioEffectConfiguration {
        /// <summary>
        /// Specifies the audio effects to bypass
        /// </summary>
        public bool BypassAudioEffects = false;

        /// <summary>
        /// Specifies a value between -3 and -3 (1 default) for the pitch
        /// </summary>
        public float Pitch = 1;

        /// <summary>
        /// Specifies looping behaviour
        /// </summary>
        public Boolean Loop = false;

        /// <summary>
        /// Specifies one of the random sound effect to play
        /// </summary>
        public AudioClip[] SoundEffect;

        /// <summary>
        /// Specifies the volume between 0 and 1 to multiply with <see cref="AudioController.ObjectVolume"/> and <see cref="LevelBehaviour.GlobalSoundVolume"/>
        /// </summary>
        public float Volume = 1;

        [NonSerialized]
        private AudioController audioController;

        internal AudioController AudioControllerInstance {
            set { this.audioController = value; }
        }

        /// <summary>
        /// Plays the specified sound effect on any available audio channel, if available
        /// </summary>
        public void Play() {
            if (this.audioController != null) {
                this.audioController.Play(this);
            }
        }

        internal void SelfValidate() {
            if (this.Pitch < -3 || this.Pitch > 3) {
                throw new UnityException("Invalid pitch");
            }

            if (this.Volume < 0 || this.Volume > 1) {
                throw new UnityException("Invalid volume");
            }

            if (this.SoundEffect == null || this.SoundEffect.Length == 0) {
                throw new UnityException("No sound effect assigned");
            }
        }
    }

    #endregion
}