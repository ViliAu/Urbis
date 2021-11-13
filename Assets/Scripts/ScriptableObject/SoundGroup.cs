using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New sound group", menuName = "Sound Group")]
public class SoundGroup : ScriptableObject {
    public string groupName = "changeme";
    public AudioClip[] audioClips;
}
