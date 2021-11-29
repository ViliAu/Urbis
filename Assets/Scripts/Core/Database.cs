using System.Collections.Generic;
using UnityEngine;

public static class Database {
    
    private static Dictionary<string, AudioClip> audioClips = null;
    private static Dictionary<string, Entity> entities = null;
    private static Dictionary<string, Sprite> crosshairs = null;
    private static Dictionary<string, Sprite> icons = null;

    private static SoundGroup[] soundGroups = null;

    public static void LoadAudioClips() {
        AudioClip[] clips = Resources.LoadAll<AudioClip>("Audio");
        audioClips = new Dictionary<string, AudioClip>();
        int counter = 0;
        for (int i = 0; i < clips.Length; i++)  {
            audioClips.Add(clips[i].name.ToLower(), clips[i]);
            counter++;
        }
        //Debug.Log("Succesfully assigned: " + counter + " audio files.");
    }

    public static void LoadEntities() {
        Entity[] ents = Resources.LoadAll<Entity>("Prefabs");
        entities = new Dictionary<string, Entity>();
        int counter = 0;
        for (int i = 0; i < ents.Length; i++)  {
            if (ents[i].entityName == "changeme") {
                Debug.LogWarning("Entity "+ents[i].gameObject.name+" hs no entityname. It will not get added to the database.");
                continue;
            }
            entities.Add(ents[i].entityName.ToLower(), ents[i]);
            counter++;
        }
        //Debug.Log("Succesfully assigned: " + counter + " entities.");
    }

    public static void LoadCrosshairs() {
        Sprite[] crhs = Resources.LoadAll<Sprite>("Textures/UI/Crosshair");
        crosshairs = new Dictionary<string, Sprite>();
        int counter = 0;
        for (int i = 0; i < crhs.Length; i++) {
            crosshairs.Add(crhs[i].name.ToLower(), crhs[i]);
            counter++;
        }
        //Debug.Log("Succesfully assigned: " + counter + " entities.");
    }

    public static void LoadIcons() {
        Sprite[] iconList = Resources.LoadAll<Sprite>("Textures/UI/Icons");
        icons = new Dictionary<string, Sprite>();
        int counter = 0;
        for (int i = 0; i < iconList.Length; i++) {
            icons.Add(iconList[i].name.ToLower(), iconList[i]);
            counter++;
        }
        //Debug.Log("Succesfully assigned: " + counter + " entities.");
    }

    public static void LoadSoundGroups() {
        soundGroups = Resources.LoadAll<SoundGroup>("ScriptableObjects");
    }

    public static AudioClip GetAudioClip(string soundName) {
        if (audioClips == null) {
            LoadAudioClips();
        }
        AudioClip clip;
        audioClips.TryGetValue(soundName.ToLower(), out clip);
        return clip;
    }

    public static Entity GetEntity(string entityName) {
        if (entities == null) {
            LoadEntities();
        }
        entities.TryGetValue(entityName.ToLower(), out Entity entity);
        return entity;
    }

    public static Sprite GetCrosshair(string crosshairName) {
        if (crosshairs == null) {
            LoadCrosshairs();
        }
        crosshairs.TryGetValue(crosshairName.ToLower(), out Sprite sprite);
        return sprite;
    }

    public static Sprite GetIcon(string iconName) {
        if (icons == null) {
            LoadIcons();
        }
        icons.TryGetValue(iconName.ToLower(), out Sprite sprite);
        return sprite;
    }

    public static SoundGroup[] GetSoundGroups() {
        if (soundGroups == null) {
            LoadSoundGroups();
        }
        return soundGroups;
    }
}