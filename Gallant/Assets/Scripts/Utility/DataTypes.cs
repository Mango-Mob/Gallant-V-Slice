using UnityEngine;

namespace Utility
{
    public struct FloatRange
    {
        public FloatRange(float _min, float _max) { min = _min; max = _max; }

        public float min;
        public float max;

        public float GetRandom()
        {
            return Random.Range(min, max);
        }
    }
}
