﻿using UnityEngine;
using fsm;

/// <summary>
/// 动画状态机的跳转，实例由AniState.to方法进行创建。
/// 主要用于判断什么时候跳转到目标状态，判断方法是所有 设置过的 条件进行与(and)运算，为true才会进行跳转。
/// 唯一的例外是trigger变量，被设为true后会强制允许跳转，但状态机可能不会立刻执行到。
/// 如果要多个条件之间进行或(or)运算，可以创建多个AniTransition实例。
/// </summary>
[System.Serializable]
public class AniTransition : TimerTransition {

    /// <summary> 触发动画跳转，不论when、after条件是否满足。 </summary>
    public bool trigger = false;

    System.Nullable<float> exitTime;
    System.Func<bool> onDoCheck;

    ///////////////////////////////////////////////////////////////////////////////
    // Functions
    ///////////////////////////////////////////////////////////////////////////////
    
    public AniTransition ()
        : base () {
        onCheck = OnCheck;
    }

    /// <summary> 当条件达成就可以触发跳转，和after是与(and)的布尔关系。 </summary>
    /// <param name="_onCheck">
    /// 条件函数，返回是否可以跳转。
    /// 这个方法不能有任何副作用，如果需要修改变量，应该在state的回调里修改！
    /// </param>
    public AniTransition when (System.Func<bool> _onCheck = null) {
        onDoCheck = _onCheck;
        return this;
    }

    /// <summary> 限制只有动画播放到了一定的时候才能跳转，和when是与(and)的布尔关系。 </summary>
    /// <param name="_normalizedTime">
    /// 和Mecanim里的Exit Time一样，用于限制退出这个状态所需要的时间。
    /// 如果是0.5则代表必须等到动画播放到一半后才能切换到其它状态。
    /// 使用normalizedTime进行判断，例如是2.5则代表要等动画loop 2.5次后才能切换到其它状态。
    /// </param>
    public AniTransition after (float _normalizedTime) {
        exitTime = _normalizedTime;
        return this;
    }

    bool OnCheck () {
        if (trigger) {
            trigger = false;
            return true;
        }

        float normalizedTime = ((AniState)source).normalizedTime;
        bool played = normalizedTime == 0.0f;
        if (played) {
            normalizedTime = 1.0f;
#if UNITY_EDITOR
            if (exitTime.HasValue != false && exitTime.Value > 1.0f) {
                var s = (AniState)source;
                var state = s.anim[s.curAniName];
                if (state != null && (state.wrapMode != WrapMode.Loop || state.wrapMode != WrapMode.PingPong || state.wrapMode != WrapMode.ClampForever)) {
                    Debug.LogWarning(string.Format("非循环动画的normalizedTime永远不可能大于1 ({0} to {1})", source.name, target.name));
	            }
            }
#endif
        }

        if (exitTime.HasValue && normalizedTime < exitTime.Value) {
            return false;
        }
        if (onDoCheck != null) {
            return onDoCheck();
        }
        return exitTime.HasValue && normalizedTime >= exitTime.Value;
    }
}

/// <summary>
/// 动画状态机的普通状态
/// </summary>
[System.Serializable]
public class AniState : State {

    public Animation anim = null;
    public string curAniName = null;

    ///////////////////////////////////////////////////////////////////////////////
    // Properties
    ///////////////////////////////////////////////////////////////////////////////

    public float normalizedTime {
        get {
            //AnimationState aniState = anim.GetPlayingAnimation();
            AnimationState aniState = anim[curAniName];
            //Debug.Log(string.Format("aniState: {0} " + aniState.normalizedTime, aniState.name));
            return aniState != null ? aniState.normalizedTime : 0.0f;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // Functions
    ///////////////////////////////////////////////////////////////////////////////
    
    /// <param name="_name"> 状态名，用于调试，同时作为当前状态所播放的动作名(也就是AnimationClip.name) </param>
    public AniState (string _name, Animation _anim, State _parent = null) 
        : base(_name, _parent) {
        anim = _anim;
        mgDebug.Assert(anim != null);
        onFadeIn += Play;
        //onEnter += delegate ( State _from, State _to ) {
        //};
        //onExit += delegate ( State _from, State _to ) {
        //};
    }

    /// <summary> 声明状态的跳转 </summary>
    /// <param name="_targetState"> 目标状态 </param>
    /// <param name="_duration"> 动画过渡所需时间 </param>
    public AniTransition to (AniState _targetState, float _duration) {
        AniTransition transition = new AniTransition() {
            source = this,
            target = _targetState,
            duration = _duration,
        };
        transitionList.Add (transition);
        return transition;
    }

    /// <summary> 播放本状态的动作 </summary>
    public virtual void Play (Transition transition) {
        if (transition == null) {
            anim.Play(name);
            curAniName = name;
            return;
        }
        if (string.IsNullOrEmpty(name)) {
            anim.Stop();
            curAniName = null;
        }
        else {
            anim.CrossFade(name, ((TimerTransition)transition).duration);
            curAniName = name;
        }
    }
}

/// <summary>
/// 随机动作状态，用于播放一系列随机动作
/// </summary>
[System.Serializable]
public class RandAniState : AniState {
    public string[] animList = null;

    /// <param name="_animList"> 以";"分割的动画名称列表 </param>
    /// <param name="_name"> 状态名，用于调试 </param>
    public RandAniState (string _name, Animation _anim, string _animList, State _parent = null)
        : base(_name, _anim, _parent) {
        animList = _animList.Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries);
    }
    
    public override void Play (Transition transition) {
        var aniName = animList[Random.Range(0, animList.Length)];
        if (string.IsNullOrEmpty(aniName)) {
            anim.Stop();
            curAniName = null;
        }
        else {
            anim.CrossFade(aniName, ((TimerTransition)transition).duration);
            curAniName = aniName;
        }
    }
}