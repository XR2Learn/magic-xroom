/*********** Copyright © 2024 University of Applied Sciences of Southern Switzerland (SUPSI) ***********\
 
 Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
 associated documentation files (the "Software"), to deal in the Software without restriction,
 including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
 subject to the following conditions:

 The above copyright notice and this permission notice shall be included in all copies or substantial
 portions of the Software.

 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT
 LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
 SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

\*******************************************************************************************************/


using System;
using System.Collections.Generic;
using UnityEngine;

namespace XR2Learn.Common
{

    public class SoundManager : MonoBehaviour
    {
        [Serializable]
        public struct Audio
        {
            [SerializeField]
            public string name;
            [SerializeField]
            public AudioSource source;
        }

        [SerializeField]
        public List<Audio> sounds;

        private static Dictionary<string, AudioSource> _sounds;

        public void Awake()
        {
            _sounds = new();
            foreach (Audio audio in sounds)
            {
                if (_sounds.ContainsKey(audio.name))
                    throw new Exception("[SoundManager] Duplicate AudioSource name: " + audio.name);
                _sounds.Add(audio.name, audio.source);
            }
        }

        public static void PlaySound(string sound, bool reverse = false)
        {
            if (_sounds.ContainsKey(sound))
            {
                AudioSource audio = _sounds[sound];
                if (reverse)
                {
                    audio.pitch = -1;
                    audio.time = audio.clip.length - 0.01f;
                }
                else
                {
                    audio.pitch = 1;
                    audio.time = 0;
                }
                _sounds[sound].Play();
            }
            else
            {
                Debug.LogWarning("Attempting to play unknown audio " + sound);
            }
        }
    }
}
