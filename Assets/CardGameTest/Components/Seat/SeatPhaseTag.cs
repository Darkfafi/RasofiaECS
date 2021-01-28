public class SeatPhaseTag : EntityComponent
{
	public SeatPhase SeatPhase
	{
		get; private set;
	}

	public SeatPhaseTag(SeatPhase seatPhase)
	{
		SeatPhase = seatPhase;
	}

	public override string GetExtraInfo()
	{
		return SeatPhase.ToString();
	}
}