public class CardGameMaster : EntityMasterComponent
{
	public GamePhaseTag GamePhaseTag
	{
		get; private set;
	}

	public override bool IsComplete
	{
		get
		{
			return GamePhaseTag != null;
		}
	}

	protected override void OnRefresh()
	{
		GamePhaseTag = Entity.GetComponent<GamePhaseTag>();
	}
}
