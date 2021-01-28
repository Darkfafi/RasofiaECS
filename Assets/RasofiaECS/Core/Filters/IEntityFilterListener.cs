public interface IEntityFilterListener<FilterDataT, FilterRefresherT> : IEntityFilterListener<FilterDataT>
	where FilterDataT : struct, IEntityFilterData
	where FilterRefresherT : FilterRefresher, new()
{
}

public interface IEntityFilterListener<FilterDataT>
   where FilterDataT : struct, IEntityFilterData
{
	void OnDataRegistered(FilterDataT filterData);
	void OnDataUnregistered(FilterDataT filterData);
}