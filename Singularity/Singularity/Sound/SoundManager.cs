using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace Singularity.Sound
{
    public class SoundManager
    {

        // Sound listener for 3D audio.
        private readonly AudioListener mListener;
        // The z coordinate for 3D audio.
        private float mSoundPlaneDepth;
        // Dictionary containing all the songs assigned to the level names and paired with phase of the game.
        private readonly Dictionary<string, Song[]> mAllSongs;
        // Dictionary containing all the sound FX assigned to their individual name.
        private readonly Dictionary<string, SoundEffect> mEffects;
        // Dictionary containing all the UI sounds assigned to their individual name.
        private readonly Dictionary<string, SoundEffect> mUiSounds;
        // The name of the current level for accessing the right themes from the dictionary.
        private string mLevel;
        // Dictionary containing all the instances of any sound FX assigned to an ascending id.
        private Dictionary<int, SoundEffectInstance> mEffectInstances;
        // Dictionary containing all the instances of any UI sound assigned to an ascending id.
        private Dictionary<int, SoundEffectInstance> mUiInstances;
        // Ascending id counters for instances of sound FX and UI sounds and a counter for all instances.
        private int mAllInstanceId;
        private int mEffectInstanceId;
        private int mUiInstanceId;
        // Dictionary that maps the mAllInstanceId to a pair of SoundClass and m<Class>Id.
        private Dictionary<int, Tuple<SoundClass, int>> mInstanceMap;

        #region AudioData

        /// <summary>
        /// Looping song used as the in-game soundtrack
        /// </summary>
        private static Song sSSoundtrack;

        #endregion

        #region Initialization Methods

        /// <summary>
        /// Constructs the manager for audio playback of all sound effects.
        /// </summary>
        public SoundManager()
        {
            MediaPlayer.IsRepeating = true;
            SoundEffect.DistanceScale = (float)0.5;
            mListener = new AudioListener();
            mSoundPlaneDepth = 0;
            mAllSongs = new Dictionary<string, Song[]>();
            mEffects = new Dictionary<string, SoundEffect>();
            mUiSounds = new Dictionary<string, SoundEffect>();
            mEffectInstances = new Dictionary<int, SoundEffectInstance>();
            mUiInstances = new Dictionary<int, SoundEffectInstance>();
            mLevel = String.Empty;
            mEffectInstanceId = 0;
            mUiInstanceId = 0;
            mAllInstanceId = 0;
            mInstanceMap = new Dictionary<int, Tuple<SoundClass, int>>();
        }

        public void LoadContent(ContentManager contentManager)
        {
            // Load all sound files from the directory.
            foreach (string s in Directory.GetFiles(path: @"Content\Sound", searchPattern: "*.xnb"))
            {
                // Cut off file ending and determine game phase.
                string fullName = s.Substring(startIndex: 8);
                int endPos = fullName.LastIndexOf(value: ".", comparisonType: StringComparison.Ordinal);
                int phasePos = fullName.LastIndexOf(value: "_", comparisonType: StringComparison.Ordinal);
                int levelPos = fullName.LastIndexOf(value: @"\", comparisonType: StringComparison.Ordinal);
                fullName = fullName.Substring(startIndex: 0, length: endPos);
                string levelName = fullName.Substring(startIndex: levelPos + 1, length: phasePos - levelPos - 1);
                string phase = fullName.Substring(startIndex: phasePos + 1);
                Console.WriteLine(value: $"Loading theme {levelName} for game phase {phase} from {fullName}");
                Song song = contentManager.Load<Song>(assetName: fullName);
                if (!mAllSongs.ContainsKey(key: levelName))
                {
                    mAllSongs[key: levelName] = new Song[3];
                }

                if (phase == "Menu")
                {
                    mAllSongs[key: levelName][(int) SoundPhase.Menu] = song;
                }

                if (phase == "Build")
                {
                    mAllSongs[key: levelName][(int) SoundPhase.Build] = song;
                }

                if (phase == "Battle")
                {
                    mAllSongs[key: levelName][(int) SoundPhase.Battle] = song;
                }
            }

            foreach (string s in Directory.GetFiles(path: @"Content\Sound\SFX", searchPattern: "*.xnb"))
            {
                string fullName = s.Substring(startIndex: 8);
                int endPos = fullName.LastIndexOf(value: ".", comparisonType: StringComparison.Ordinal);
                fullName = fullName.Substring(startIndex: 0, length: endPos);
                SoundEffect effect = contentManager.Load<SoundEffect>(assetName: fullName);
                string effectName = fullName.Substring(startIndex: fullName.LastIndexOf(value: @"\", comparisonType: StringComparison.Ordinal) + 1);
                mEffects[key: effectName] = effect;
                Console.WriteLine(value: $"Loaded {effectName}");
            }

            foreach (string s in Directory.GetFiles(path: @"Content\Sound\UI", searchPattern: "*.xnb"))
            {
                string fullName = s.Substring(startIndex: 8);
                int endPos = fullName.LastIndexOf(value: ".", comparisonType: StringComparison.Ordinal);
                fullName = fullName.Substring(startIndex: 0, length: endPos);
                SoundEffect effect = contentManager.Load<SoundEffect>(assetName: fullName);
                string effectName = fullName.Substring(startIndex: fullName.LastIndexOf(value: @"\", comparisonType: StringComparison.Ordinal) + 1);
                mEffects[key: effectName] = effect;
                Console.WriteLine(value: $"Loaded {effectName}");
            }

            //sSoundtrack = contentManager.Load<Song>("BGmusic");
            sSSoundtrack = mAllSongs[key: "Tutorial"][(int) SoundPhase.Build];
        }

        #endregion

        public void PlaySoundTrack()
        {
            if (sSSoundtrack == null)
            {
                return;
            }

            MediaPlayer.Play(song: sSSoundtrack);
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
            if (!mEffects.ContainsKey(key: name))
            {
                return -2;
            }
            if (soundClass == SoundClass.Effect)
            {
                SoundEffectInstance effectInstance = mEffects[key: name].CreateInstance();
                effectInstance.Volume = volume;
                effectInstance.Pitch = pitch;
                effectInstance.IsLooped = loop;
                if (!isGlobal)
                {
                    AudioEmitter emitter = new AudioEmitter { Position = new Vector3(x: x, y: y, z: mSoundPlaneDepth) };
                    effectInstance.Apply3D(listener: mListener, emitter: emitter);
                }
                mEffectInstances.Add(key: mEffectInstanceId, value: effectInstance);
                mEffectInstances[key: mEffectInstanceId].Play();
                mInstanceMap.Add(key: mAllInstanceId, value: new Tuple<SoundClass, int>(item1: soundClass, item2: mEffectInstanceId));
                mEffectInstanceId++;
                return mAllInstanceId++;
            }
            if (soundClass == SoundClass.Ui)
            {
                SoundEffectInstance effectInstance = mUiSounds[key: name].CreateInstance();
                effectInstance.Volume = volume;
                effectInstance.Pitch = pitch;
                effectInstance.IsLooped = loop;
                if (!isGlobal)
                {
                    AudioEmitter emitter = new AudioEmitter { Position = new Vector3(x: x, y: y, z: mSoundPlaneDepth) };
                    effectInstance.Apply3D(listener: mListener, emitter: emitter);
                }
                mUiInstances.Add(key: mEffectInstanceId, value: effectInstance);
                mUiInstances[key: mEffectInstanceId].Play();
                mInstanceMap.Add(key: mAllInstanceId, value: new Tuple<SoundClass, int>(item1: soundClass, item2: mUiInstanceId));
                mUiInstanceId++;
                return mAllInstanceId++;
            }
            return -1;
        }

        /// <summary>
        /// Sets the volume of the sound effect instance specified by the given id.
        /// </summary>
        /// <param name="id">The global sound instance id.</param>
        /// <param name="volume">The desired volume.</param>
        public void SetSoundVolume(int id, float volume)
        {
            if (mInstanceMap.ContainsKey(key: id))
            {
                SoundClass soundClass = mInstanceMap[key: id].Item1;
                int instanceId = mInstanceMap[key: id].Item2;
                if (soundClass == SoundClass.Effect)
                {
                    mEffectInstances[key: instanceId].Volume = volume;
                }
                else if (soundClass == SoundClass.Ui)
                {
                    mUiInstances[key: instanceId].Volume = volume;
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
            mListener.Position = new Vector3(x: x, y: y, z: mSoundPlaneDepth);
        }

        /// <summary>
        /// Change the position of a 3D sound effect instance.
        /// </summary>
        /// <param name="id">The gloabl id of the sound effect instance.</param>
        /// <param name="x">The new x coordinate.</param>
        /// <param name="y">The new y coordinate.</param>
        public void SetSoundPosition(int id, float x, float y)
        {
            if (mInstanceMap.ContainsKey(key: id))
            {
                SoundClass soundClass = mInstanceMap[key: id].Item1;
                int instanceId = mInstanceMap[key: id].Item2;
                if (soundClass == SoundClass.Effect)
                {
                    AudioEmitter emitter = new AudioEmitter { Position = new Vector3(x: x, y: y, z: mSoundPlaneDepth) };
                    mEffectInstances[key: instanceId].Apply3D(listener: mListener, emitter: emitter);
                }
                else if (soundClass == SoundClass.Ui)
                {
                    AudioEmitter emitter = new AudioEmitter { Position = new Vector3(x: x, y: y, z: mSoundPlaneDepth) };
                    mUiInstances[key: instanceId].Apply3D(listener: mListener, emitter: emitter);
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
            if (mInstanceMap.ContainsKey(key: id))
            {
                SoundClass soundClass = mInstanceMap[key: id].Item1;
                int instanceId = mInstanceMap[key: id].Item2;
                if (soundClass == SoundClass.Effect)
                {
                    mEffectInstances[key: instanceId].Pitch = pitch;
                }
                else if (soundClass == SoundClass.Ui)
                {
                    mUiInstances[key: instanceId].Pitch = pitch;
                }
            }
        }

        /// <summary>
        /// Stops the playback of an individual sound - SFX or UI.
        /// </summary>
        /// <param name="id">The id of the sound to be stopped.</param>
        public void StopSound(int id)
        {
            if (mInstanceMap.ContainsKey(key: id))
            {
                SoundClass soundClass = mInstanceMap[key: id].Item1;
                if (soundClass == SoundClass.Effect)
                {
                    mEffectInstances[key: mInstanceMap[key: id].Item2].Stop();
                }
                else if (soundClass == SoundClass.Ui)
                {
                    mUiInstances[key: mInstanceMap[key: id].Item2].Stop();
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
                for (int i = 0; i <= mEffectInstanceId; i++)
                {
                    if (mEffectInstances.ContainsKey(key: i))
                    {
                        mEffectInstances[key: i].Stop();
                    }
                }
            }
            else if (soundClass == SoundClass.Ui)
            {
                for (int i = 0; i <= mEffectInstanceId; i++)
                {
                    if (mUiInstances.ContainsKey(key: i))
                    {
                        mUiInstances[key: i].Stop();
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
                for (int i = 0; i <= mEffectInstanceId; i++)
                {
                    if (mEffectInstances.ContainsKey(key: i))
                    {
                        mEffectInstances[key: i].Pause();
                    }
                }
            }
            if (soundClass == SoundClass.Ui)
            {
                for (int i = 0; i <= mUiInstanceId; i++)
                {
                    if (mUiInstances.ContainsKey(key: i))
                    {
                        mUiInstances[key: i].Pause();
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
                for (int i = 0; i <= mEffectInstanceId; i++)
                {
                    if (mEffectInstances.ContainsKey(key: i))
                    {
                        if (mEffectInstances[key: i].State == SoundState.Paused)
                        {
                            mEffectInstances[key: i].Play();
                        }
                    }
                }
            }
            if (soundClass == SoundClass.Ui)
            {
                for (int i = 0; i <= mUiInstanceId; i++)
                {
                    if (mUiInstances.ContainsKey(key: i))
                    {
                        if (mUiInstances[key: i].State == SoundState.Paused)
                        {
                            mUiInstances[key: i].Play();
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
            if (mLevel == String.Empty)
            {
                // TODO: Maybe play random background music.
            }
            MediaPlayer.Play(song: mAllSongs[key: mLevel][(int)soundPhase]);
        }

        /// <summary>
        /// Setting the theme song for the current level by specifying the current level. String.Empty() for random music from pool.
        /// </summary>
        /// <param name="name">The level name.</param>
        public void SetLevelThemeMusic(string name)
        {
            mLevel = name;
        }

        /// <summary>
        /// To be called within a level or menu. Deletes all the stopped (disposable) instances.
        /// </summary>
        public void CleanUpStoppedInstances()
        {
            Dictionary<int, SoundEffectInstance>[] dicts = { mEffectInstances, mUiInstances };
            foreach (Dictionary<int, SoundEffectInstance> dict in dicts)
            {
                foreach (KeyValuePair<int, SoundEffectInstance> pair in dict)
                {
                    if (pair.Value.State == SoundState.Stopped)
                    {
                        mInstanceMap.Remove(key: pair.Key);
                        dict.Remove(key: pair.Key);
                    }
                }
            }
        }


        /// <summary>
        /// To be called after each level or menu. Disposes all the SFX and UI instances and resets the id counters.
        /// </summary>
        public void CleanUpLevelInstances()
        {
            mInstanceMap = new Dictionary<int, Tuple<SoundClass, int>>();
            mEffectInstances = new Dictionary<int, SoundEffectInstance>();
            mUiInstances = new Dictionary<int, SoundEffectInstance>();
            mAllInstanceId = 0;
            mEffectInstanceId = 0;
            mUiInstanceId = 0;
        }


        /// <summary>
        /// Set the z coordinate for all 3D sounds (effectivly the distance of the sound plane from the listener).
        /// </summary>
        /// <param name="z">The desired z coordinate.</param>
        public void SetSoundPlaneDepth(float z)
        {
            mSoundPlaneDepth = z;
        }
    } /* end class SoundManager */

}
