using System;

public class ParentTag : EntityComponent
{
	public event Action<Entity> ParentChangedEvent;

	public Entity Parent
	{
		get; private set;
	}

	public ParentTag(Entity parent)
	{
		Parent = parent;
	}

	public void SetOwnerZone(Entity parent)
	{
		if(Parent != parent)
		{
			Parent = parent;
			ParentChangedEvent?.Invoke(Parent);
		}
	}
}
