namespace EntitySystem.Core.AI
{
    public abstract class CoreComponent
    {
        public AIEntity Owner { get; protected set; }

        public CoreComponent(AIEntity _owner)
        {
            Owner = _owner;
        }
        private CoreComponent() { }

        public abstract void Update(float deltaTime);
    }
}
