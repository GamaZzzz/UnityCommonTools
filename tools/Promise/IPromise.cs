using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public interface IPromise<T>
{
    void OnSuccess(Action<T> callback);
    void OnFaild(Action<Exception> callback);
    void TriggerSuccess(T arg);
    void TriggerFaild(Exception e);
}
