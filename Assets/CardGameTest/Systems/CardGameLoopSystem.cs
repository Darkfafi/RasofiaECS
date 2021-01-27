using System.Collections;
using UnityEngine;

public class CardGameLoopSystem : EntitySystemBase, IEntityFilterListener<GameLoopData>
{
	private IEntityFilter<GameLoopData> _gameLoopFilter;

	public override void Initialize(EntityAdmin entityAdmin)
	{
		base.Initialize(entityAdmin);
		_gameLoopFilter = entityAdmin.GetEntityFilter<GameLoopData>();
		_gameLoopFilter.RegisterListener(this);
	}

	public override void Deinitialize()
	{
		_gameLoopFilter.UnregisterListener(this);
		_gameLoopFilter = null;
		base.Deinitialize();
	}

	public void OnDataRegistered(GameLoopData filterData)
	{
		filterData.GamePhaseTag.GamePhaseChangedEvent += OnGamePhaseChangedEvent;
		OnGamePhaseChangedEvent(filterData.GamePhaseTag.GamePhase);
	}

	public void OnDataUnregistered(GameLoopData filterData)
	{
		filterData.GamePhaseTag.GamePhaseChangedEvent -= OnGamePhaseChangedEvent;
		OnGamePhaseChangedEvent(GamePhase.End);
	}

	private void OnGamePhaseChangedEvent(GamePhase phase)
	{
		switch(phase)
		{
			case GamePhase.Setup:
				SeatDeckData[] allSeatDeckData = EntityAdmin.GetEntityFilter<SeatDeckData>().GetAllData();
				for(int i = 0; i < allSeatDeckData.Length; i++)
				{
					Entity[] cards = CardHelperMethods.CreateFullDeckOfCards(EntityAdmin, true);
					for(int j = 0; j < cards.Length; j++)
					{
						CardHelperMethods.MoveToZone(cards[j], allSeatDeckData[i].CardZoneTag);
					}
				}
				break;
		}
	}
}

public struct GameLoopData : IFilterData
{
	public GamePhaseTag GamePhaseTag;

	public bool TrySetFilterData(Entity entity, EntityAdmin entityAdmin)
	{
		GamePhaseTag = entity.GetComponent<GamePhaseTag>();
		return GamePhaseTag != null;
	}
}

public struct SeatDeckData : IFilterData
{
	public CardZoneTag CardZoneTag;

	public bool TrySetFilterData(Entity entity, EntityAdmin entityAdmin)
	{
		CardZoneTag = entity.GetComponent<CardZoneTag>();
		return CardZoneTag != null && CardZoneTag.CardZone == CardZone.Deck;
	}
}