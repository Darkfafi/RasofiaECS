using UnityEngine;

public class CardGame : MonoBehaviour
{
	private EntityAdmin _admin;

	protected void Awake()
	{
#warning TODO: Look into the following:
		/*
		 * Within the GDC talk `Overwatch Gameplay Architecture and Netcode`, it can be seen that less abstract / small components where used to define given components of the game
		 * Source: https://www.youtube.com/watch?v=W3aieHjyNvw
		 * Make it so that the Components being used are less abstract so that more direct communication can be applied onto them.
		 */
		_admin = EntityAdmin.Create(new CardGameLoopSystem());

		Entity game = _admin.CreateEntity
		(
			new GamePhaseTag(GamePhase.None)
		);

		Entity seat = _admin.CreateEntity
		(
			new ParentTag(game), 
			new SeatPhaseTag(SeatPhase.None)
		);

		_admin.CreateEntity
		(
			new ParentTag(seat), 
			new CardZoneTag(CardZone.Deck)
		);

		game.GetComponent<GamePhaseTag>().SetPhase(GamePhase.Setup);
	}

	protected void Update()
	{
		_admin.ExecuteSystems(Time.deltaTime);
	}
}


