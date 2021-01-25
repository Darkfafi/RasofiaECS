public interface IEntityFilterListener<FilterDataT, FilterRefresherT> 
	where FilterDataT : struct, IFilterData
	where FilterRefresherT : FilterRefresher, new()
{
	void OnDataRegistered(FilterDataT filterData);
	void OnDataUnregistered(FilterDataT filterData);
}