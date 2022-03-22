using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio", menuName = "KObjects/KAudioClip", order = 1)]
public class VariedAudioClip : ScriptableObject {
    public AudioClip[] audioclips;
    public float pitchVarience;
    public float pitch;
}