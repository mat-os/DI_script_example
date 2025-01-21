using System.Collections.Generic;
using Configs;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.Widgets.RandomReview
{
    public class RandomReviewService
    {
        private readonly List<Review> _shuffledReviews;
        private int _currentIndex;

        [Inject]
        public RandomReviewService(GameConfig gameConfig)
        {
            // Берём конфигурацию отзывов из GameConfig
            _shuffledReviews = new List<Review>(gameConfig.ReviewConfig.Reviews);
            
            // Перемешиваем список при инициализации
            ShuffleReviews();
        }

        /// <summary>
        /// Получить случайный отзыв без повторений.
        /// Когда список заканчивается, он перемешивается и используется снова.
        /// </summary>
        public Review GetRandomReview()
        {
            if (_shuffledReviews == null || _shuffledReviews.Count == 0)
            {
                Debug.LogWarning("No reviews available!");
                return null;
            }

            // Получаем текущий отзыв
            Review review = _shuffledReviews[_currentIndex];
            
            // Увеличиваем индекс
            _currentIndex++;

            // Если список закончился, перемешиваем его снова и сбрасываем индекс
            if (_currentIndex >= _shuffledReviews.Count)
            {
                ShuffleReviews();
                _currentIndex = 0;
            }

            return review;
        }

        /// <summary>
        /// Метод для перемешивания списка с использованием алгоритма Фишера-Йетса
        /// </summary>
        private void ShuffleReviews()
        {
            for (int i = _shuffledReviews.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                // Обмениваем элементы местами
                ( _shuffledReviews[i], _shuffledReviews[randomIndex] ) = ( _shuffledReviews[randomIndex], _shuffledReviews[i] );
            }

            Debug.Log("Reviews shuffled!");
        }
    }
}