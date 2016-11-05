using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Deffered<T>
{
    public IPromise<T> Promise { get; private set; }

    public Deffered()
    {
        Promise = new Promise<T>();
    }

    public void Resolve(T arg)
    {
        Promise.TriggerSuccess(arg);
    }

    public void Reject(Exception e)
    {
        Promise.TriggerFaild(e);
    }
}

