public class CardGameLoopSystem : EntitySystemBase
{
	private CardGameMaster _cardGameMaster;

	public override void Initialize(EntityAdmin entityAdmin)
	{
		base.Initialize(entityAdmin);
		_cardGameMaster = EntityAdmin.GetSingletonComponent<CardGameMaster>();
		_cardGameMaster.GamePhaseTag.GamePhaseChangedEvent += OnGamePhaseChangedEvent;
	}

	public override void Deinitialize()
	{
		_cardGameMaster.GamePhaseTag.GamePhaseChangedEvent -= OnGamePhaseChangedEvent;
		_cardGameMaster = null;
		base.Deinitialize();
	}

	private void OnGamePhaseChangedEvent(GamePhase phase)
	{
		switch(phase)
		{
			case GamePhase.Setup:
				EntityAdmin.GetEntityFilter<MasterFilterData<SeatMaster>>().ForEach(x =>
				{
					Entity[] cards = CardHelperMethods.CreateFullDeckOfCards(EntityAdmin, true);
					for(int j = 0; j < cards.Length; j++)
					{
						CardHelperMethods.MoveToZone(cards[j], x.Master.DeckZone);
					}
				});
				break;
		}
	}
}