public class CardZoneTag : EntityComponent
{
	public CardZone CardZone
	{
		get; private set;
	}

	public CardZoneTag(CardZone cardZone)
	{
		CardZone = cardZone;
	}
}
