using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SoftKitty
{
    public class SoundManager : MonoBehaviour
    {
        #region Variables
        private static SoundManager instance;
        private static Dictionary<string, AudioClip> SoundDic = new Dictionary<string, AudioClip>();
        public AudioSource SoundPlayer;
        #endregion

        #region Internal Methods
        private void Awake()
        {
            instance = this;
            SoundDic.Clear();
        }

        private static void CreateInstance()
        {
            GameObject newObj = Instantiate(Resources.Load<GameObject>("SoftKittyShared/SoundManager"));
            newObj.transform.position = Vector3.zero;
            newObj.transform.localScale = Vector3.one;
            instance = newObj.GetComponent<SoundManager>();
        }


        public void PlaySound3D(string _soundName, Vector3 _position, float _volumn)
        {
            try
            {
                AudioSource.PlayClipAtPoint(GetClip(_soundName), _position, _volumn);
            }
            catch
            {

            }
        }

        public void PlaySound2D(string _soundName, float _volumn)
        {
            try
            {
                SoundPlayer.PlayOneShot(GetClip(_soundName), _volumn);
            }
            catch
            {

            }
        }

        public AudioClip GetClip(string _soundName)
        {
            if (SoundDic.ContainsKey(_soundName))
            {
                return SoundDic[_soundName];
            }
            else
            {
                try
                {
                    AudioClip _clip = Resources.Load<AudioClip>("Sounds/" + _soundName);
                    if (_clip != null)
                    {
                        SoundDic.Add(_soundName, _clip);
                        return _clip;
                    }
                }
                catch
                {
                    return null;
                }

            }
            return null;
        }
        #endregion


        //Plays a 2D sound with the provided audio clip name
        public static void Play2D(string _soundName, float _volumn=1F)
        {
            if (instance == null)
            {
                CreateInstance();
            }
            instance.PlaySound2D(_soundName, _volumn);
        }

        //Plays a 3D sound at the specified position using the provided audio clip name.
        public static void Play3D(string _soundName, Vector3 _position, float _volumn = 1F)
        {
            if (instance == null)
            {
                CreateInstance();
            }
            instance.PlaySound3D(_soundName, _position, _volumn);
        }

        //In order to save performance, after playing an AudioClip,
        //the script will keep it in memory because most likely it going to be played again later.
        //Calling this will clear all those data to release memory.
        public static void ClearSoundCache() 
        {
            SoundDic.Clear();
        }



    }
}
