using System;
using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;

public class SpineAnimHandler : MonoBehaviour
{
    protected SkeletonAnimation _skeletonAnimation;

    [SerializeField] public List<AnimationState> _animationState = new List<AnimationState>();
    [SerializeField] private List<SkinState> _skinState = new List<SkinState>();

    [Serializable]
    public class AnimationState
    {
        [SpineAnimation] public string stateName;
        public Spine.Animation animation;
    }

    [Serializable]
    public class SkinState
    {
        [SpineSkin] public string stateName;
        public Spine.Skin skin;
    }

    private Spine.Animation _loopAnimation;

    private bool _blockIntercept;

    protected virtual void Awake()
    {
        _skeletonAnimation = GetComponent<SkeletonAnimation>();

        foreach (var entry in _animationState)
        {
            SkeletonData skeletonData = _skeletonAnimation.skeletonDataAsset.GetSkeletonData(true);
            entry.animation = skeletonData != null ? skeletonData.FindAnimation(entry.stateName) : null;
        }

        foreach (var entry in _skinState)
        {
            SkeletonData skeletonData = _skeletonAnimation.skeletonDataAsset.GetSkeletonData(true);
            entry.skin = skeletonData != null ? skeletonData.FindSkin(entry.stateName) : null;
        }
    }

    public void ChangeSkin(string skinName)
    {
        var targetSkin = GetSkin(skinName);
        if (targetSkin == null)
            return;
        
        _skeletonAnimation.Skeleton.SetSkin(targetSkin);
        _skeletonAnimation.Skeleton.SetSlotsToSetupPose();
    }

    public void ChangeSkin(int skinIndex)
    {
        var targetSkin = _skinState[skinIndex];
        if (targetSkin == null)
            return;

        _skeletonAnimation.Skeleton.SetSkin(targetSkin.stateName);
        _skeletonAnimation.Skeleton.SetSlotsToSetupPose();
    }

    public void DamagedAnim()
    {
        StartCoroutine(coDamagedAnim());
    }

    private IEnumerator coDamagedAnim()
    {
        _skeletonAnimation.skeleton.SetColor(new Color32(220, 58, 58, 225));
        yield return new WaitForSeconds(0.1f);
        _skeletonAnimation.skeleton.SetColor(Color.white);
        yield return new WaitForSeconds(0.1f);
        _skeletonAnimation.skeleton.SetColor(new Color32(220, 58, 58, 225));
        yield return new WaitForSeconds(0.1f);
        _skeletonAnimation.skeleton.SetColor(Color.white);
    }

    public void PlayLoopAnim(string animName, int index = 0)
    {
        var targetAnim = GetAnimation(animName);
        if (targetAnim == null)
            return;
        
        PlayLoopAnim(targetAnim, index);
    }

    public void PlayLoopAnim(int animIndex, int index = 0)
    {
        var targetAnim = _animationState[animIndex].animation;
        if (targetAnim == null)
            return;
        PlayLoopAnim(targetAnim, index);
    }

    public void PlayLoopAnim(Spine.Animation animation, int index = 0)
    {
        if (_blockIntercept)
            return;
        _loopAnimation = animation;
        _skeletonAnimation.AnimationState.SetAnimation(0, animation, true);
    }

    public void PlayOneShotAnim(string animName, bool returnLoop = true, bool playFullAnim = false, bool forcePlay = false, int index = 0)
    {
        var targetAnim = GetAnimation(animName);
        if (targetAnim == null)
            return;

        PlayOneShotAnim(targetAnim, returnLoop, playFullAnim, forcePlay, index);
    }

    public void PlayOneShotAnim(int animIndex, bool returnLoop = true, bool playFullAnim = false, bool forcePlay = false, int index = 0)
    {
        var targetAnim = _animationState[animIndex].animation;
        if (targetAnim == null)
            return;
        PlayOneShotAnim(targetAnim, returnLoop, playFullAnim, forcePlay, index);
    }

    public void PlayOneShotAnim(Spine.Animation animation, bool returnLoop = true, bool playFullAnim = false, bool forcePlay = false, int index = 0)
    {
        if (_blockIntercept && !forcePlay)
            return;
        if (playFullAnim)
        {
            _blockIntercept = true;
            StartCoroutine(coWaitUntilFullAnim(animation.Duration));
        }
        _skeletonAnimation.AnimationState.SetAnimation(0, animation, false);
        if (returnLoop)
            _skeletonAnimation.AnimationState.AddAnimation(0, _loopAnimation, true, animation.Duration);
    }

    private IEnumerator coWaitUntilFullAnim(float duration)
    {
        yield return new WaitForSecondsRealtime(duration);
        _blockIntercept = false;
    }

    public Spine.Animation GetAnimation(string stateShortName)
    {
        return GetAnimation(StringToHash(stateShortName));
    }

    public Spine.Animation GetAnimation(int stateShortName)
    {
        var foundState = _animationState.Find(entry => StringToHash(entry.stateName) == stateShortName);
        return (foundState == null) ? null : foundState.animation;
    }

    public Spine.Skin GetSkin(string stateShortName)
    {
        return GetSkin(StringToHash(stateShortName));
    }

    public Spine.Skin GetSkin(int stateShortName)
    {
        var foundState = _skinState.Find(entry => StringToHash(entry.stateName) == stateShortName);
        return (foundState == null) ? null : foundState.skin;
    }

    private int StringToHash(string str)
    {
        return Animator.StringToHash(str);
    }
}