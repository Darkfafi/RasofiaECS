using System;
using System.Collections.Generic;
using System.Linq;

public sealed class EntityAdmin : Entity, IDisposable
{
	public delegate void EntityHandler(Entity entity);
	public event EntityHandler EntityAddedEvent;
	public event EntityHandler EntityRemovedEvent;

	private Dictionary<string, Entity> _entitiesMap = new Dictionary<string, Entity>();
	private Dictionary<Type, IEntityFilter> _entityFiltersMap = new Dictionary<Type, IEntityFilter>();

	private List<EntitySystemBase> _systems = new List<EntitySystemBase>();

	public static EntityAdmin Create(params EntitySystemBase[] systems)
	{
		return new EntityAdmin(systems, null);
	}

	public static EntityAdmin Create(params EntityComponent[] components)
	{
		return new EntityAdmin(null, components);
	}

	internal EntityAdmin(EntitySystemBase[] systems, EntityComponent[] singletonComponents)
		: base(singletonComponents)
	{
		if(systems != null)
		{
			AddSystems(systems);
		}
		AddEntity(this);
	}

	public Entity[] GetAllEntities()
	{
		return _entitiesMap.Values.ToArray();
	}

	public bool TryGetEntity(string entityId, out Entity entity)
	{
		return _entitiesMap.TryGetValue(entityId, out entity);
	}

	public void ExecuteSystems(float deltaTime)
	{
		for(int i = 0; i < _systems.Count; i++)
		{
			_systems[i].Execute(deltaTime);
		}
	}

	public void AddSystems(EntitySystemBase[] entitySystems)
	{
		for(int i = 0; i < entitySystems.Length; i++)
		{
			AddSystem(entitySystems[i]);
		}
	}

	public void AddSystem(EntitySystemBase entitySystem)
	{
		if(!_systems.Contains(entitySystem))
		{
			_systems.Add(entitySystem);
			entitySystem.Initialize(this);
		}
	}

	public void RemoveSystem(EntitySystemBase entitySystem)
	{
		if(_systems.Remove(entitySystem))
		{
			entitySystem.Deinitialize();
		}
	}

	public Entity CreateEntity(params EntityComponent[] entityComponents)
	{
		Entity entity = new Entity(entityComponents);
		AddEntity(entity);
		return entity;
	}

	public void DestroyEntity(Entity entity)
	{
		// Can't Remove Itself
		if(entity.UniqueIdentifier != UniqueIdentifier && 
			_entitiesMap.Remove(entity.UniqueIdentifier))
		{
			EntityRemovedEvent?.Invoke(entity);
		}
	}

	public void DestroyEntity(string entityId)
	{
		if(_entitiesMap.TryGetValue(entityId, out Entity entity))
		{
			DestroyEntity(entity);
		}
	}

	public IEntityFilter<FilterDataT> GetEntityFilter<FilterDataT>()
		where FilterDataT : struct, IFilterData
	{
		return GetEntityFilter<FilterDataT, FilterRefresher>();
	}

	public EntityFilter<FilterDataT, FilterRefresherT> GetEntityFilter<FilterDataT, FilterRefresherT>()
		where FilterDataT : struct, IFilterData
		where FilterRefresherT : FilterRefresher, new()
	{
		Type filterDataType = typeof(FilterDataT);
		if(!_entityFiltersMap.TryGetValue(filterDataType, out IEntityFilter baseFilter))
		{
			EntityFilter<FilterDataT, FilterRefresherT> castedFilter = new EntityFilter<FilterDataT, FilterRefresherT>(this);
			_entityFiltersMap[filterDataType] = castedFilter;
			return castedFilter;
		}
		return baseFilter as EntityFilter<FilterDataT, FilterRefresherT>;
	}

	public void Dispose()
	{
		foreach(var pair in _entityFiltersMap)
		{
			pair.Value.Dispose();
		}
		_entityFiltersMap.Clear();

		Entity[] entities = _entitiesMap.Values.ToArray();
		for(int i = entities.Length - 1; i >= 0; i--)
		{
			DestroyEntity(entities[i]);
		}
		_entitiesMap.Clear();

		for(int i = _systems.Count - 1; i >= 0; i--)
		{
			RemoveSystem(_systems[i]);
		}
		_systems.Clear();
	}

	private void AddEntity(Entity entity)
	{
		_entitiesMap.Add(entity.UniqueIdentifier, entity);
		EntityAddedEvent?.Invoke(entity);
	}
}
