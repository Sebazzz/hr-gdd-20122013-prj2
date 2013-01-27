using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public sealed class WorldAudioController : AudioController {
    /// <summary>
    /// This sound plays when all sheep are collected
    /// </summary>
    public AudioEffectConfiguration GameWonSound;

    /// <summary>
    /// This sound plays when not enough sheep are collected
    /// </summary>
    public AudioEffectConfiguration GameLostSound;

}
