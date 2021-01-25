using System;
using System.Collections.Generic;

public class EntityAdmin : IDisposable
{
	public delegate void EntityHandler(Entity entity);
	public event EntityHandler EntityAddedEvent;
	public event EntityHandler EntityRemovedEvent;

	private List<Entity> _entities = new List<Entity>();
	private Dictionary<Type, IEntityFilter> _entityFiltersMap = new Dictionary<Type, IEntityFilter>();

	public Entity[] GetAllEntities()
	{
		return _entities.ToArray();
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
	}
}
