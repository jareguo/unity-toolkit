using UnityEngine;

/// <summary>
/// Layer管理类，用于缓存项目里用到的所有layer，包括layer mask的集合
/// 这里面声明的layer都是和unity的layer一一对应的，但是这里的mask不一定是只有一个layer，可能是多个layer mask做或运算
/// </summary>
public static class Layer {

    /// <summary> ground layer index </summary>
    public static readonly int Ground = LayerMask.NameToLayer("Ground"); // 简单的layer直接这里初始化
    /// <summary> ground layer mask </summary>
    public static readonly int GroundMask = 1 << Ground;

    /// <summary> mouse over collider layer index </summary>
    public static readonly int MouseOverCollider = LayerMask.NameToLayer("MouseOverCollider");
    /// <summary> mouse over collider layer mask </summary>
    public static readonly int MouseOverColliderMask = 1 << MouseOverCollider;

    /// <summary> interactive layer mask </summary>
    public static readonly int InteractiveLayerMask = MouseOverColliderMask | GroundMask;

    /// <summary> 初始化很复杂的mask </summary>
    static Layer () {

    }
}
