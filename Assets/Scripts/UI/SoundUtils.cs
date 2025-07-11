using UnityEngine;

public static class SoundUtils
{
    public static void PlaySoundAndDestroy(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null) return;

        GameObject tempGO = new GameObject("TempAudio_" + Random.Range(0, 100000));
        AudioSource aSource = tempGO.AddComponent<AudioSource>();
        aSource.clip = clip;
        aSource.volume = volume;
        aSource.Play();

        Object.DontDestroyOnLoad(tempGO);
        Object.Destroy(tempGO, clip.length);
    }

}
