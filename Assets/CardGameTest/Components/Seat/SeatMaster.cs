using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	public CardZoneTag DeckZone
	{
		get; private set;
	}

	protected override void OnRefresh()
	{
		SeatPhaseTag = Entity.GetComponent<SeatPhaseTag>();
		DeckZone = Entity.GetComponent<CardZoneTag>(x => x.CardZone == CardZone.Deck);
	}
}
