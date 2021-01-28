public class CardColorTag : EntityComponent
{
    public CardColor CardColor;

	public override string GetExtraInfo()
	{
		return CardColor.ToString();
	}
}
