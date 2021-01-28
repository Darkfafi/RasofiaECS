public class CardMaster : EntityMasterComponent
{
	public CardColorTag CardColorTag
	{
		get; private set;
	}

	public CardSuitTag CardSuitTag
	{
		get; private set;
	}

	public CardTypeTag CardTypeTag
	{
		get; private set;
	}

	public JokerCardTypeTag JokerCardTypeTag
	{
		get; private set;
	}

	public override bool IsComplete
	{
		get
		{
			return CardColorTag != null && CardSuitTag != null && CardTypeTag != null && JokerCardTypeTag != null;
		}
	}

	protected override void OnRefresh()
	{
		CardColorTag = Entity.GetComponent<CardColorTag>();
		CardSuitTag = Entity.GetComponent<CardSuitTag>();
		CardTypeTag = Entity.GetComponent<CardTypeTag>();
		JokerCardTypeTag = Entity.GetComponent<JokerCardTypeTag>();
	}
}
