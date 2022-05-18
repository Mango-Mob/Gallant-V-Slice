using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProbabilityList<T> : IList<T>
{
    protected List<(T, int)> m_elements = new List<(T, int)>();
    
    private int m_totalWeight = 0;

    public T this[int index] { get => m_elements[index].Item1; set => m_elements[index] = (value, m_elements[index].Item2); }

    public int Count => m_elements.Count;
    public int TotalWeight => m_totalWeight;

    public bool IsReadOnly => throw new NotImplementedException();

    public void Add(T item)
    {
        //Add to the list with an equal chance weight
        int newWeight = Mathf.RoundToInt(Math.Max(m_totalWeight, 1) / Math.Min(Count, 1));
        m_totalWeight += newWeight;

        m_elements.Add((item, newWeight));
    }

    public void Add(T item, int weight)
    {
        m_totalWeight += weight;

        m_elements.Add((item, weight));
    }

    public void Clear()
    {
        m_elements.Clear();
        m_totalWeight = 0;
    }

    public bool Contains(T item)
    {
        for (int i = 0; i < Count; i++)
        {
            if(this[i].Equals(item))
            {
                return true;
            }
        }
        return false;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        for (int i = 0; i < arrayIndex; i++)
        {
            array[i] = this[i];
        }
    }

    public T GetRandomly()
    {
        int weightSelect = UnityEngine.Random.Range(1, m_totalWeight + 1); //1 to t - 1
        
        for (int i = 0; i < m_elements.Count; i++)
        {
            weightSelect -= m_elements[i].Item2;
            if(weightSelect <= 0)
            {
                return m_elements[i].Item1;
            }
        }
        return default(T);
    }

    public IEnumerator<T> GetEnumerator()
    {
        throw new NotImplementedException(); //Issue where the list isn't the same type consistant
    }

    public int IndexOf(T item)
    {
        for (int i = 0; i < Count; i++)
        {
            if (this[i].Equals(item))
            {
                return i;
            }
        }
        return -1;
    }

    public void Insert(int index, T item)
    {
        this[index] = item;
    }

    public void SetWeight(T item, int newWeight)
    {
        int index = IndexOf(item);
        if (index >= 0)
        {
            SetWeight(index, newWeight);
        }
    }

    public void SetWeight(int index, int newWeight)
    {
        m_totalWeight -= m_elements[index].Item2;
        m_totalWeight += newWeight;

        m_elements.Insert(index, (m_elements[index].Item1, newWeight));
    }

    public bool Remove(T item)
    {
        int index = IndexOf(item);
        if(index >= 0)
        {
            RemoveAt(index);
            return true;
        }
        return false;
    }

    public void RemoveAt(int index)
    {
        m_totalWeight -= m_elements[index].Item2;
        m_elements.RemoveAt(index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return m_elements.GetEnumerator();
    }

}