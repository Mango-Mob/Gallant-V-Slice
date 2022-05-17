using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//TODO
public class ProbabilityList<T>
{
    private List<T> m_objects = new List<T>();
    private List<float> m_weights = new List<float>();
    

    private List<(T, int)> m_elements = new List<(T, int)>();
    private float m_totalWeight = 0f;

    public void Add(T toAdd, int weight)
    {
        m_elements.Add((toAdd, weight));
        m_totalWeight += weight;
    }

    public void Remove(T toRemove)
    {
        for (int i = m_objects.Count - 1; i >= 0; i--)
        {
            if(m_objects[i].Equals(toRemove))
            {
                RemoveAt(i);
                return;
            }
        }
    }

    public void RemoveAt(int indexToRemove)
    {
        m_objects.RemoveAt(indexToRemove);

        m_totalWeight -= m_weights[indexToRemove];
        m_weights.RemoveAt(indexToRemove);
    }

    public T Get(int index)
    {
        return m_objects[index];
    }
}