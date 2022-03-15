using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPlayer : MonoBehaviour {
    public List<AnimationState> animList;
    public Dictionary<string, AnimationState> animDictionary;
    public AnimationState anim;
    public bool playOnStart = true;
    public float startOffset;
    void Start() {
        if (anim != null) {
            anim.Init(transform,startOffset);
            if(playOnStart)
                anim.Play();
        }
        
        animDictionary = new Dictionary<string, AnimationState>();
        foreach (var animation in animList) {
            animDictionary.Add(animation.name,animation);
        }
    }

    public void PlayAnim() {
        if(!anim.animLock)
            anim.Play();   
    }

    public void PlayAnim(string animName) {
        anim = animDictionary[animName];
        anim.Init(transform,startOffset);
        anim.Play();  
    } 
    
    public void Pause() => anim.Pause();
    public void Resume() => anim.Resume();
}