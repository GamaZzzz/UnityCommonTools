using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GameObjectPool<T>  where T : MonoBehaviour
{
    private Queue<T> _stored = new Queue<T>();
    private List<T> _actived = new List<T>();

    private ObjectBuilder _builder;
    private GameObject _original;

    private int _initialsize;
    private int _maxsize;
    private int _cursize;
    private DateTime _lastcleantime = DateTime.Now;
    private const double CLEANDURATION = 5 * 60;//五分钟

    public GameObjectPool(int initialsize, int maxsize, GameObject original, ObjectBuilder builder)
    {
        _original = original;
        _builder = builder;
        _initialsize = initialsize;
        _maxsize = maxsize;
        _cursize = 0;
        NewObjects();
    }

    public T Claims()
    {
        T t = default(T);

        while(_stored.Count <= 0)
        {
            NewObjects();
        }

        t = _stored.Dequeue();

        _actived.Add(t);

        t.gameObject.SetActive(true);

        return t;
    }
    
    public void Reclaims(T t)
    {
        t.gameObject.SetActive(false);
        _actived.Remove(t);
        _stored.Enqueue(t);
        DoClean();
    }

    void NewObjects()
    {
        if(_cursize < _maxsize)
        {
            for (int i = 0; i < _initialsize; ++i)
            {
                T t = _builder.Build<T>(_original);
                t.gameObject.SetActive(false);
                _stored.Enqueue(t);
            }
            _cursize += _initialsize;
        }
    }

    void DoClean()
    {
        double duration = (DateTime.Now - _lastcleantime).TotalSeconds;
        if(_stored.Count > _initialsize && duration >= CLEANDURATION)
        {
            int delta = _stored.Count - _initialsize;
            for(int i=0; i < delta; ++i)
            {
                _builder.Destroy(_stored.Dequeue());
            }
        }
    }
}

