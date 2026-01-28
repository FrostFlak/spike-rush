using System;
using System.Collections.Generic;
using Alchemy.Serialization;
using Helpers;
using UnityEngine;

namespace Game {
    [AlchemySerialize]
    public partial class AudioController : SingletonMonoBehaviour<AudioController> {

        [Header("SFX")]
        [SerializeField] private AudioSource _sfxSource;
        [AlchemySerializeField, NonSerialized] private Dictionary<BreakSFX, List<AudioClip>> _breakClips;
        [Header("Music")]
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private List<AudioClip> _musicClips;
            
        public enum BreakSFX {
            GlassBreak,
            MetalBreak,
            WoodBreak,
            SlimeBreak,
            StoneBreak,
            CartonBreak,
        }

        public void PlayBreak(BreakSFX sfx, bool randomPitch = false) {
            if (randomPitch)
                _sfxSource.pitch = UnityEngine.Random.Range(.93f, 1.07f);

            _sfxSource.PlayOneShot(_breakClips[sfx].GetRandom());
            _sfxSource.pitch = 1f;
        }
    }
}