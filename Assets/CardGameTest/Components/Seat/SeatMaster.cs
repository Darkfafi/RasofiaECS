using System;
using System.Collections.Generic;

public class SeatMaster : EntityMasterComponent
{
	public override bool IsComplete
	{
		get
		{
			return SeatPhaseTag != null;
		}
	}

	public SeatPhaseTag SeatPhaseTag
	{
		get; private set;
	}

	private Dictionary<CardZone, CardZoneMaster> _cardZones = new Dictionary<CardZone, CardZoneMaster>();

	public CardZoneMaster GetCardZoneMaster(CardZone cardZone)
	{
		if(_cardZones.TryGetValue(cardZone, out CardZoneMaster cardZoneMaster))
		{
			return cardZoneMaster;
		}
		return null;
	}

	protected override void OnRefresh()
	{
		SeatPhaseTag = Entity.GetComponent<SeatPhaseTag>();
		CardZone[] cardZones = Enum.GetValues(typeof(CardZone)) as CardZone[];

		for(int i = 0; i < cardZones.Length; i++)
		{
			CardZoneMaster cardZoneMaster = Entity.GetComponent<CardZoneMaster>(x => x.CardZoneTag?.CardZone == cardZones[i]);
			if(cardZoneMaster != null)
			{
				_cardZones[cardZones[i]] = cardZoneMaster;
			}
			else
			{
				_cardZones.Remove(cardZones[i]);
			}
		}
	}
}
