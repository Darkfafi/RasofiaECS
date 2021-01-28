public abstract class EntityMasterComponent : EntityComponent
{
	public abstract bool IsComplete
	{
		get;
	}

	protected abstract void OnRefresh();

	internal void Refresh()
	{
		OnRefresh();
	}
}

public struct MasterFilterData<MasterT> : IEntityFilterData where MasterT : EntityMasterComponent
{
	public MasterT Master;

	public bool TrySetFilterData(Entity entity, EntityAdmin entityAdmin)
	{
		Master = entity.GetComponent<MasterT>();
		return Master.IsComplete;
	}
}
