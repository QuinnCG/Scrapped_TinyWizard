using System.Linq;

namespace Quinn.AI.States
{
	public class RandomState : State
	{
		private readonly (State state, float weight)[] _states;
		private State _selected;

		public RandomState(params State[] states)
		{
			_states = states.Select(x => (x, 1f)).ToArray();
		}
		public RandomState(params (State state, float weight)[] states)
		{
			_states = states;
		}

		protected override void OnEnter()
		{
			foreach (var (state, weight) in _states)
			{
				float value = UnityEngine.Random.value;

				if (weight <= value)
				{
					_selected = state;
					return;
				}
			}

			_selected = _states[UnityEngine.Random.Range(0, _states.Length)].state;
		}

		protected override bool OnUpdate()
		{
			return _selected.Update();
		}

		protected override void OnExit()
		{
			_selected.Exit();
		}
	}
}
