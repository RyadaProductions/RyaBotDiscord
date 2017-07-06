using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RyaBot.Models
{
  public class HashList<T> : CollectionBase, IEnumerable
  {
    private List<T> _internalList = new List<T>();
    private HashSet<int> _internalHashSet = new HashSet<int>();

    public HashList()
    {
    }

    public bool Add(T obj)
    {
      if (_internalHashSet.Add(obj.GetHashCode()))
      {
        _internalList.Add(obj);
        return true;
      }
      return false;
    }

    public void Remove(T obj)
    {
      _internalHashSet.Remove(obj.GetHashCode());
      _internalList.Remove(obj);
    }

    public T First()
    {
      return _internalList.First();
    }

    public T Last()
    {
      return _internalList.Last();
    }

    public new void Clear()
    {
      _internalList.Clear();
      _internalHashSet.Clear();
    }

    public new int Count()
    {
      return _internalHashSet.Count();
    }

    public new IEnumerator GetEnumerator()
    {
      return _internalList.GetEnumerator();
    }
  }
}
