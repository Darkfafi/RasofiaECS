using UnityEngine;

public class CardGame : MonoBehaviour, IEntityAdminHolder
{
	public EntityAdmin EntityAdmin
	{
		get; private set;
	}

	protected void Awake()
	{
		EntityAdmin = EntityAdmin.Create(new CardGameLoopSystem());

		// Singleton Components
		EntityAdmin.AddSingletonComponent(new CardGameMaster(), new GamePhaseTag(GamePhase.None));
		EntityAdmin.AddSingletonComponent(new ActionsMaster());

		// Setup Seat / Gameboard
		SeatMaster seatMaster;
		EntityAdmin.CreateEntity
		(
			seatMaster = new SeatMaster(),
			new SeatPhaseTag(SeatPhase.None)
		);

		CardZoneHelperMethods.CreateCardZone(EntityAdmin, CardZone.Deck, seatMaster);
		CardZoneHelperMethods.CreateCardZone(EntityAdmin, CardZone.Play, seatMaster);

		// Start Game
		EntityAdmin.GetSingletonComponent<CardGameMaster>().GamePhaseTag.SetPhase(GamePhase.Setup);
	}

	protected void Update()
	{
		EntityAdmin.ExecuteSystems(Time.deltaTime);
	}
}


