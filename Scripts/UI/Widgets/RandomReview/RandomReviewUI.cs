using System;
using System.Collections;
using Configs;
using DG.Tweening;
using Game.Scripts.Infrastructure.Bootstrapper;
using TMPro;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.Widgets.RandomReview
{
    public class RandomReviewUI : MonoBehaviour
    {
        public Action OnPlayerCloseIt;

        [Header("UI Elements")]
        [SerializeField] private RandomReviewLineUI[] _randomReviewLineUI;
        [SerializeField] private TMP_Text _rewardText;
        [SerializeField] private TMP_Text _levelName;
        [SerializeField]private float _delaySeconds;
        [SerializeField]private CanvasGroup _canvasGroup;
        [SerializeField]private GameObject _tapToContinueText;

        private RandomReviewService _randomReviewService;

        private Coroutine _displayCoroutine;

        [Inject]
        public void Counstruct(RandomReviewService randomReviewService)
        {
            _randomReviewService = randomReviewService;
        }

        public void OnOpenStart(string levelName)
        {
            _levelName.text = levelName;
            
            // Если корутина уже работает, остановим её перед запуском новой
            if (_displayCoroutine != null)
            {
                CoroutineRunner.Instance.StopCoroutine(_displayCoroutine);
            }

            // Запускаем корутину показа строк
            _displayCoroutine = CoroutineRunner.Instance.StartCoroutine(DisplayReviews());
        }

        private IEnumerator DisplayReviews()
        {
            _tapToContinueText.transform.localScale = Vector3.zero;
            
            ShowEarningText(0, 500);
            
            // Очищаем все строки перед началом
            foreach (var line in _randomReviewLineUI)
            {
                line.Clear();
            }

            // Отображаем строки с задержкой
            for (int i = 0; i < _randomReviewLineUI.Length; i++)
            {
                Review randomReview = _randomReviewService.GetRandomReview();
                _randomReviewLineUI[i].DisplayReview(randomReview);

                // Задержка перед показом следующей строки
                yield return new WaitForSeconds(_delaySeconds); // Время задержки между строками
            }

            _tapToContinueText.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            CloseUI();
            _displayCoroutine = null; // Завершаем корутину
        }
        private void ShowEarningText(int startAmount, int targetAmount)
        {
            DOVirtual.Int(startAmount, targetAmount, 2f, value =>
            {
                _rewardText.text = "+" + value.ToString();
            }).SetEase(Ease.InOutSine);
        }
        public void CloseUI()
        {
            OnPlayerCloseIt?.Invoke();
            _canvasGroup.DOFade(0, 0.5f).OnComplete(() =>
            {
                Deactivate();
                Clear();
            });
        }
        public void Clear()
        {
            // Очищаем все строки и останавливаем корутину показа
            if (_displayCoroutine != null)
            {
                CoroutineRunner.Instance.StopCoroutine(_displayCoroutine);
                _displayCoroutine = null;
            }

            foreach (var line in _randomReviewLineUI)
            {
                line.Clear();
            }
        }

        public void Activate()
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        public void Deactivate()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }
    }
}