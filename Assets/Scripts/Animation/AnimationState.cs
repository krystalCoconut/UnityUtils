using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "AnimationState", menuName = "Week1/AnimationState", order = 1)]
public class AnimationState : ScriptableObject {
    // Movement Events are made of unityactions containing dotweens
    private Sequence Animation;
    public float endLoopDuration;
    private float startOffset;
    
    public bool animLock;
    public void Lock() => animLock = true;
    public void Unlock() => animLock = false;
    
    
    [Serializable]
    public class TimedEvent {
        public UnityEvent Event;
        public float delay;
    }

    public List<TimedEvent> events;
    
    public bool returnToStartPosition;
    public bool loop;
    [FormerlySerializedAs("timestoLoop")]
    public int timesToLoop;
    [TextArea(20,20)]
    public string AnimationScript;
    private Transform t;
    
    public void Init(Transform transform) {
        t = transform;
        Animation = DOTween.Sequence();
        LoadAnimation();
    }

    public void Pause() {
        Animation.timeScale = 0;
    }

    public void Resume() {
        Animation.timeScale = 1;
    }

    public void Init(Transform transform,float offset) {
        t = transform;
        Animation = DOTween.Sequence();
        startOffset = offset;
        LoadAnimation();
    }


    public void LoadAnimation() {
        
        Vector3 startPosition = t.localPosition;
        Vector3 startRotation = t.eulerAngles;
        Vector3 startLocalScale = t.localScale;
        
        // Split the lines
        string[] lines = AnimationScript.Split('\n');

        int lineNumber = 0;
        // Parse the operator of the command
        foreach (var line in lines) {
            // Split the line into params
            string[] parameters = line.Split(' ');

            if (line == "\n")
                break;
            
            if (parameters.Length != 4) {
                Debug.LogError($"Command: {line}:Line {lineNumber} should contain 4 params, contains {parameters.Length}");
                break;
            }
            string command = parameters[0];
            // Get the command
            switch (parameters[0]) {
                case "append":
                case "APPEND":
                    AnimationAppend($"{parameters[1]} {parameters[2]} {parameters[3]}");
                    break;
                case "join":
                case "JOIN":
                    AnimationJoin($"{parameters[1]} {parameters[2]} {parameters[3]}");
                    break;
                default:
                    Debug.LogError($"{command} is not a valid command");
                    return;
            }

            lineNumber++;
        }

        Animation.PrependInterval(startOffset).Pause();
        
        // Hack:
        // Put the animation back to where it was at the start
        if (returnToStartPosition) {
            Animation.Append(t.DOLocalMove(startPosition,endLoopDuration)).Pause();
            Animation.Join(t.DOLocalRotate(startRotation, endLoopDuration)).Pause();
            Animation.Join(t.DOScale(startLocalScale, endLoopDuration)).Pause();
        }
    }

    public void Play() {
        startOffset = 0;
        if (loop) {
            if(timesToLoop == 0)
                Animation.Play().SetLoops(-1).OnComplete(() => {
                    Animation.PrependInterval(0);
                });
            else
                Animation.Play().SetLoops(timesToLoop).OnComplete( () => Animation.PrependInterval(0));
        }
        else {
            Animation.Play().OnStart(Lock).OnComplete(Unlock);
        }
            
    }
    
    public void AnimationAppend(string tweenString) => Animation.Append( ConstructTweenFromString(tweenString,t) );
    public void AnimationJoin( string tweenString) => Animation.Join( ConstructTweenFromString(tweenString,t) );
    
    // Animation Format: 
    // MOVE 0,10,00 .2
    // ROTATE 0,90,180 20
    private Tween ConstructTweenFromString(string tweenString, Transform target) {
        string[] parameters = tweenString.Split(' ');
        Tween returnTween;
        // Check if the string contains 3 parameters
        if (parameters.Length != 3) {
            Debug.LogError($"Tween Command: {tweenString} should contain 3 params, contains {parameters.Length}");
            return null;
        }

        string[] coordStrings = parameters[1].Split(',');
        // Check if the vector has 3 parameters
        if (coordStrings.Length != 3) {
            Debug.LogError($"The supplied vector should have 3 coords, contains {coordStrings.Length}");
            return null;
        }
        
        // Check if all the coords are floats
        Vector3 vector;
        float duration;
        if (!float.TryParse(coordStrings[0], out vector.x)) {
            Debug.LogError($"X Coordinate is not a float");
            return null;
        }

        if (!float.TryParse(coordStrings[1], out vector.y)) {
            Debug.LogError($"Y Coordinate is not a float");
            return null;
        }

        if (!float.TryParse(coordStrings[2], out vector.z)) {
            Debug.LogError($"Z Coordinate is not a float");
            return null;
        }
        
        // Check if the duration is a float
        if (!float.TryParse(parameters[2], out duration)) {
            Debug.LogError($"duration is not a float");
            return null;
        }

        string command = parameters[0];
        
        // Make the tween :)
        switch (command) {
            case "move":
            case "MOVE":
                returnTween = target.DOLocalMove(t.localPosition + vector,duration).Pause();
                break;
            case "rotate":
            case "ROTATE":
                returnTween = target.DOLocalRotate(t.eulerAngles + vector,duration).Pause();
                break;
            case "scale":
            case "SCALE":
                returnTween = target.DOScale(Vector3.Scale(t.localScale , vector),duration).Pause();
                break;
            default:
                Debug.LogError($"{command} is not a valid command");
                return null;
        }

        return returnTween;
    }

}