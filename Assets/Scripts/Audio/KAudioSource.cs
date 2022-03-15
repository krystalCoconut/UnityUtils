// KAudioSource

using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioClip))]
public class KAudioSource : MonoBehaviour {
    
    public VariedAudioClip vClip;
    private AudioSource aus;
    public void Awake() {aus = GetComponent<AudioSource>();}

    public void PlayRandOneShot() {
        // Get a random clip
        int index = Random.Range(0,vClip.audioclips.Length-1);
        float varience = Random.Range(-vClip.pitchVarience / 2f, vClip.pitchVarience / 2f);
        aus.pitch = vClip.pitch + varience; 
        aus.PlayOneShot(vClip.audioclips[index]);
    }
}