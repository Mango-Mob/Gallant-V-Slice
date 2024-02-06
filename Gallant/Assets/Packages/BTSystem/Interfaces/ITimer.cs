namespace BTSystem.Interfaces
{
    public interface ITimer
    {
        public void UpdateTimer();
        public void SetTime(float time);
        public void Reset();
        public float GetRemainder();
    }
}