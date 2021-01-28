using System;
using System.Collections.Generic;
using System.Linq;

public sealed class EntityAdmin : IDisposable
{
	public delegate void EntityHandler(Entity entity);
	public event EntityHandler EntityAddedEvent;
	public event EntityHandler EntityRemovedEvent;

	public bool IsRunning
	{
		get; private set;
	}

	private Dictionary<string, Entity> _entitiesMap = new Dictionary<string, Entity>();
	private Dictionary<Type, IEntityFilter> _entityFiltersMap = new Dictionary<Type, IEntityFilter>();

	private List<EntitySystemBase> _systems = new List<EntitySystemBase>();

	public static EntityAdmin Create(params EntitySystemBase[] systems)
	{
		return new EntityAdmin(systems);
	}

	internal EntityAdmin(EntitySystemBase[] systems)
	{
		if(systems != null)
		{
			AddSystems(systems);
		}
	}

	public void AddSingletonComponent<T>(T singletonComponent, params EntityComponent[] entityComponents) where T : EntityMasterComponent
	{
		string singletonId = typeof(T).FullName;
		Entity entity = CreateEntity(singletonId, singletonComponent);
		entity.AddComponents(entityComponents);
	}

	public T GetSingletonComponent<T>() where T : EntityMasterComponent
	{
		return GetSingletonEntity<T>()?.GetComponent<T>();
	}

	public Entity GetSingletonEntity<T>() where T : EntityMasterComponent
	{
		if(_entitiesMap.TryGetValue(typeof(T).FullName, out Entity entity))
		{
			return entity;
		}
		return null;
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
		if(!IsRunning)
		{
			IsRunning = true;
			for(int i = 0, c = _systems.Count; i < c; i++)
			{
				_systems[i].Initialize(this);
			}
		}

		for(int i = 0, c = _systems.Count; i < c; i++)
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
			if(IsRunning)
			{
				entitySystem.Initialize(this);
			}
		}
	}

	public void RemoveSystem(EntitySystemBase entitySystem)
	{
		if(_systems.Remove(entitySystem))
		{
			if(IsRunning)
			{
				entitySystem.Deinitialize();
			}
		}
	}

	public Entity CreateEntity(params EntityComponent[] entityComponents)
	{
		return CreateEntity(Guid.NewGuid().ToString(), entityComponents);
	}

	public Entity CreateEntity(string identifier, params EntityComponent[] entityComponents)
	{
		if(_entitiesMap.ContainsKey(identifier))
		{
			throw new Exception($"Duplicate Identifier, can't create Entity with ID \"{identifier}\"");
		}

		Entity entity = new Entity(identifier, entityComponents);
		AddEntity(entity);
		return entity;
	}

	public void DestroyEntity(Entity entity)
	{
		// Can't Remove Itself
		if(_entitiesMap.Remove(entity.UniqueIdentifier))
		{
			EntityRemovedEvent?.Invoke(entity);
			entity.ComponentAddedEvent -= OnEntityMarkedDirty;
			entity.ComponentRemovedEvent -= OnEntityMarkedDirty;
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
		where FilterDataT : struct, IEntityFilterData
	{
		return GetEntityFilter<FilterDataT, FilterRefresher>();
	}

	public EntityFilter<FilterDataT, FilterRefresherT> GetEntityFilter<FilterDataT, FilterRefresherT>()
		where FilterDataT : struct, IEntityFilterData
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
		entity.ComponentAddedEvent += OnEntityMarkedDirty;
		entity.ComponentRemovedEvent += OnEntityMarkedDirty;
		EntityAddedEvent?.Invoke(entity);
	}

	private void OnEntityMarkedDirty(Entity entity, EntityComponent component)
	{
		EntityMasterComponent[] masters = entity.GetComponents<EntityMasterComponent>();
		for(int i = masters.Length - 1; i >= 0; i--)
		{
			masters[i].Refresh();
		}
	}
}
