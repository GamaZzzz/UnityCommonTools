using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class Promise<T> : IPromise<T>
{
    Action<Exception> _onFaild;
    Action<T> _onSuccess;
    public void OnFaild(Action<Exception> callback)
    {
        _onFaild = callback;
    }

    public void OnSuccess(Action<T> callback)
    {
        _onSuccess = callback;
    }

    public void TriggerFaild(Exception e)
    {
        _onFaild.Dispatch(e);
    }

    public void TriggerSuccess(T arg)
    {
        _onSuccess.Dispatch(arg);
    }
}
