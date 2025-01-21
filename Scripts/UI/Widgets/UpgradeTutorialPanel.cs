using System;
using UnityEngine;

namespace Game.Scripts.UI.Widgets
{
    public class UpgradeTutorialPanel : MonoBehaviour
    {
        [SerializeField] private UpgradeTutorialStep[] _upgradeTutorialSteps; // Массив шагов туториала
        [SerializeField] private Transform _hand; // Массив шагов туториала

        private int _currentStep = -1; // Текущий шаг (начинаем с -1)
        private bool _isWaitingForUpgrade; // Флаг ожидания выполнения действия игроком

        private void Start()
        {
            ShowNextStep(); // Запускаем туториал с первого шага
        }

        private void ShowNextStep()
        {
            // Скрыть предыдущий текст, если мы не на первом шаге
            if (_currentStep >= 0 && _currentStep < _upgradeTutorialSteps.Length)
            {
                _upgradeTutorialSteps[_currentStep].Text.SetActive(false);
                _upgradeTutorialSteps[_currentStep].Mask.SetActive(false);
            }

            _currentStep++; // Переходим к следующему шагу

            if (_currentStep < _upgradeTutorialSteps.Length)
            {
                // Активируем новый шаг
                var step = _upgradeTutorialSteps[_currentStep];

                // Включаем текст текущего шага
                step.Text.SetActive(true);
                step.Mask.SetActive(true);
                _hand.transform.position = step.HandPosition.position;

                // Подписываемся на событие завершения покупки апгрейда
                _isWaitingForUpgrade = true;
                step.UpgradeView.OnCoinUpgradeClick += HandleUpgradePurchased;
            }
            else
            {
                // Все шаги завершены, отключаем панель туториала
                EndTutorial();
            }
        }

        private void HandleUpgradePurchased()
        {
            if (_isWaitingForUpgrade)
            {
                _isWaitingForUpgrade = false;

                // Отписываемся от события текущего апгрейда
                var step = _upgradeTutorialSteps[_currentStep];
                step.UpgradeView.OnCoinUpgradeClick -= HandleUpgradePurchased;

                // Переходим к следующему шагу
                ShowNextStep();
            }
        }

        private void EndTutorial()
        {
            gameObject.SetActive(false);
            Debug.Log("Tutorial completed!");
        }
    }

    [Serializable]
    public class UpgradeTutorialStep
    {
        public UpgradeView UpgradeView;
        public GameObject Text;
        public GameObject Mask;
        public Transform HandPosition;
    }
}