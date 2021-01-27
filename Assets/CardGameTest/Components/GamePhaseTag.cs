using System;

public class GamePhaseTag : EntityComponent
{
	public event Action<GamePhase> GamePhaseChangedEvent;

	public GamePhase GamePhase
	{
		get; private set;
	}

	public GamePhaseTag(GamePhase gamePhase)
	{
		GamePhase = gamePhase;
	}

	public void SetPhase(GamePhase phase)
	{
		if(GamePhase != phase)
		{
			GamePhase = phase;
			GamePhaseChangedEvent?.Invoke(GamePhase);
		}
	}
}