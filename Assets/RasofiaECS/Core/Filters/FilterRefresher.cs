using System;

public class FilterRefresher
{
	public event Action<Entity> RefreshRequestEvent;

	public EntityAdmin EntityAdmin
	{
		get; private set;
	}

	public void Initialize(EntityAdmin entityAdmin)
	{
		EntityAdmin = entityAdmin;
		EntityAdmin.EntityAddedEvent += OnEntityAddedEvent;
		EntityAdmin.EntityRemovedEvent += OnEntityRemovedEvent;

		Entity[] allEntities = EntityAdmin.GetAllEntities();
		for(int i = 0, c = allEntities.Length; i < c; i++)
		{
			OnEntityAddedEvent(allEntities[i]);
		}
	}

	public void Deinitialize()
	{
		EntityAdmin.EntityAddedEvent -= OnEntityAddedEvent;
		EntityAdmin.EntityRemovedEvent -= OnEntityRemovedEvent;

		Entity[] allEntities = EntityAdmin.GetAllEntities();
		for(int i = 0, c = allEntities.Length; i < c; i++)
		{
			UnwatchEntity(allEntities[i]);
		}

		EntityAdmin = null;
	}

	protected virtual void WatchEntity(Entity entity)
	{
		entity.ComponentAddedEvent += OnComponentAddedEvent;
		entity.ComponentRemovedEvent += OnComponentRemovedEvent;
		EntityComponent[] entityComponents = entity.GetAllComponents();
		for(int i = 0, c = entityComponents.Length; i < c; i++)
		{
			WatchComponent(entityComponents[i]);
		}	
	}

	protected virtual void UnwatchEntity(Entity entity)
	{
		entity.ComponentAddedEvent -= OnComponentAddedEvent;
		entity.ComponentRemovedEvent -= OnComponentRemovedEvent;
		EntityComponent[] entityComponents = entity.GetAllComponents();
		for(int i = 0, c = entityComponents.Length; i < c; i++)
		{
			UnwatchComponent(entityComponents[i]);
		}
	}

	protected virtual void WatchComponent(EntityComponent component)
	{

	}

	protected virtual void UnwatchComponent(EntityComponent component)
	{

	}

	protected void FireRefreshRequest(Entity entity)
	{
		RefreshRequestEvent?.Invoke(entity);
	}

	protected void OnComponentAddedEvent(Entity entity, EntityComponent component)
	{
		WatchComponent(component);
		FireRefreshRequest(entity);
	}

	protected void OnComponentRemovedEvent(Entity entity, EntityComponent component)
	{
		UnwatchComponent(component);
		FireRefreshRequest(entity);
	}

	private void OnEntityAddedEvent(Entity entity)
	{
		WatchEntity(entity);
		FireRefreshRequest(entity);
	}

	private void OnEntityRemovedEvent(Entity entity)
	{
		UnwatchEntity(entity);
		FireRefreshRequest(entity);
	}
}
