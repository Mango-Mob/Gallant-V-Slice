namespace BTSystem.Interfaces
{
    public interface ITimer
    {
        public void UpdateTimer();
        public void Reset();
        public float GetRemainder();
    }
}