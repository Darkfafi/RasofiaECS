public class CardZoneMaster : EntityMasterComponent
{
	public override bool IsComplete
	{
		get
		{
			return CardZoneTag != null;
		}
	}

	public CardZoneTag CardZoneTag
	{
		get; private set;
	}

	protected override void OnRefresh()
	{
		CardZoneTag = Entity.GetComponent<CardZoneTag>();
	}
}
