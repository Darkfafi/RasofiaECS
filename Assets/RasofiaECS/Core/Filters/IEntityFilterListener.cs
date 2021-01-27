public interface IEntityFilterListener<FilterDataT, FilterRefresherT> : IEntityFilterListener<FilterDataT>
	where FilterDataT : struct, IFilterData
	where FilterRefresherT : FilterRefresher, new()
{
}

public interface IEntityFilterListener<FilterDataT>
   where FilterDataT : struct, IFilterData
{
	void OnDataRegistered(FilterDataT filterData);
	void OnDataUnregistered(FilterDataT filterData);
}