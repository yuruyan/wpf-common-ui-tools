namespace CommonUITools.Model;

/// <summary>
/// 表示不抛出异常
/// </summary>
[AttributeUsage(
    AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Property,
    Inherited = false,
    AllowMultiple = true
)]
public sealed class NoExceptionAttribute : Attribute { }

/// <summary>
/// 表示只有一个实例
/// </summary>
[AttributeUsage(
    AttributeTargets.Class,
    Inherited = false,
    AllowMultiple = true
)]
public sealed class SingleInstanceAttribute : Attribute { }

/// <summary>
/// 表示线程安全
/// </summary>
[AttributeUsage(
    AttributeTargets.Property
    | AttributeTargets.Class
    | AttributeTargets.Struct
    | AttributeTargets.Method
    | AttributeTargets.Field
    | AttributeTargets.Delegate,
    Inherited = false,
    AllowMultiple = true
)]
public sealed class ThreadSafeAttribute : Attribute { }
