using System;
using System.Collections.Generic;

public static class CardZoneHelperMethods
{
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

	public static Entity CreateCardZone(EntityAdmin entityAdmin, CardZone cardZoneType, SeatMaster owner)
	{
		Entity cardZone = entityAdmin.CreateEntity
		(
			new CardZoneMaster(),
			new CardZoneTag(cardZoneType)
		);

		if(owner != null)
		{
			cardZone.AddComponent(owner);
		}

		return cardZone;
	}
}