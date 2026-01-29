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
        [AlchemySerializeField, NonSerialized] private Dictionary<EnvironmentSFX, AudioClip> _envSfx;
        [AlchemySerializeField, NonSerialized] private Dictionary<UISFX, AudioClip> _uiSfx;
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
        
        public enum EnvironmentSFX {
            WaterSplash,
            BoostPlatform
        }
        
        public enum UISFX {
            MultipleCoins,
            Switch,
            PopClick,
            Upgrade,
        }

        public void PlayBreak(BreakSFX sfx, bool randomPitch = false) => Play(_breakClips[sfx].GetRandom(), randomPitch);
        
        public void PlayEnvironment(EnvironmentSFX sfx, bool randomPitch = false) => Play(_envSfx[sfx], randomPitch);
        
        public void PlayUI(UISFX sfx, bool randomPitch = false) => Play(_uiSfx[sfx], randomPitch);

        private void Play(AudioClip clip, bool randomPitch) {
            if (randomPitch)
                _sfxSource.pitch = UnityEngine.Random.Range(.93f, 1.07f);
            
            _sfxSource.PlayOneShot(clip);
            _sfxSource.pitch = 1f;
        }
    }
}