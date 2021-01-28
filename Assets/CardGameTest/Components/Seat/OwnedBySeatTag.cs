public class OwnedBySeatTag : EntityComponent
{
	public SeatMaster Owner
	{
		get; private set;
	}

	public OwnedBySeatTag(SeatMaster owner)
	{
		Owner = owner;
	}
}
