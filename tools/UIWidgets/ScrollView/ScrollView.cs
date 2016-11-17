using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScrollView : MonoBehaviour
{
    public  GridLayoutGroup grid;
    [SerializeField]
    private ScrollRect rect;

    private RectTransform _gridRect;
    private int _childCount = 0;
    public List<GridItem> Children { get; private set; }

    void Start()
    {
        Children = new List<GridItem>();
        _gridRect = grid.gameObject.GetComponent<RectTransform>();
    }

    public void AddCell(GridItem cell)
    {
        cell.RectRoot.SetParent(_gridRect);
        cell.RectRoot.SetSiblingIndex(_childCount);
        cell.Index = _childCount;
        ++_childCount;
    }

    public void Remove(int index)
    {
        try
        {
            Children.RemoveAt(index);
            --_childCount;
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public void Remove(GridItem child)
    {
        if(Children.Remove(child))
        {
            --_childCount;
        }
    }

    public void RemoveAll()
    {
        Children.Clear();
        _childCount = 0;
    }

    public void Sort()
    {
        Children.Sort((a, b) =>
        {
            return a.Index.CompareTo(b.Index);
        });

        for (int i = 0; i < _childCount; ++i)
        {
            Children[i].SetOrder(i);
            Children[i].Index = i;
        }
    }

    public void Sort(IComparer<GridItem> sort)
    {
        Children.Sort(sort);
        for (int i = 0; i < _childCount; ++i)
        {
            Children[i].SetOrder(i);
            Children[i].Index = i;
        }
    }
}

