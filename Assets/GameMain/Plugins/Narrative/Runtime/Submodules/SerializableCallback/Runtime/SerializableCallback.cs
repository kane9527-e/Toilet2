public abstract class SerializableCallback<TReturn> : SerializableCallbackBase<TReturn>
{
    public TReturn Invoke()
    {
        if (func == null) Cache();
        if (_dynamic)
        {
            var call = func as InvokableCallback<TReturn>;
            return call.Invoke();
        }

        return func.Invoke(Args);
    }

    protected override void Cache()
    {
        if (_target == null || string.IsNullOrEmpty(_methodName))
        {
            func = new InvokableCallback<TReturn>(null, null);
        }
        else
        {
            if (_dynamic)
                func = new InvokableCallback<TReturn>(target, methodName);
            else
                func = GetPersistentMethod();
        }
    }
}

public abstract class SerializableCallback<T0, TReturn> : SerializableCallbackBase<TReturn>
{
    public TReturn Invoke(T0 arg0)
    {
        if (func == null) Cache();
        if (_dynamic)
        {
            var call = func as InvokableCallback<T0, TReturn>;
            return call.Invoke(arg0);
        }

        return func.Invoke(Args);
    }

    protected override void Cache()
    {
        if (_target == null || string.IsNullOrEmpty(_methodName))
        {
            func = new InvokableCallback<T0, TReturn>(null, null);
        }
        else
        {
            if (_dynamic)
                func = new InvokableCallback<T0, TReturn>(target, methodName);
            else
                func = GetPersistentMethod();
        }
    }
}

public abstract class SerializableCallback<T0, T1, TReturn> : SerializableCallbackBase<TReturn>
{
    public TReturn Invoke(T0 arg0, T1 arg1)
    {
        if (func == null) Cache();
        if (_dynamic)
        {
            var call = func as InvokableCallback<T0, T1, TReturn>;
            return call.Invoke(arg0, arg1);
        }

        return func.Invoke(Args);
    }

    protected override void Cache()
    {
        if (_target == null || string.IsNullOrEmpty(_methodName))
        {
            func = new InvokableCallback<T0, T1, TReturn>(null, null);
        }
        else
        {
            if (_dynamic)
                func = new InvokableCallback<T0, T1, TReturn>(target, methodName);
            else
                func = GetPersistentMethod();
        }
    }
}

public abstract class SerializableCallback<T0, T1, T2, TReturn> : SerializableCallbackBase<TReturn>
{
    public TReturn Invoke(T0 arg0, T1 arg1, T2 arg2)
    {
        if (func == null) Cache();
        if (_dynamic)
        {
            var call = func as InvokableCallback<T0, T1, T2, TReturn>;
            return call.Invoke(arg0, arg1, arg2);
        }

        return func.Invoke(Args);
    }

    protected override void Cache()
    {
        if (_target == null || string.IsNullOrEmpty(_methodName))
        {
            func = new InvokableCallback<T0, T1, T2, TReturn>(null, null);
        }
        else
        {
            if (_dynamic)
                func = new InvokableCallback<T0, T1, T2, TReturn>(target, methodName);
            else
                func = GetPersistentMethod();
        }
    }
}

public abstract class SerializableCallback<T0, T1, T2, T3, TReturn> : SerializableCallbackBase<TReturn>
{
    public TReturn Invoke(T0 arg0, T1 arg1, T2 arg2, T3 arg3)
    {
        if (func == null) Cache();
        if (_dynamic)
        {
            var call = func as InvokableCallback<T0, T1, T2, T3, TReturn>;
            return call.Invoke(arg0, arg1, arg2, arg3);
        }

        return func.Invoke(Args);
    }

    protected override void Cache()
    {
        if (_target == null || string.IsNullOrEmpty(_methodName))
        {
            func = new InvokableCallback<T0, T1, T2, T3, TReturn>(null, null);
        }
        else
        {
            if (_dynamic)
                func = new InvokableCallback<T0, T1, T2, T3, TReturn>(target, methodName);
            else
                func = GetPersistentMethod();
        }
    }
}