using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//TODO
public class ProbabilityList<T>
{
    private List<T> m_objects = new List<T>();
    private List<float> m_probability = new List<float>();

    public void Add(T toAdd, float prob = -1)
    {
        if (prob == -1)
        {
            prob = 1f / m_objects.Count;
        }

        m_objects.Add(toAdd);
        m_probability.Add(prob);
        Update();
    }

    private void Update()
    {
        float size = 0f;
        foreach (var prob in m_probability)
        {
            size += prob;
        }

        if(size > 1.0f)
        {
            for (int i = 0; i < m_probability.Count; i++)
            {
                m_probability[i] = m_probability[i] / size;
            }
        }
    }
}