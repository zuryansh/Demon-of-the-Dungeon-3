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

    public void PlaySound(AudioClip clip, float volume, SoundType type)
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
        Destroy(source.gameObject, clip.length);
    }

    public void PlayRandomSound(AudioClip[] audioClips, float volume, SoundType type)
    {
        PlaySound(audioClips.Choice(), volume, type);
    }


}
