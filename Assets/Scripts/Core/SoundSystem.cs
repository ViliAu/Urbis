using System.Collections;
using UnityEngine;

public static class SoundSystem {

    private static GameObject sourceRoot = null;
    private static AudioSource musicSource01 = null;
    private static AudioSource musicSource02 = null;
    private static AudioSource ambianceSource01 = null;
    private static AudioSource ambianceSource02 = null;

    private static float masterVolume = .55f;
    private static float sfxVolume = .7f;
    private static float musicVolume = .65f;
    private static float ambianceVolume = .08f;

    public static void SoundSystemSetup() {
        sourceRoot = new GameObject("Sound Source Root");                   // Spawn a empty gameobject that holds every sound source

        // Make music sources
        musicSource01 = SpawnPermanentSoundSource("Music Source 01", musicVolume);
        musicSource02 = SpawnPermanentSoundSource("Music Source 02", musicVolume);

        // Spawn ambiance sources
        ambianceSource01 = SpawnPermanentSoundSource("Ambiance Source 01", ambianceVolume);
        ambianceSource02 = SpawnPermanentSoundSource("Ambiance Source 02", ambianceVolume);
    }

    static AudioSource SpawnPermanentSoundSource(string name, float volume) {
        GameObject go = new GameObject(name);
        go.transform.SetParent(sourceRoot.transform);
        AudioSource msource = go.AddComponent<AudioSource>();
        msource.spatialBlend = 0;
        msource.volume = masterVolume * volume;
        msource.loop = true;
        return msource;
    }

    static string GetRandomSoundGroupClip(string groupName) {
        // Find the correct sound group
        SoundGroup[] soundGroups = Database.GetSoundGroups();
        for (int i = 0; i < soundGroups.Length; i++) {
            if (soundGroups[i].groupName == groupName) {
                // Play a random sound from the sound group
                int randomIndex = Random.Range(0, soundGroups[i].audioClips.Length);
                if (soundGroups[i].audioClips[randomIndex] != null) {
                    return soundGroups[i].audioClips[randomIndex].name;
                }
            }
        }
        return "";
    }

    

    public static void PlaySound(string soundName, Vector3 soundPosition) {
        PlaySoundClip(soundName, 1f, soundPosition);
    }

    public static void PlaySound(string soundName, Vector3 soundPosition, float volume) {
        PlaySoundClip(soundName, volume, soundPosition);
    }

    public static void PlaySound(string soundName, Vector3 soundPosition, Transform parent) {
        PlaySoundClip(soundName, 1f, soundPosition, parent);
    }

    public static void PlaySound(string soundName, Vector3 soundPosition, float volume, Transform parent) {
        PlaySoundClip(soundName, volume, soundPosition, parent);
    }

    public static void PlaySoundGroup(string groupName, Vector3 soundPosition) {
        PlaySound(GetRandomSoundGroupClip(groupName), soundPosition);
    }

    public static void PlaySoundGroup(string groupName, Vector3 soundPosition, float volume) {
        PlaySound(GetRandomSoundGroupClip(groupName), soundPosition, volume);
    }

    public static void PlaySoundGroup(string groupName, Vector3 soundPosition, Transform parent) {
        PlaySound(GetRandomSoundGroupClip(groupName), soundPosition, parent);
    }

    public static void PlaySoundGroup(string groupName, Vector3 soundPosition, float volume, Transform parent) {
        PlaySound(GetRandomSoundGroupClip(groupName), soundPosition, volume, parent);
    }

    public static void PlaySound2D(string soundName) {
        PlaySoundClip(soundName, 1f);
    }

    public static void PlaySound2D(string soundName, float volume) {
        PlaySoundClip(soundName, volume);
    }

    public static void PlaySoundGroup2D(string groupName) {
        PlaySoundGroup(groupName, Vector3.zero);
    }

    public static void PlaySoundGroup2D(string groupName, float volume) {
        PlaySoundGroup(groupName, Vector3.zero, volume, null);
    }

    static void PlaySoundClip(string soundName, float volume, Vector3 soundPosition = default, Transform parent = null) {
        // Is the sound root initialized?
        if (sourceRoot == null)
            SoundSystemSetup();

        // Is the sound clip we want to play valid ?
        AudioClip clip = Database.GetAudioClip(soundName);
        if (clip == null) {
            Debug.Log("Could not find sound file named: " + soundName);
            return;
        }

        // Create new sound object and attach audio source to it
        GameObject soundObject = new GameObject("Sound Object");
        soundObject.transform.SetParent(parent == null ? sourceRoot.transform : parent);
        AudioSource soundSource = soundObject.AddComponent<AudioSource>();

        // Change sound objects properties
        soundObject.transform.position = soundPosition;

        // Change audio source settings
        soundSource.clip = clip;
        soundSource.volume = masterVolume * sfxVolume * volume;
        // If we're playing a 3D sound set spatial blend to 1
        soundSource.spatialBlend = soundPosition == default ? 0 : 1f;

        soundSource.minDistance = 5f;

        // Playe audio clip
        soundSource.Play();
        StaticCoroutine.Instance.StartCoroutine(DestroySoundObject(soundObject, clip.length));
    }

    public static void PlayMusic(string musicName, float fadeIn) {

        AudioClip musicClip = Database.GetAudioClip(musicName);
        if (musicClip != null) {
            StaticCoroutine.Instance.StartCoroutine(MusicFade(musicClip, fadeIn));
        }
    }

    public static void PlayAmbiance(string ambianceName, float fadeIn) {
        AudioClip ambianceClip = Database.GetAudioClip(ambianceName);
        if (ambianceClip != null) {
            StaticCoroutine.Instance.StartCoroutine(AmbianceFade(ambianceClip, fadeIn));
        }
    }

    static IEnumerator MusicFade(AudioClip musicClip, float fadeIn) {
        // Is the sound root initialized?
        if (sourceRoot == null)
            SoundSystemSetup();

        AudioSource fadeInSource = null;
        AudioSource fadeOutSource = null;
        if (!musicSource01.isPlaying && !musicSource02.isPlaying) {
            fadeInSource = musicSource01;
            fadeOutSource = musicSource02;
        } else {
            if (musicSource01.isPlaying) {
                fadeOutSource = musicSource01;
                fadeInSource = musicSource02;
            } else if (musicSource02.isPlaying) {
                fadeOutSource = musicSource02;
                fadeInSource = musicSource01;
            }
        }

        float t = fadeIn;
        float targetVolume = masterVolume * musicVolume;
        fadeInSource.clip = musicClip;
        fadeInSource.Play();
        while (t > 0) {
            fadeInSource.volume = targetVolume - (targetVolume * (t / fadeIn));
            fadeOutSource.volume = targetVolume * (t / fadeIn);
            t -= Time.deltaTime;
            yield return null;
        }

        fadeInSource.volume = targetVolume;
        fadeOutSource.volume = 0;
        fadeOutSource.Stop();
    }

    static IEnumerator AmbianceFade(AudioClip ambianceClip, float fadeIn) {
        // Is the sound root initialized?
        if (sourceRoot == null)
            SoundSystemSetup();

        AudioSource fadeInSource = null;
        AudioSource fadeOutSource = null;
        if (!ambianceSource01.isPlaying && !ambianceSource02.isPlaying) {
            fadeInSource = ambianceSource01;
            fadeOutSource = ambianceSource02;
        } else {
            if (ambianceSource01.isPlaying) {
                fadeOutSource = ambianceSource01;
                fadeInSource = ambianceSource02;
            } else if (ambianceSource02.isPlaying) {
                fadeOutSource = ambianceSource02;
                fadeInSource = ambianceSource01;
            }
        }

        float t = fadeIn;
        float targetVolume = masterVolume * ambianceVolume;
        fadeInSource.clip = ambianceClip;
        fadeInSource.Play();
        while (t > 0) {
            fadeInSource.volume = targetVolume - (targetVolume * (t / fadeIn));
            fadeOutSource.volume = targetVolume * (t / fadeIn);
            t -= Time.deltaTime;
            yield return null;
        }

        fadeInSource.volume = targetVolume;
        fadeOutSource.volume = 0;
        fadeOutSource.Stop();
    }

    static IEnumerator DestroySoundObject(GameObject soundObject, float delay){
        yield return new WaitForSeconds(delay);
        Object.Destroy(soundObject);
    }

}