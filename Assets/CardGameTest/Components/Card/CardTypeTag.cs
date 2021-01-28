public class CardTypeTag : EntityComponent
{
	public CardType CardType;

	public override string GetExtraInfo()
	{
		return CardType.ToString();
	}
}
