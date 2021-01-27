using System;

public interface IEntityFilter<FilterDataT> : IEntityFilter
	where FilterDataT : struct, IFilterData
{
	FilterDataT[] GetAllData();
	void RegisterListener(IEntityFilterListener<FilterDataT> listener, bool triggerForCurrentEntries = true);
	void UnregisterListener(IEntityFilterListener<FilterDataT> listener, bool triggerForCurrentEntries = true);
	bool TryGetData(string entityId, out FilterDataT filterData);
}

public interface IEntityFilter : IDisposable
{

}
