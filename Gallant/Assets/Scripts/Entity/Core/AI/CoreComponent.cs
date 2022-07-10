namespace EntitySystem.Core.AI
{
    public abstract class CoreComponent
    {
        public Entity Owner { get; protected set; }

        public CoreComponent(Entity _owner)
        {
            Owner = _owner;
        }
        private CoreComponent() { }

        public abstract void Update(float deltaTime);
    }
}
