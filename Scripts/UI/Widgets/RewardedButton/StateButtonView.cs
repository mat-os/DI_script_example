using System;
using Game.RateUs.Scripts.UI.Common;
using UnityEngine;
using UnityEngine.UI;

public class StateButtonView : MonoBehaviour
{
    [Serializable]
    public class RewardedButtonState
    {
        [SerializeField] private RewardedButtonStateType _type;
        [SerializeField] private Transform _stateRoot;

        public RewardedButtonStateType Type => _type;
        public Transform StateRoot => _stateRoot;
    }

    [SerializeField] private RewardedButtonState[] _states;
    [SerializeField] private Button _button;
    private RewardedButtonStateType _currentState;

    public void SetState(RewardedButtonStateType type)
    {
        if (_currentState == RewardedButtonStateType.Used)
            return;

        _currentState = type;
        foreach (var state in _states)
        {
            state.StateRoot.gameObject.SetActive(state.Type == type);
        }

        _button.interactable = type == RewardedButtonStateType.Active;
    }

    public void ResetState()
    {
        SetState(RewardedButtonStateType.Inactive);
    }
}