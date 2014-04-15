/// <summary> 
/// 包含一些小的辅助类
/// 如果有必要，也可把辅助类放在相同目录下的新脚本里，例如Math.cs, TextUtility.cs...
/// </summary>

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections;
using System.Collections.Generic;



/// <summary>
/// Unity相关扩展，包含全局方法和扩展方法
/// </summary>

public static partial class Unity {

    /// <summary> 是否是pro版的Unity </summary>
#if UNITY_EDITOR
    public static bool isPro { get { return Application.HasProLicense(); } }
#else
    public const bool isPro = true;
#endif

    /// <summary> 加载场景，如果可以异步加载就用异步 </summary>
    /// <param name="levelName"> 场景名 </param>
    /// <param name="additive"> 是否保留原场景 </param>
    /// <returns> 异步加载时的coroutine </returns>
    public static AsyncOperation LoadLevel (string levelName, bool additive = false) {
        if (isPro) {
            if (additive)
                return Application.LoadLevelAdditiveAsync(levelName);
            else
                return Application.LoadLevelAsync(levelName);
        }
        else {
            if (additive)
                Application.LoadLevelAdditive(levelName);
            else
                Application.LoadLevel(levelName);
            return null;
        }
    }

    /// <summary>
    /// 替换mecanim角色的一个动作
    /// </summary>
    /// <param name="animator">调用者</param>
    /// <param name="clipName">动作名称</param>
    /// <param name="overrideClip">新动作</param>
    public static void ReplaceClip(this Animator animator, String clipName, AnimationClip overrideClip) {
        AnimatorOverrideController overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
        if (overrideController == null) {
            overrideController = new AnimatorOverrideController();
            overrideController.runtimeAnimatorController = animator.runtimeAnimatorController;
        }
        overrideController[clipName] = overrideClip;
        if (ReferenceEquals(animator.runtimeAnimatorController, overrideController) == false) {
            animator.runtimeAnimatorController = overrideController;
        }
    }

    /// <summary>
    /// 替换mecanim角色的一个动作
    /// </summary>
    /// <param name="animator">调用者</param>
    /// <param name="originalClip">旧动作</param>
    /// <param name="overrideClip">新动作</param>
    public static void ReplaceClip(this Animator animator, AnimationClip originalClip, AnimationClip overrideClip) {
        AnimatorOverrideController overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
        if (overrideController == null) {
            overrideController = new AnimatorOverrideController();
            overrideController.runtimeAnimatorController = animator.runtimeAnimatorController;
        }
        overrideController[originalClip] = overrideClip;
        if (ReferenceEquals(animator.runtimeAnimatorController, overrideController) == false) {
            animator.runtimeAnimatorController = overrideController;
        }
    }
    
    /// <summary> 获得Animation组件正在播放的动画 </summary>
    public static AnimationState GetPlayingAnimation(this Animation ani) {
        if (ani.isPlaying) {
            foreach (AnimationState state in ani) {
                if (ani.IsPlaying(state.name)) {
                    return state;
                }
            }
        }
        return null;
    }

    /// <summary> 获得Component，如果不存在可以新建 </summary>
    public static T GetComponent<T>(this GameObject go, bool create) where T : Component {
        T comp = go.GetComponent(typeof(T)) as T;
        if (create && comp == null) {
            comp = go.AddComponent(typeof(T)) as T;
        }
        return comp;
    }
    /// <summary> 获得Component，如果不存在可以新建 </summary>
    public static T GetComponent<T> (this Component component, bool create) where T : Component {
        T comp = component.GetComponent(typeof(T)) as T;
        if (create && comp == null) {
            comp = component.gameObject.AddComponent(typeof(T)) as T;
        }
        return comp;
    }

    /// <summary>
    /// 简单的辅助方法，返回一个新的Vector，无副作用，用来减少代码行数
    /// </summary>
    /// <example>
    /// 例如
    /// Vector3 vec = transform.localPosition;
    /// vec.y = height;
    /// transform.localPosition = vec;
    /// 可以写成
    /// transform.localPosition = transform.localPosition.SetY(height);
    /// 
    /// 例如
    /// var dis = mob.transform.position - pos;
    /// dis.y = 0;
    /// float sqrDis = dis.sqrMagnitude;
    /// 可以写成
    /// float sqrDis = (mob.transform.position - pos).SetY(0).sqrMagnitude;
    /// </example>
    public static Vector3 SetX (this Vector3 vector, float x) {
        vector.x = x;
        return vector;
    }
    public static Vector3 SetY (this Vector3 vector, float y) {
        vector.y = y;
        return vector;
    }
    public static Vector3 SetZ (this Vector3 vector, float z) {
        vector.z = z;
        return vector;
    }
    public static Vector2 SetX (this Vector2 vector, float x) {
        vector.x = x;
        return vector;
    }
    public static Color SetAlpha (this Color color, float alpha) {
        color.a = alpha;
        return color;
    }

}
