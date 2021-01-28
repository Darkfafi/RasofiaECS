using System;

public class InsideZoneTag : EntityComponent
{
	public event Action<CardZoneMaster> OwnerZoneChangedEvent;

	public CardZoneMaster Zone
	{
		get; private set;
	}

	public InsideZoneTag(CardZoneMaster zone)
	{
		Zone = zone;
	}

	public void SetOwnerZone(CardZoneMaster zone)
	{
		if(Zone != zone)
		{
			Zone = zone;
			OwnerZoneChangedEvent?.Invoke(Zone);
		}
	}
}
