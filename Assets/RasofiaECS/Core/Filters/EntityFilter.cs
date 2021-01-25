using System.Collections.Generic;
using System.Linq;

public class EntityFilter<FilterDataT, FilterRefresherT> : IEntityFilter
	where FilterDataT : struct, IFilterData
	where FilterRefresherT : FilterRefresher, new()
{
	public delegate void EntityDataHandler(string entityId, FilterDataT filterData);
	public event EntityDataHandler DataAddedEvent;
	public event EntityDataHandler DataRemovedEvent;

	private List<IEntityFilterListener<FilterDataT, FilterRefresherT>> _listeners = new List<IEntityFilterListener<FilterDataT, FilterRefresherT>>();
	private Dictionary<string, FilterDataT> _idToFilterData = new Dictionary<string, FilterDataT>();
	private FilterRefresherT _refresher;

	public EntityFilter(EntityAdmin entityAdmin)
	{
		_refresher = new FilterRefresherT();
		_refresher.RefreshRequestEvent += OnRefreshRequestEvent;
		_refresher.Initialize(entityAdmin);
	}

	public FilterDataT[] GetAllData()
	{
		return _idToFilterData.Values.ToArray();
	}

	public void RegisterListener(IEntityFilterListener<FilterDataT, FilterRefresherT> listener, bool triggerForCurrentEntries = true)
	{
		if(!_listeners.Contains(listener))
		{
			_listeners.Add(listener);
			if(triggerForCurrentEntries)
			{
				FilterDataT[] allData = GetAllData();
				for(int i = 0, c = allData.Length; i < c; i++)
				{
					listener.OnDataRegistered(allData[i]);
				}
			}
		}
	}

	public void UnregisterListener(IEntityFilterListener<FilterDataT, FilterRefresherT> listener, bool triggerForCurrentEntries = true)
	{
		if(_listeners.Remove(listener))
		{
			if(triggerForCurrentEntries)
			{
				FilterDataT[] allData = GetAllData();
				for(int i = 0, c = allData.Length; i < c; i++)
				{
					listener.OnDataUnregistered(allData[i]);
				}
			}
		}
	}

	public bool TryGetData(string entityId, out FilterDataT filterData)
	{
		return _idToFilterData.TryGetValue(entityId, out filterData);
	}

	public void Dispose()
	{
		FilterDataT[] remainingData = GetAllData();
		var listeners = _listeners.ToArray();
		_refresher.RefreshRequestEvent -= OnRefreshRequestEvent;
		_refresher.Deinitialize();

		for(int i = 0, c = listeners.Length; i < c; i++)
		{
			for(int j = remainingData.Length - 1; j >= 0; j--)
			{
				listeners[i].OnDataUnregistered(remainingData[j]);
			}
		}

		_idToFilterData.Clear();
		_listeners.Clear();
	}

	private void Apply(Entity entity)
	{
		if(_idToFilterData.TryGetValue(entity.UniqueIdentifier, out FilterDataT filterData))
		{
			if(!filterData.TrySetFilterData(entity))
			{
				_idToFilterData.Remove(entity.UniqueIdentifier); 
				var listeners = _listeners.ToArray();
				for(int i = 0, c = listeners.Length; i < c; i++)
				{
					listeners[i].OnDataUnregistered(filterData);
				}
				DataRemovedEvent?.Invoke(entity.UniqueIdentifier, filterData);
			}
		}
		else
		{
			filterData = new FilterDataT();
			if(filterData.TrySetFilterData(entity))
			{
				_idToFilterData[entity.UniqueIdentifier] = filterData;
				var listeners = _listeners.ToArray();
				for(int i = 0, c = listeners.Length; i < c; i++)
				{
					listeners[i].OnDataRegistered(filterData);
				}
				DataAddedEvent?.Invoke(entity.UniqueIdentifier, filterData);
			}
		}
	}

	private void OnRefreshRequestEvent(Entity entity)
	{
		Apply(entity);
	}
}