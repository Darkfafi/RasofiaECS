using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private EntityAdmin _admin;

	protected void Awake()
	{
        _admin = new EntityAdmin();

        for(int i = 0; i <= 60; i++)
        {
            Entity testEntity = new Entity();
            testEntity.AddComponent(new TestComponent());
            testEntity.GetComponent<TestComponent>().SetHealth(i);
            _admin.AddEntity(testEntity);
        }

        EntityFilter<TestFilterData, TestRefresher> testFilter = _admin.GetEntityFilter<TestFilterData, TestRefresher>();
    }

	protected void Update()
	{
        if(Input.GetKeyDown(KeyCode.Space))
        {
            _admin.GetAllEntities()[0].GetComponent<TestComponent>().SetHealth(Time.time % 5 > 2.5f ? 20 : 60);
        }
	}
}

public class TestComponent : EntityComponent
{
    public event Action<TestComponent> TestUpdatedEvent;

    public int Health
    {
        get; private set;
    }

    public void SetHealth(int h)
    {
        Health = h;
        TestUpdatedEvent?.Invoke(this);
    }
}

public class TestRefresher : FilterRefresher
{
	protected override void WatchComponent(EntityComponent component)
	{
		base.WatchComponent(component);
        if(component is TestComponent testComp)
        {
            testComp.TestUpdatedEvent += OnTestUpdatedEvent;
        }
    }

	protected override void UnwatchComponent(EntityComponent component)
	{
        if(component is TestComponent testComp)
        {
            testComp.TestUpdatedEvent -= OnTestUpdatedEvent;
        }
		base.UnwatchComponent(component);
    }

	private void OnTestUpdatedEvent(TestComponent testComp)
	{
        FireRefreshRequest(testComp.Parent);
	}
}
public struct TestFilterDataLight : IFilterData
{
    public TestComponent TestComponent;

    public bool TrySetFilterData(Entity entity)
    {
        TestComponent = entity.GetComponent<TestComponent>();
        return TestComponent != null;
    }
}

public struct TestFilterData : IFilterData
{
    public TestComponent TestComponent;

    public bool TrySetFilterData(Entity entity)
    {
        TestComponent = entity.GetComponent<TestComponent>();
        return TestComponent != null && TestComponent.Health >= 50;
    }
}
