using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace Singularity.Sound
{
    public class SoundManager
    {

        // Sound listener for 3D audio.
        private readonly AudioListener _mListener;
        // The z coordinate for 3D audio.
        private float _mSoundPlaneDepth;
        // Dictionary containing all the songs assigned to the level names and paired with phase of the game.
        private readonly Dictionary<string, Song[]> _mAllSongs;
        // Dictionary containing all the sound FX assigned to their individual name.
        private readonly Dictionary<string, SoundEffect> _mEffects;
        // Dictionary containing all the UI sounds assigned to their individual name.
        private readonly Dictionary<string, SoundEffect> _mUiSounds;
        // The name of the current level for accessing the right themes from the dictionary.
        private string _mLevel;
        // Dictionary containing all the instances of any sound FX assigned to an ascending id.
        private Dictionary<int, SoundEffectInstance> _mEffectInstances;
        // Dictionary containing all the instances of any UI sound assigned to an ascending id.
        private Dictionary<int, SoundEffectInstance> _mUiInstances;
        // Ascending id counters for instances of sound FX and UI sounds and a counter for all instances.
        private int _mAllInstanceId;
        private int _mEffectInstanceId;
        private int _mUiInstanceId;
        // Dictionary that maps the mAllInstanceId to a pair of SoundClass and m<Class>Id.
        private Dictionary<int, Tuple<SoundClass, int>> _mInstanceMap;

        #region AudioData

        /// <summary>
        /// Looping song used as the in-game soundtrack
        /// </summary>
        private static Song _sSoundtrack;

        #endregion

        #region Initialization Methods

        /// <summary>
        /// Constructs the manager for audio playback of all sound effects.
        /// </summary>
        public SoundManager()
        {
            MediaPlayer.IsRepeating = true;
            SoundEffect.DistanceScale = (float)0.5;
            _mListener = new AudioListener();
            _mSoundPlaneDepth = 0;
            _mAllSongs = new Dictionary<string, Song[]>();
            _mEffects = new Dictionary<string, SoundEffect>();
            _mUiSounds = new Dictionary<string, SoundEffect>();
            _mEffectInstances = new Dictionary<int, SoundEffectInstance>();
            _mUiInstances = new Dictionary<int, SoundEffectInstance>();
            _mLevel = String.Empty;
            _mEffectInstanceId = 0;
            _mUiInstanceId = 0;
            _mAllInstanceId = 0;
            _mInstanceMap = new Dictionary<int, Tuple<SoundClass, int>>();
        }

        public void LoadContent(ContentManager contentManager)
        {
            // Load all sound files from the directory.
            foreach (string s in Directory.GetFiles(@"Content\Sound", "*.xnb"))
            {
                // Cut off file ending and determine game phase.
                string fullName = s.Substring(8);
                int endPos = fullName.LastIndexOf(".", StringComparison.Ordinal);
                int phasePos = fullName.LastIndexOf("_", StringComparison.Ordinal);
                int levelPos = fullName.LastIndexOf(@"\", StringComparison.Ordinal);
                fullName = fullName.Substring(0, endPos);
                string levelName = fullName.Substring(levelPos + 1, phasePos - levelPos - 1);
                string phase = fullName.Substring(phasePos + 1);
                Console.WriteLine($"Loading theme {levelName} for game phase {phase} from {fullName}");
                Song song = contentManager.Load<Song>(fullName);
                if (!_mAllSongs.ContainsKey(levelName))
                {
                    _mAllSongs[levelName] = new Song[3];
                }

                if (phase == "Menu")
                {
                    _mAllSongs[levelName][(int) SoundPhase.Menu] = song;
                }

                if (phase == "Build")
                {
                    _mAllSongs[levelName][(int) SoundPhase.Build] = song;
                }

                if (phase == "Battle")
                {
                    _mAllSongs[levelName][(int) SoundPhase.Battle] = song;
                }
            }

            foreach (string s in Directory.GetFiles(@"Content\Sound\SFX", "*.xnb"))
            {
                string fullName = s.Substring(8);
                int endPos = fullName.LastIndexOf(".", StringComparison.Ordinal);
                fullName = fullName.Substring(0, endPos);
                SoundEffect effect = contentManager.Load<SoundEffect>(fullName);
                string effectName = fullName.Substring(fullName.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
                _mEffects[effectName] = effect;
                Console.WriteLine($"Loaded {effectName}");
            }

            foreach (string s in Directory.GetFiles(@"Content\Sound\UI", "*.xnb"))
            {
                string fullName = s.Substring(8);
                int endPos = fullName.LastIndexOf(".", StringComparison.Ordinal);
                fullName = fullName.Substring(0, endPos);
                SoundEffect effect = contentManager.Load<SoundEffect>(fullName);
                string effectName = fullName.Substring(fullName.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
                _mEffects[effectName] = effect;
                Console.WriteLine($"Loaded {effectName}");
            }

            //sSoundtrack = contentManager.Load<Song>("BGmusic");
            _sSoundtrack = _mAllSongs["Tutorial"][(int) SoundPhase.Build];
        }

        #endregion

        public void PlaySoundTrack()
        {
            if (_sSoundtrack == null)
                return;

            MediaPlayer.Play(_sSoundtrack);
        }


        /// <summary>
        /// Adds an instance of the sound effect with the given properties to the Dictionary containing all instances and plays it.
        /// </summary>
        /// <param name="name">The sound effect's name.</param>
        /// <param name="x">x coordinate where the sound is supposed to be played.</param>
        /// <param name="y">y coordinate where the sound is supposed to be played.</param>
        /// <param name="volume">The supposed sound volume.</param>
        /// <param name="pitch">The pitch modificator.</param>
        /// <param name="isGlobal">Specifies whether the sound has a position.</param>
        /// <param name="loop">Specifies whether the sound is supposed to loop.</param>
        /// <param name="soundClass">Specifies the SoundClass e.g. Effect.</param>
        /// <returns>The id of the instance if params are valid. -1 if the SoundClass is invalid. -2 if the effect doesn't exist.</returns>
        public int PlaySound(string name, float x, float y, float volume, float pitch, bool isGlobal, bool loop, SoundClass soundClass)
        {
            if (!_mEffects.ContainsKey(name))
            {
                return -2;
            }
            if (soundClass == SoundClass.Effect)
            {
                SoundEffectInstance effectInstance = _mEffects[name].CreateInstance();
                effectInstance.Volume = volume;
                effectInstance.Pitch = pitch;
                effectInstance.IsLooped = loop;
                if (!isGlobal)
                {
                    AudioEmitter emitter = new AudioEmitter { Position = new Vector3(x, y, _mSoundPlaneDepth) };
                    effectInstance.Apply3D(_mListener, emitter);
                }
                _mEffectInstances.Add(_mEffectInstanceId, effectInstance);
                _mEffectInstances[_mEffectInstanceId].Play();
                _mInstanceMap.Add(_mAllInstanceId, new Tuple<SoundClass, int>(soundClass, _mEffectInstanceId));
                _mEffectInstanceId++;
                return _mAllInstanceId++;
            }
            else if (soundClass == SoundClass.Ui)
            {
                SoundEffectInstance effectInstance = _mUiSounds[name].CreateInstance();
                effectInstance.Volume = volume;
                effectInstance.Pitch = pitch;
                effectInstance.IsLooped = loop;
                if (!isGlobal)
                {
                    AudioEmitter emitter = new AudioEmitter { Position = new Vector3(x, y, _mSoundPlaneDepth) };
                    effectInstance.Apply3D(_mListener, emitter);
                }
                _mUiInstances.Add(_mEffectInstanceId, effectInstance);
                _mUiInstances[_mEffectInstanceId].Play();
                _mInstanceMap.Add(_mAllInstanceId, new Tuple<SoundClass, int>(soundClass, _mUiInstanceId));
                _mUiInstanceId++;
                return _mAllInstanceId++;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Sets the volume of the sound effect instance specified by the given id.
        /// </summary>
        /// <param name="id">The global sound instance id.</param>
        /// <param name="volume">The desired volume.</param>
        public void SetSoundVolume(int id, float volume)
        {
            if (_mInstanceMap.ContainsKey(id))
            {
                SoundClass soundClass = _mInstanceMap[id].Item1;
                int instanceId = _mInstanceMap[id].Item2;
                if (soundClass == SoundClass.Effect)
                {
                    _mEffectInstances[instanceId].Volume = volume;
                }
                else if (soundClass == SoundClass.Ui)
                {
                    _mUiInstances[instanceId].Volume = volume;
                }
            }
        }

        /// <summary>
        /// Setting the position of the Sound Listener used for 3D sound. Uses the given x and y coordinates and the mSoundPlanDepth for the z coordinate.
        /// </summary>
        /// <param name="x">New x coordinate of the listener.</param>
        /// <param name="y">New y coordinate of the listener.</param>
        public void SetListenerPosition(float x, float y)
        {
            _mListener.Position = new Vector3(x, y, _mSoundPlaneDepth);
        }

        /// <summary>
        /// Change the position of a 3D sound effect instance.
        /// </summary>
        /// <param name="id">The gloabl id of the sound effect instance.</param>
        /// <param name="x">The new x coordinate.</param>
        /// <param name="y">The new y coordinate.</param>
		public void SetSoundPosition(int id, float x, float y)
        {
            if (_mInstanceMap.ContainsKey(id))
            {
                SoundClass soundClass = _mInstanceMap[id].Item1;
                int instanceId = _mInstanceMap[id].Item2;
                if (soundClass == SoundClass.Effect)
                {
                    AudioEmitter emitter = new AudioEmitter { Position = new Vector3(x, y, _mSoundPlaneDepth) };
                    _mEffectInstances[instanceId].Apply3D(_mListener, emitter);
                }
                else if (soundClass == SoundClass.Ui)
                {
                    AudioEmitter emitter = new AudioEmitter { Position = new Vector3(x, y, _mSoundPlaneDepth) };
                    _mUiInstances[instanceId].Apply3D(_mListener, emitter);
                }
            }
        }

        /// <summary>
        /// Set the pitch of a sound effect instance.
        /// </summary>
        /// <param name="id">The gloabl id of the sound effect instance.</param>
        /// <param name="pitch">The new pitch.</param>
	    public void SetSoundPitch(int id, float pitch)
        {
            if (_mInstanceMap.ContainsKey(id))
            {
                SoundClass soundClass = _mInstanceMap[id].Item1;
                int instanceId = _mInstanceMap[id].Item2;
                if (soundClass == SoundClass.Effect)
                {
                    _mEffectInstances[instanceId].Pitch = pitch;
                }
                else if (soundClass == SoundClass.Ui)
                {
                    _mUiInstances[instanceId].Pitch = pitch;
                }
            }
        }

        /// <summary>
        /// Stops the playback of an individual sound - SFX or UI.
        /// </summary>
        /// <param name="id">The id of the sound to be stopped.</param>
        public void StopSound(int id)
        {
            if (_mInstanceMap.ContainsKey(id))
            {
                SoundClass soundClass = _mInstanceMap[id].Item1;
                if (soundClass == SoundClass.Effect)
                {
                    _mEffectInstances[_mInstanceMap[id].Item2].Stop();
                }
                else if (soundClass == SoundClass.Ui)
                {
                    _mUiInstances[_mInstanceMap[id].Item2].Stop();
                }
            }
        }

        /// <summary>
        /// Stopping all the currently playing sounds of a given class.
        /// </summary>
        /// <param name="soundClass">The class to be stopped.</param>
        public void StopSoundClass(SoundClass soundClass)
        {
            if (soundClass == SoundClass.Music)
            {
                if (MediaPlayer.State == MediaState.Playing)
                {
                    MediaPlayer.Stop();
                }
            }
            else if (soundClass == SoundClass.Effect)
            {
                for (int i = 0; i <= _mEffectInstanceId; i++)
                {
                    if (_mEffectInstances.ContainsKey(i))
                    {
                        _mEffectInstances[i].Stop();
                    }
                }
            }
            else if (soundClass == SoundClass.Ui)
            {
                for (int i = 0; i <= _mEffectInstanceId; i++)
                {
                    if (_mUiInstances.ContainsKey(i))
                    {
                        _mUiInstances[i].Stop();
                    }
                }
            }
        }

        /// <summary>
        /// Pause the currently playing sounds of the given class. 
        /// </summary>
        /// <param name="soundClass">The SoundClass to pause.</param>
		public void PauseSoundClass(SoundClass soundClass)
        {
            if (soundClass == SoundClass.Music)
            {
                if (MediaPlayer.State == MediaState.Playing)
                {
                    MediaPlayer.Pause();
                }
            }
            if (soundClass == SoundClass.Effect)
            {
                for (int i = 0; i <= _mEffectInstanceId; i++)
                {
                    if (_mEffectInstances.ContainsKey(i))
                    {
                        _mEffectInstances[i].Pause();
                    }
                }
            }
            if (soundClass == SoundClass.Ui)
            {
                for (int i = 0; i <= _mUiInstanceId; i++)
                {
                    if (_mUiInstances.ContainsKey(i))
                    {
                        _mUiInstances[i].Pause();
                    }
                }
            }
        }

        /// <summary>
        /// Resume playing the paused sounds of the given Sound Class.
        /// </summary>
        /// <param name="soundClass">The Sound Class to resume playing.</param>
		public void ResumeSoundClass(SoundClass soundClass)
        {
            if (MediaPlayer.State == MediaState.Paused)
            {
                MediaPlayer.Resume();
            }
            if (soundClass == SoundClass.Effect)
            {
                for (int i = 0; i <= _mEffectInstanceId; i++)
                {
                    if (_mEffectInstances.ContainsKey(i))
                    {
                        if (_mEffectInstances[i].State == SoundState.Paused)
                        {
                            _mEffectInstances[i].Play();
                        }
                    }
                }
            }
            if (soundClass == SoundClass.Ui)
            {
                for (int i = 0; i <= _mUiInstanceId; i++)
                {
                    if (_mUiInstances.ContainsKey(i))
                    {
                        if (_mUiInstances[i].State == SoundState.Paused)
                        {
                            _mUiInstances[i].Play();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Setting the music according to the game phase.
        /// </summary>
        /// <param name="soundPhase">The desired mood.</param>
        public void SetSoundPhase(SoundPhase soundPhase)
        {
            if (MediaPlayer.State == MediaState.Playing)
            {
                MediaPlayer.Stop();
            }
            if (_mLevel == String.Empty)
            {
                // TODO: Maybe play random background music.
            }
            MediaPlayer.Play(_mAllSongs[_mLevel][(int)soundPhase]);
        }

        /// <summary>
        /// Setting the theme song for the current level by specifying the current level. String.Empty() for random music from pool.
        /// </summary>
        /// <param name="name">The level name.</param>
		public void SetLevelThemeMusic(string name)
        {
            _mLevel = name;
        }

        /// <summary>
        /// To be called within a level or menu. Deletes all the stopped (disposable) instances.
        /// </summary>
	    public void CleanUpStoppedInstances()
        {
            Dictionary<int, SoundEffectInstance>[] dicts = { _mEffectInstances, _mUiInstances };
            foreach (Dictionary<int, SoundEffectInstance> dict in dicts)
            {
                foreach (KeyValuePair<int, SoundEffectInstance> pair in dict)
                {
                    if (pair.Value.State == SoundState.Stopped)
                    {
                        _mInstanceMap.Remove(pair.Key);
                        dict.Remove(pair.Key);
                    }
                }
            }
        }


        /// <summary>
        /// To be called after each level or menu. Disposes all the SFX and UI instances and resets the id counters.
        /// </summary>
        public void CleanUpLevelInstances()
        {
            _mInstanceMap = new Dictionary<int, Tuple<SoundClass, int>>();
            _mEffectInstances = new Dictionary<int, SoundEffectInstance>();
            _mUiInstances = new Dictionary<int, SoundEffectInstance>();
            _mAllInstanceId = 0;
            _mEffectInstanceId = 0;
            _mUiInstanceId = 0;
        }


        /// <summary>
        /// Set the z coordinate for all 3D sounds (effectivly the distance of the sound plane from the listener).
        /// </summary>
        /// <param name="z">The desired z coordinate.</param>
	    public void SetSoundPlaneDepth(float z)
        {
            _mSoundPlaneDepth = z;
        }
    } /* end class SoundManager */

}
