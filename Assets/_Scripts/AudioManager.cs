using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    Music,
    Sfx
}

public class AudioManager : PersistentSingletion<AudioManager> 
{
    [SerializeField] AudioSource SFXSourcePrefab;
    [SerializeField] AudioSource musicSourcePrefab;

    



    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        GameSceneManager.Instance.ENewSceneLoaded += PlaySceneMusic;

    }

    void PlaySceneMusic(SceneData data)
    {
        if(data.BGMusic!= null)
        {
            PlaySound(data.BGMusic, data.Volume, SoundType.Music);
        }
    }

    public AudioSource PlaySound(AudioClip clip, float volume, SoundType type, float duration = 0)
    {
        AudioSource source = null;
        if(type == SoundType.Music)
        {
            source = Instantiate(musicSourcePrefab, transform.position, Quaternion.identity);
        }
        else if(type == SoundType.Sfx)
        {
            source = Instantiate(SFXSourcePrefab, transform.position, Quaternion.identity);
        }
        source.clip = clip;
        source.volume = volume;
        source.Play();
        float d = duration;
        if (duration <= 0) {d = clip.length; }
        if(type != SoundType.Music)
            Destroy(source.gameObject, d);
        return source;
    }

    public AudioSource PlayRandomSound(AudioClip[] audioClips, float volume, SoundType type, float duration = 0)
    {
       return PlaySound(audioClips.Choice(), volume, type,duration);
    }


}
