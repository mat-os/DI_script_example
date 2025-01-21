using DG.Tweening;
using Game.Scripts.Infrastructure.Services.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Widgets
{
    public class TapGameUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Image _progressBar; // Прогресс-бар
        [SerializeField] private TMP_Text _powerText; // Процент силы
        [SerializeField] private GameObject _tapToFillUI; // Основной UI мини-игры
        [SerializeField] private RectTransform _arrowTransform; // Стрелка-спидометр

        [Header("Arrow Settings")]
        [SerializeField] private float _minRotationZ = 90f; // Угол при Fill = 0
        [SerializeField] private float _maxRotationZ = -90f; // Угол при Fill = 1
        
        private PlayerLaunchTapGameService _playerLaunchTapGameService;

        [Inject]
        public void Construct(PlayerLaunchTapGameService tapGameService)
        {
            _playerLaunchTapGameService = tapGameService;
            
            _playerLaunchTapGameService.OnTapGameStart += ShowUI;
            _playerLaunchTapGameService.OnProgressChanged += UpdateUI;
            _playerLaunchTapGameService.OnTapGameEnd += HandleTapGameEnd;
        }

        public void OnOpenStart()
        {
            _tapToFillUI.SetActive(false);
        }

        private void OnDestroy()
        {
            _playerLaunchTapGameService.OnTapGameStart -= ShowUI;
            _playerLaunchTapGameService.OnProgressChanged -= UpdateUI;
            _playerLaunchTapGameService.OnTapGameEnd -= HandleTapGameEnd;
        }

        private void ShowUI()
        {
            _tapToFillUI.SetActive(true);
        }

        private void UpdateUI(float progress)
        {
            // 1️⃣ Обновляем прогресс-бар и текст
            _progressBar.fillAmount = progress;
            _powerText.text = Mathf.RoundToInt(progress * 100) + "%";

            // 2️⃣ Обновляем поворот стрелки
            UpdateArrowRotation(progress);
        }

        private void UpdateArrowRotation(float progress)
        {
            // Интерполируем угол поворота от minRotationZ до maxRotationZ
            float rotationZ = Mathf.Lerp(_minRotationZ, _maxRotationZ, progress);
            _arrowTransform.localRotation = Quaternion.Euler(0, 0, rotationZ);
        }

        private void HandleTapGameEnd(float finalProgress)
        {
            _tapToFillUI.SetActive(false);
            if (finalProgress >= 0.9f)
            {
                ShowSuccessEffect();
            }
            else
            {
                ShowFailureEffect();
            }
        }

        private void ShowSuccessEffect()
        {
            _tapToFillUI.SetActive(false);

            DOTween.Sequence()
                .AppendCallback(() => Debug.Log("🎉 MAXIMUM SPEED!"))
                .AppendInterval(0.5f)
                .AppendCallback(() => Debug.Log("🚀 Full Speed Launch!"));
        }

        private void ShowFailureEffect()
        {
            _tapToFillUI.SetActive(false);

            Debug.Log("❌ Nice Try! Partial Speed Launch.");
        }
    }
}