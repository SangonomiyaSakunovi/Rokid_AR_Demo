using System;
using System.Collections.Generic;
using UnityEngine;

namespace SangoUtils.HTCViveTrackerHelpers
{
    internal abstract class IState
    {
        protected StateMachine Machine { get; set; }

        public abstract void Enter();
        public abstract void Update();
        public abstract void Exit();
    }

    /// <summary>
    /// A small statemachine, only use for this package.
    /// </summary>
    internal abstract class StateMachine
    {
        protected Dictionary<string, object> BlackBoard = new();

        private Dictionary<int, IState> _states = new();
        private IState _currentState;

        public IState State => _currentState;

        public void Enter<T>() where T : IState => Enter(typeof(T));
        public void Remove<T>() where T : IState => Remove(typeof(T));

        public void Add(IState state)
        {
            _states.Add(state.GetType().GetHashCode(), state);
        }

        public void Enter(Type type)
        {
            if (_currentState?.GetType().GetHashCode() == type.GetHashCode())
                return;

            if (_states.TryGetValue(type.GetHashCode(), out var state))
            {
                _currentState?.Exit();
                _currentState = state;
                _currentState.Enter();
            }
            else
            {
                Debug.LogError($"[Sango] StateMachine has no state [{nameof(state)}].");
            }
        }

        public void Update()
        {
            _currentState?.Update();
        }

        public void Exit()
        {
            _currentState.Exit();
            _currentState = null;
        }

        public void Remove(Type type)
        {
            if (_states.ContainsKey(type.GetHashCode()))
                _states.Remove(type.GetHashCode());
        }

        public void Clear()
        {
            _currentState?.Exit();
            _currentState = null;
            _states.Clear();
        }
    }
}
