using System;
using Object = UnityEngine.Object;

public class InvokableCallback<TReturn> : InvokableCallbackBase<TReturn>
{
    public Func<TReturn> func;

    /// <summary> Constructor </summary>
    public InvokableCallback(object target, string methodName)
    {
        if (target == null || string.IsNullOrEmpty(methodName))
            func = () => default;
        else
            func = (Func<TReturn>)Delegate.CreateDelegate(typeof(Func<TReturn>), target, methodName);
    }

    public TReturn Invoke()
    {
        return func();
    }

    public override TReturn Invoke(params object[] args)
    {
        return func();
    }
}

public class InvokableCallback<T0, TReturn> : InvokableCallbackBase<TReturn>
{
    public Func<T0, TReturn> func;

    /// <summary> Constructor </summary>
    public InvokableCallback(object target, string methodName)
    {
        if (target == null || string.IsNullOrEmpty(methodName))
            func = x => default;
        else
            func = (Func<T0, TReturn>)Delegate.CreateDelegate(typeof(Func<T0, TReturn>), target, methodName);
    }

    public TReturn Invoke(T0 arg0)
    {
        return func(arg0);
    }

    public override TReturn Invoke(params object[] args)
    {
        // Convert from special "unity-nulls" to true null
        if (args[0] is Object && (Object)args[0] == null) args[0] = null;
        return func((T0)args[0]);
    }
}

public class InvokableCallback<T0, T1, TReturn> : InvokableCallbackBase<TReturn>
{
    public Func<T0, T1, TReturn> func;

    /// <summary> Constructor </summary>
    public InvokableCallback(object target, string methodName)
    {
        if (target == null || string.IsNullOrEmpty(methodName))
            func = (x, y) => default;
        else
            func = (Func<T0, T1, TReturn>)Delegate.CreateDelegate(typeof(Func<T0, T1, TReturn>), target, methodName);
    }

    public TReturn Invoke(T0 arg0, T1 arg1)
    {
        return func(arg0, arg1);
    }

    public override TReturn Invoke(params object[] args)
    {
        // Convert from special "unity-nulls" to true null
        if (args[0] is Object && (Object)args[0] == null) args[0] = null;
        if (args[1] is Object && (Object)args[1] == null) args[1] = null;
        return func((T0)args[0], (T1)args[1]);
    }
}

public class InvokableCallback<T0, T1, T2, TReturn> : InvokableCallbackBase<TReturn>
{
    public Func<T0, T1, T2, TReturn> func;

    /// <summary> Constructor </summary>
    public InvokableCallback(object target, string methodName)
    {
        if (target == null || string.IsNullOrEmpty(methodName))
            func = (x, y, z) => default;
        else
            func = (Func<T0, T1, T2, TReturn>)Delegate.CreateDelegate(typeof(Func<T0, T1, T2, TReturn>), target,
                methodName);
    }

    public TReturn Invoke(T0 arg0, T1 arg1, T2 arg2)
    {
        return func(arg0, arg1, arg2);
    }

    public override TReturn Invoke(params object[] args)
    {
        // Convert from special "unity-nulls" to true null
        if (args[0] is Object && (Object)args[0] == null) args[0] = null;
        if (args[1] is Object && (Object)args[1] == null) args[1] = null;
        if (args[2] is Object && (Object)args[2] == null) args[2] = null;
        return func((T0)args[0], (T1)args[1], (T2)args[2]);
    }
}

public class InvokableCallback<T0, T1, T2, T3, TReturn> : InvokableCallbackBase<TReturn>
{
    public Func<T0, T1, T2, T3, TReturn> func;

    /// <summary> Constructor </summary>
    public InvokableCallback(object target, string methodName)
    {
        if (target == null || string.IsNullOrEmpty(methodName))
            func = (x, y, z, w) => default;
        else
            func = (Func<T0, T1, T2, T3, TReturn>)Delegate.CreateDelegate(typeof(Func<T0, T1, T2, T3, TReturn>), target,
                methodName);
    }

    public TReturn Invoke(T0 arg0, T1 arg1, T2 arg2, T3 arg3)
    {
        return func(arg0, arg1, arg2, arg3);
    }

    public override TReturn Invoke(params object[] args)
    {
        // Convert from special "unity-nulls" to true null
        if (args[0] is Object && (Object)args[0] == null) args[0] = null;
        if (args[1] is Object && (Object)args[1] == null) args[1] = null;
        if (args[2] is Object && (Object)args[2] == null) args[2] = null;
        if (args[3] is Object && (Object)args[3] == null) args[3] = null;
        return func((T0)args[0], (T1)args[1], (T2)args[2], (T3)args[3]);
    }
}