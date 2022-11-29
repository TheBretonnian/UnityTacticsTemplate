using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AnimationHandler : MonoBehaviour
{
    Animator animator;

    struct ActionPair
    {
        public Action Action;
        public string AnimationClip;
    }ActionPair configuredAction;


    public delegate void NotifyEndOfAnimation(string animationClip);
    public event NotifyEndOfAnimation OnAnimationFinished;


    void Awake()
    {
        TryGetComponent<Animator>(out animator);
    }

    //Use this for animations which loop or if you dont care about end 
    public void PlayAnimation(string animationClip)
    {
        animator?.Play(animationClip);
    }

    public void PlayAnimation(string animationClip, Action ActionOnAnimationFinished)
    {
        configuredAction.AnimationClip = animationClip;
        configuredAction.Action = ActionOnAnimationFinished;
        StartCoroutine(PlayAnimationAndWaitAnimationCompleted(animationClip));
    }

    //Call this as Coroutine to make a Sync. Call
    public IEnumerator PlayAnimationAndWaitAnimationCompleted(string animationClip)
    {
        PlayAnimation(animationClip);
        while(animator?.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f || animator?.GetCurrentAnimatorStateInfo(0).IsName(animationClip)==false)
        {
            yield return new WaitForEndOfFrame();
        }

        if(animationClip == configuredAction.AnimationClip)
        {
            configuredAction.Action?.Invoke();
        }
        OnAnimationFinished?.Invoke(animationClip);
    }


}
