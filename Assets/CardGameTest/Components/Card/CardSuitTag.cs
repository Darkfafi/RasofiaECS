public class CardSuitTag : EntityComponent
{
	public CardSuit CardSuit;

	public override string GetExtraInfo()
	{
		return CardSuit.ToString();
	}
}
