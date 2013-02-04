﻿/// <summary>
/// Represents an audio controller for sheep
/// </summary>
public sealed class SheepAudioController : AudioController {
    /// <summary>
    /// Defines the idle blaat/grunt sound
    /// </summary>
    public AudioEffectConfiguration IdleSound;

    /// <summary>
    /// Defines the killed by a wolf sound
    /// </summary>
    public AudioEffectConfiguration KilledSound;

    /// <summary>
    /// Defines the fall in hole sound
    /// </summary>
    public AudioEffectConfiguration FallInHoleSound;

    /// <summary>
    /// Defines the drowning sound
    /// </summary>
    public AudioEffectConfiguration DrowningSound;

    /// <summary>
    /// Defines the fall in water sound
    /// </summary>
    public AudioEffectConfiguration FallInWaterSound;

    /// <summary>
    /// Defines the electric fence touch sound
    /// </summary>
    public AudioEffectConfiguration ElectricFenceTouchSound;
}