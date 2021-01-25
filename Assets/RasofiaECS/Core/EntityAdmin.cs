using System;
using System.Collections.Generic;

public class EntityAdmin : IDisposable
{
	public delegate void EntityHandler(Entity entity);
	public event EntityHandler EntityAddedEvent;
	public event EntityHandler EntityRemovedEvent;

	private List<Entity> _entities = new List<Entity>();
	private Dictionary<Type, IEntityFilter> _entityFiltersMap = new Dictionary<Type, IEntityFilter>();

	private List<EntitySystemBase> _systems = new List<EntitySystemBase>();

	public Entity[] GetAllEntities()
	{
		return _entities.ToArray();
	}

	public void ExecuteSystems(float deltaTime)
	{
		for(int i = 0; i < _systems.Count; i++)
		{
			_systems[i].Execute(deltaTime);
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

	public void AddEntity(Entity entity)
	{
		if(!_entities.Contains(entity))
		{
			_entities.Add(entity);
			EntityAddedEvent?.Invoke(entity);
		}
	}

	public void RemoveEntity(Entity entity)
	{
		if(_entities.Remove(entity))
		{
			EntityRemovedEvent?.Invoke(entity);
		}
	}

	public EntityFilter<FilterDataT, FilterRefresher> GetEntityFilter<FilterDataT>()
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

		for(int i = _entities.Count - 1; i >= 0; i--)
		{
			RemoveEntity(_entities[i]);
		}
		_entities.Clear();

		for(int i = _systems.Count - 1; i >= 0; i--)
		{
			RemoveSystem(_systems[i]);
		}
		_systems.Clear();
	}
}
