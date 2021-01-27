using System;

public class InsideZoneTag : EntityComponent
{
	public event Action<CardZoneTag> OwnerZoneChangedEvent;

	public CardZoneTag Zone
	{
		get; private set;
	}

	public InsideZoneTag(CardZoneTag zone)
	{
		Zone = zone;
	}

	public void SetOwnerZone(CardZoneTag zone)
	{
		if(Zone != zone)
		{
			Zone = zone;
			OwnerZoneChangedEvent?.Invoke(Zone);
		}
	}
}
