﻿using System;
using System.Collections.Generic;

public sealed class Entity
{
	public readonly string UniqueIdentifier;

	public delegate void EntityComponentHandler(Entity entity, EntityComponent component);
	public event EntityComponentHandler ComponentAddedEvent;
	public event EntityComponentHandler ComponentRemovedEvent;

	private List<EntityComponent> _components = new List<EntityComponent>();

	public Entity()
	{
		UniqueIdentifier = Guid.NewGuid().ToString();
	}

	public void AddComponent(EntityComponent entityComponent)
	{
		if(!_components.Contains(entityComponent) && entityComponent.Parent == null)
		{
			_components.Add(entityComponent);
			entityComponent.Parent = this;
			ComponentAddedEvent?.Invoke(this, entityComponent);
		}
	}

	public void RemoveComponent(EntityComponent entityComponent)
	{
		if(_components.Remove(entityComponent))
		{
			ComponentRemovedEvent?.Invoke(this, entityComponent);
			entityComponent.Parent = null;
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

	public EntityComponent[] GetAllComponents()
	{
		return _components.ToArray();
	}
}