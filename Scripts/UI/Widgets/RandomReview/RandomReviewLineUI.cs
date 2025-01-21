using System.Collections;
using Configs;
using DG.Tweening;
using Game.Scripts.Infrastructure.Bootstrapper;
using TMPro;
using UnityEngine;

namespace Game.Scripts.UI.Widgets.RandomReview
{
    public class RandomReviewLineUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TMP_Text magazineNameText;
        [SerializeField] private TMP_Text reviewText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private Transform scoreTransform;

        [Header("Typing Settings")]
        [SerializeField] private float typingSpeed = 0.05f; // Скорость печати текста

        private Coroutine _currentCoroutine;

        public void DisplayReview(Review review)
        {
            // Останавливаем текущую корутину, если она ещё выполняется
            if (_currentCoroutine != null)
            {
                CoroutineRunner.Instance.StopCoroutine(_currentCoroutine);
            }

            // Запускаем новую корутину для отображения рецензии
            _currentCoroutine = CoroutineRunner.Instance.StartCoroutine(TypeReview(review));
        }

        private IEnumerator TypeReview(Review review)
        {
            // Устанавливаем статический текст для названия журнала
            magazineNameText.text = review.MagazineName;

            // Плавно печатаем текст рецензии
            reviewText.text = "";
            foreach (var letter in review.Text)
            {
                reviewText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }

            // После завершения текста запускаем анимацию оценки
            PlayScoreAnimation(review.Score);

            _currentCoroutine = null; // Завершаем текущую корутину
        }

        private void PlayScoreAnimation(string score)
        {
            scoreText.text = score;

            // Начальное состояние для анимации
            scoreText.alpha = 0f;
            scoreTransform.localScale = Vector3.zero;

            // Анимация скейла и фейда
            var sequence = DOTween.Sequence();
            sequence.Append(scoreTransform.DOScale(Vector3.one, 0.5f)
                .SetEase(Ease.OutBack)); // Скейл с эффектом "отскока"
            sequence.Join(scoreText.DOFade(1f, 0.5f)); // Фейд
            sequence.OnComplete(() => Debug.Log("Score animation completed!")); // Событие после анимации
        }

        public void Clear()
        {
            // Очищаем текущий текст и анимацию
            if (_currentCoroutine != null)
            {
                CoroutineRunner.Instance.StopCoroutine(_currentCoroutine);
                _currentCoroutine = null;
            }

            magazineNameText.text = "";
            reviewText.text = "";
            scoreText.text = "";

            // Сбрасываем состояние элементов
            scoreText.alpha = 0f;
            scoreTransform.localScale = Vector3.zero;
        }
    }
}
