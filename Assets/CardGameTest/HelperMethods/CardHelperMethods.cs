using System;
using System.Collections.Generic;

public static class CardHelperMethods
{
	public static Entity CreateCardEntity(EntityAdmin admin, CardType cardType, CardSuit cardSuit)
	{
		return admin.CreateEntity
		(
			new CardMaster(),
			new CardTypeTag() { CardType = cardType },
			new CardSuitTag() { CardSuit = cardSuit },
			new CardColorTag() { CardColor = GetCardColorForSuit (cardSuit) }
		);
	}

	public static Entity CreateJokerCardEntity(EntityAdmin admin, CardColor cardColor)
	{
		return admin.CreateEntity
		(
			new CardMaster(),
			new CardColorTag() { CardColor = cardColor },	
			new JokerCardTypeTag()
		);
	}

	public static Entity[] CreateFullDeckOfCards(EntityAdmin admin, bool includeJokers)
	{
		List<Entity> allCards = new List<Entity>();
		CardSuit[] cardSuits = Enum.GetValues(typeof(CardSuit)) as CardSuit[];
		CardType[] cardTypes = Enum.GetValues(typeof(CardType)) as CardType[];

		for(int i = 0; i < cardSuits.Length; i++)
		{
			CardSuit cardSuit = cardSuits[i];
			for(int j = 0; j < cardTypes.Length; j++)
			{
				CardType cardType = cardTypes[j];
				allCards.Add(CreateCardEntity(admin, cardType, cardSuit));
			}
		}

		if(includeJokers)
		{
			CardColor[] cardColor = Enum.GetValues(typeof(CardColor)) as CardColor[];
			for(int i = 0; i < cardColor.Length; i++)
			{
				allCards.Add(CreateJokerCardEntity(admin, cardColor[i]));
			}
		}

		return allCards.ToArray();
	}

	public static void MoveToZone(Entity entity, CardZoneMaster zone)
	{
		InsideZoneTag ownerZoneTag = entity.GetComponent<InsideZoneTag>();
		if(ownerZoneTag != null)
		{
			if(zone == null)
			{
				entity.RemoveComponent(ownerZoneTag);
			}
			else
			{
				ownerZoneTag.SetOwnerZone(zone);
			}
		}
		else if(zone != null)
		{
			entity.AddComponent(new InsideZoneTag(zone));
		}
	}

	public static CardColor GetCardColorForSuit(CardSuit suit)
	{
		switch(suit)
		{
			case CardSuit.Clubs:
			case CardSuit.Spades:
				return CardColor.Black;
			case CardSuit.Diamonds:
			case CardSuit.Hearts:
				return CardColor.Red;
			default:
				throw new NotSupportedException($"CardSuit {suit} is not mapped to a CardColor");
		}
	}
}