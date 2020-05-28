using System.Collections.Generic;

public class FiniteStateMachine <T>  {
	private T owner;
	public FSMState<T> currentState;
	private FSMState<T> globalState;
	private Stack<FSMState<T>> previousStates = new Stack<FSMState<T>>();
	private bool reverting;

	public void Awake() {
		currentState = null;
		globalState = null;
	}

	public void Configure(T newOwner, FSMState<T> initialState, FSMState<T> initialGlobalState = null) {
		owner = newOwner;
		ChangeState(initialState);
		ChangeGlobalState(initialGlobalState);
	}

	public void Update() {
		globalState?.Execute(owner);
		currentState?.Execute(owner);
	}

	public void ChangeState(FSMState<T> newState) {
		if (currentState != null && !reverting) {
			previousStates.Push(currentState);
		}
		reverting = false;
		currentState?.Exit(owner);
		currentState = newState;
		currentState?.Enter(owner);
	}

	public void ChangeGlobalState(FSMState<T> newState) {
		globalState = newState;
		globalState?.Enter(owner);
	}

	public void RevertToPreviousState(int skips = 0) {
		for (int i = 0; i < skips; i++) {
			previousStates.Pop();
		}
		
		if (previousStates.Count > 0) {
			reverting = true;
			ChangeState(previousStates.Pop());
		}
	}
}