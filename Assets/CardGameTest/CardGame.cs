using UnityEngine;

public class CardGame : MonoBehaviour
{
	private EntityAdmin _admin;

	protected void Awake()
	{
		_admin = EntityAdmin.Create(new CardGameLoopSystem());

		// Singleton Components
		_admin.AddSingletonComponent(new CardGameMaster(), new GamePhaseTag(GamePhase.None));

		Entity seat = _admin.CreateEntity
		(
			new SeatMaster(),
			new SeatPhaseTag(SeatPhase.None)
		);

		_admin.CreateEntity
		(
			new CardZoneMaster(),
			new CardZoneTag(CardZone.Deck),
			new ParentTag(seat)
		);

		_admin.GetSingletonComponent<CardGameMaster>().GamePhaseTag.SetPhase(GamePhase.Setup);
	}

	protected void Update()
	{
		_admin.ExecuteSystems(Time.deltaTime);
	}
}


