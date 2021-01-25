public class EntitySystemBase
{
	protected EntityAdmin EntityAdmin
	{
		get; private set;
	}

	public virtual void Initialize(EntityAdmin entityAdmin)
	{
		if(EntityAdmin != null)
		{
			Deinitialize();
		}

		EntityAdmin = entityAdmin;
	}

	public virtual void Execute(float deltaTime)
	{

	}

	public virtual void Deinitialize()
	{
		EntityAdmin = null;
	}
}
