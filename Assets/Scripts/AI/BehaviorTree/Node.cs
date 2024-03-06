using System.Collections.Generic;

namespace Quinn.AI
{
	public abstract class Node
	{
		public List<Service> Services { get; set; } = new();
		public List<Conditional> Conditionals { get; set; } = new();

		public string Name
		{
			get
			{
				var split = GetType().Name.Split('.');
				string type = split[^1];

				if (string.IsNullOrWhiteSpace(_name))
				{
					return type;
				}

				return $"{_name} ({type})";
			}

			set => _name = value;
		}

		public string RawName => _name;

		public Composite Parent => _parent;
		public Tree Tree => _tree != null ? _tree : _parent.Tree;
		public Enemy Agent => Tree.Agent;

		private Composite _parent;
		private Tree _tree;
		private string _name;

		public virtual void Enter()
		{
			OnEnter();

			foreach (var service in Services)
			{
				service.Enter();
			}

			foreach (var condition in Conditionals)
			{
				condition.Enter();
			}
		}

		public virtual Status Update()
		{
			return OnUpdate();
		}

		public virtual void Exit()
		{
			OnExit();

			foreach (var service in Services)
			{
				service.Exit();
			}

			foreach (var condition in Conditionals)
			{
				condition.Exit();
			}
		}

		public bool Evaluate()
		{
			foreach (var conditional in Conditionals)
			{
				if (!conditional.Evaluate())
				{
					return false;
				}
			}

			return true;
		}

		public void ParentEnter()
		{
			OnParentEnter();

			foreach (var service in Services)
			{
				service.ParentEnter();
			}

			foreach (var condition in Conditionals)
			{
				condition.ParentEnter();
			}
		}

		public void ParentExit()
		{
			OnParentExit();

			foreach (var service in Services)
			{
				service.ParentExit();
			}

			foreach (var condition in Conditionals)
			{
				condition.ParentExit();
			}
		}

		public void SetParent(Composite parent)
		{
			_parent = parent;

			foreach (var service in Services)
			{
				service.SetParent(parent);
			}

			foreach (var conditional in Conditionals)
			{
				conditional.SetParent(parent);
			}
		}
		public void SetTree(Tree tree)
		{
			_tree = tree;
		}

		protected virtual void OnEnter() { }
		protected virtual Status OnUpdate() => Status.Success;
		protected virtual void OnExit() { }

		protected virtual void OnParentEnter() { }
		protected virtual void OnParentExit() { }
	}
}
