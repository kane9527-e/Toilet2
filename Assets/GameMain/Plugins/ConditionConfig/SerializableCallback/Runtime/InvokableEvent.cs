using System;

public class InvokableEvent : InvokableEventBase
{
    public Action action;

    /// <summary> Constructor </summary>
    public InvokableEvent(object target, string methodName)
    {
        if (target == null || string.IsNullOrEmpty(methodName))
            action = () => { };
        else
            action = (Action)Delegate.CreateDelegate(typeof(Action), target, methodName);
    }

    public void Invoke()
    {
        action();
    }

    public override void Invoke(params object[] args)
    {
        action();
    }
}

public class InvokableEvent<T0> : InvokableEventBase
{
    public Action<T0> action;

    /// <summary> Constructor </summary>
    public InvokableEvent(object target, string methodName)
    {
        if (target == null || string.IsNullOrEmpty(methodName))
            action = x => { };
        else
            action = (Action<T0>)Delegate.CreateDelegate(typeof(Action<T0>), target, methodName);
    }

    public void Invoke(T0 arg0)
    {
        action(arg0);
    }

    public override void Invoke(params object[] args)
    {
        action((T0)args[0]);
    }
}

public class InvokableEvent<T0, T1> : InvokableEventBase
{
    public Action<T0, T1> action;

    /// <summary> Constructor </summary>
    public InvokableEvent(object target, string methodName)
    {
        if (target == null || string.IsNullOrEmpty(methodName))
            action = (x, y) => { };
        else
            action = (Action<T0, T1>)Delegate.CreateDelegate(typeof(Action<T0, T1>), target, methodName);
    }

    public void Invoke(T0 arg0, T1 arg1)
    {
        action(arg0, arg1);
    }

    public override void Invoke(params object[] args)
    {
        action((T0)args[0], (T1)args[1]);
    }
}

public class InvokableEvent<T0, T1, T2> : InvokableEventBase
{
    public Action<T0, T1, T2> action;

    /// <summary> Constructor </summary>
    public InvokableEvent(object target, string methodName)
    {
        if (target == null || string.IsNullOrEmpty(methodName))
            action = (x, y, z) => { };
        else
            action = (Action<T0, T1, T2>)Delegate.CreateDelegate(typeof(Action<T0, T1, T2>), target, methodName);
    }

    public void Invoke(T0 arg0, T1 arg1, T2 arg2)
    {
        action(arg0, arg1, arg2);
    }

    public override void Invoke(params object[] args)
    {
        action((T0)args[0], (T1)args[1], (T2)args[2]);
    }
}

public class InvokableEvent<T0, T1, T2, T3> : InvokableEventBase
{
    public Action<T0, T1, T2, T3> action;

    /// <summary> Constructor </summary>
    public InvokableEvent(object target, string methodName)
    {
        if (target == null || string.IsNullOrEmpty(methodName))
            action = (x, y, z, w) => { };
        else
            action = (Action<T0, T1, T2, T3>)Delegate.CreateDelegate(typeof(Action<T0, T1, T2, T3>), target,
                methodName);
    }

    public void Invoke(T0 arg0, T1 arg1, T2 arg2, T3 arg3)
    {
        action(arg0, arg1, arg2, arg3);
    }

    public override void Invoke(params object[] args)
    {
        action((T0)args[0], (T1)args[1], (T2)args[2], (T3)args[3]);
    }
}