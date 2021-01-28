using System;
using System.Collections.Generic;

public class Entity
{
	public readonly string UniqueIdentifier;

	public delegate void EntityComponentHandler(Entity entity, EntityComponent component);
	public event EntityComponentHandler ComponentAddedEvent;
	public event EntityComponentHandler ComponentRemovedEvent;

	private List<EntityComponent> _components = new List<EntityComponent>();

	internal Entity(string identifier, EntityComponent[] components)
	{
		UniqueIdentifier = identifier;
		if(components != null)
		{
			AddComponents(components);
		}
	}

	public void AddComponents(EntityComponent[] entityComponents)
	{
		for(int i = 0; i < entityComponents.Length; i++)
		{
			AddComponent(entityComponents[i]);
		}
	}

	public void AddComponent(EntityComponent entityComponent)
	{
		if(!_components.Contains(entityComponent) && entityComponent.Entity == null)
		{
			_components.Add(entityComponent);
			entityComponent.Initialize(this);
			ComponentAddedEvent?.Invoke(this, entityComponent);
		}
	}

	public void RemoveComponent(EntityComponent entityComponent)
	{
		if(_components.Remove(entityComponent))
		{
			ComponentRemovedEvent?.Invoke(this, entityComponent);
			entityComponent.Deinitialize();
		}
	}

	public T GetComponent<T>() where T : EntityComponent
	{
		for(int i = 0, c = _components.Count; i < c; i++)
		{
			if(_components[i] is T castedComp)
			{
				return castedComp;
			}
		}
		return null;
	}

	public T GetComponent<T>(Predicate<T> predicate) where T : EntityComponent
	{
		for(int i = 0, c = _components.Count; i < c; i++)
		{
			if(_components[i] is T castedComp && predicate(castedComp))
			{
				return castedComp;
			}
		}
		return null;
	}

	public T[] GetComponents<T>() where T : EntityComponent
	{
		List<T> components = new List<T>();
		for(int i = 0, c = _components.Count; i < c; i++)
		{
			if(_components[i] is T castedComp)
			{
				components.Add(castedComp);
			}
		}
		return components.ToArray();
	}

	public EntityComponent[] GetAllComponents()
	{
		return _components.ToArray();
	}
}