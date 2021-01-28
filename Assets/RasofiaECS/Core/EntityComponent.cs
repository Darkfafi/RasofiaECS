public abstract class EntityComponent
{
	public Entity Entity
	{
		get; private set;
	}

	internal void Initialize(Entity entity)
	{
		if(Entity == null)
		{
			Entity = entity;
		}
	}

	internal void Deinitialize()
	{
		if(Entity != null)
		{
			Entity = null;
		}
	}

	public struct FilterData : IEntityFilterData
	{
		public EntityComponent EntityComponent;

		public bool TrySetFilterData(Entity entity, EntityAdmin entityAdmin)
		{
			EntityComponent = entity.GetComponent<EntityComponent>();
			return EntityComponent != null;
		}
	}
}