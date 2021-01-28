using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using System.Linq;

public class EntityAdminTreeEditorWindow : EditorWindow
{
	private EntityAdminTree _entityAdminTreeView;

	[MenuItem("Rasofia/EntityAdmin/TreeView")]
	static void OpenWindow()
	{
		EntityAdminTreeEditorWindow window = GetWindow<EntityAdminTreeEditorWindow>();
		window.titleContent = new GUIContent("EntityAdminTreeView");
		window.Show();
	}

	protected void Update()
	{
		if(_entityAdminTreeView != null && _entityAdminTreeView.HasTarget)
		{
			_entityAdminTreeView.Reload();
			Repaint();
		}
	}

	protected void OnEnable()
	{
		_entityAdminTreeView = new EntityAdminTree(new TreeViewState());
	}

	protected void OnGUI()
	{
		if(_entityAdminTreeView != null)
		{
			GameObject activeObject = Selection.activeGameObject;

			IEntityAdminHolder potentialTarget = activeObject != null ? activeObject.GetComponent<IEntityAdminHolder>() : null;
			if(!_entityAdminTreeView.HasTarget || potentialTarget != _entityAdminTreeView.Target && potentialTarget != null)
			{
				_entityAdminTreeView.SetTarget(potentialTarget);
			}

			if(_entityAdminTreeView.HasTarget)
			{
				_entityAdminTreeView.OnGUI(new Rect(0, 0, position.width, position.height));
			}
			else
			{
				EditorGUILayout.LabelField($"No Active {nameof(IEntityAdminHolder)} Selected");
			}
		}
	}

	private class EntityAdminTree : TreeView
	{
		public bool HasTarget => Target != null && Target.EntityAdmin != null;

		public IEntityAdminHolder Target
		{
			get; private set;
		}

		private Dictionary<int, Entity> _entityMap = new Dictionary<int, Entity>();
		private Dictionary<int, EntityComponent> _componentMap = new Dictionary<int, EntityComponent>();

		public EntityAdminTree(TreeViewState state)
			: base(state, new MultiColumnHeader(new MultiColumnHeaderState(new MultiColumnHeaderState.Column[]
			{
				new MultiColumnHeaderState.Column
				{
					headerContent = new GUIContent("EntityID"),
					autoResize = true,
					width = 100,
				},
				new MultiColumnHeaderState.Column
				{
					headerContent = new GUIContent("Extra Info"),
					autoResize = true,
				}
			})))
		{
			rowHeight = 20;
			showAlternatingRowBackgrounds = true;
			showBorder = true;
			Reload();
		}

		public IReadOnlyDictionary<int, Entity> EntityMap => _entityMap;
		private IReadOnlyDictionary<int, EntityComponent> ComponentMap => _componentMap;

		public void SetTarget(IEntityAdminHolder entityAdminHolder)
		{
			if(Target != entityAdminHolder)
			{
				Target = entityAdminHolder;
				multiColumnHeader.ResizeToFit();
				Reload();
			}
		}

		protected override void RowGUI(RowGUIArgs args)
		{
			if(ComponentMap.TryGetValue(args.item.id, out EntityComponent entityComponent))
			{
				for(int i = 0; i < args.GetNumVisibleColumns(); ++i)
				{
					Rect cellRect = args.GetCellRect(i);
					args.rowRect = cellRect;
					switch(i)
					{
						case 0:
							base.RowGUI(args);
							break;
						case 1:
							GUI.Label(cellRect, entityComponent.GetExtraInfo());
							break;
					}
				}
			}
			else
			{
				base.RowGUI(args);
			}
		}

		protected override TreeViewItem BuildRoot()
		{
			int id = 0;
			_entityMap.Clear();
			_componentMap.Clear();

			TreeViewItem root = CreateTreeViewItem(null);
			root.depth = -1;

			if(Target != null && Target.EntityAdmin != null)
			{
				TreeViewItem adminItem = CreateTreeViewItem(Target.EntityAdmin.GetType().Name);
				root.AddChild(adminItem);

				EntitySystemBase[] systems = Target.EntityAdmin.GetAllSystems();
				TreeViewItem systemsItem = CreateTreeViewItem($"Systems ({systems.Length})");
				for(int i = 0; i < systems.Length; i++)
				{
					systemsItem.AddChild(CreateTreeViewItem(systems[i].GetType().Name));
				}

				Entity[] entities = Target.EntityAdmin.GetAllEntities();
				TreeViewItem singletonsItem = CreateTreeViewItem($"Singletons");
				TreeViewItem entitiesItem = CreateTreeViewItem($"Entities");
				for(int i = 0; i < entities.Length; i++)
				{
					if(Target.EntityAdmin.IsSingletonEntity(entities[i]))
					{
						AddEntityItem(singletonsItem, entities[i]);
					}
					else
					{
						AddEntityItem(entitiesItem, entities[i]);
					}
				}

				entitiesItem.displayName += $" ({entitiesItem.children.Count})";
				singletonsItem.displayName += $" ({singletonsItem.children.Count})";

				adminItem.AddChild(systemsItem);
				adminItem.AddChild(singletonsItem);
				adminItem.AddChild(entitiesItem);
			}
			else
			{
				root.AddChild(CreateTreeViewItem("N/A"));
			}

			SetupDepthsFromParentsAndChildren(root);

			return root;

			void AddEntityItem(TreeViewItem parentTreeItem, Entity entity)
			{
				TreeViewItem entityTreeItem = CreateTreeViewItem(entity.UniqueIdentifier);
				parentTreeItem.AddChild(entityTreeItem);
				EntityComponent[] components = entity.GetAllComponents();
				_entityMap.Add(entityTreeItem.id, entity);
				for(int i = 0; i < components.Length; i++)
				{
					TreeViewItem componentTreeItem = CreateTreeViewItem(components[i].GetType().Name);
					entityTreeItem.AddChild(componentTreeItem);
					_componentMap.Add(componentTreeItem.id, components[i]);
				}
			}

			TreeViewItem CreateTreeViewItem(string name)
			{
				return new TreeViewItem
				{
					id = ++id,
					displayName = name
				};
			}
		}
	}
}
