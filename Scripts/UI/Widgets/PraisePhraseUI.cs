using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts.Infrastructure;
using Game.Scripts.Infrastructure.Bootstrapper;
using Game.Scripts.Infrastructure.LevelStateMachin;
using Game.Scripts.Infrastructure.LevelStateMachin.States;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.Widgets
{
    public class PraisePhraseUI : MonoBehaviour, IDisposable
    {
        [SerializeField] private Transform _praisePhraseRoot;
        [SerializeField] private PraiseText _praiseTextPrefab;

        [Header("Message Queue Settings")] [SerializeField]
        private int _maxSimultaneousMessages = 3;

        [SerializeField] private float _messageDelay = 0.5f; // Пауза между сообщениями

        private Queue<ShowPraisePhraseEvent> _messageQueue = new Queue<ShowPraisePhraseEvent>();
        private List<PraiseText> _activeMessages = new List<PraiseText>();

        private Coroutine _displayCoroutine;
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        
        private LevelStateMachine _levelStateMachine;

        [Inject]
        public void Construct(LevelStateMachine levelStateMachine)
        {
            _levelStateMachine = levelStateMachine;
        }
        public void Subscribe()
        {
            GlobalEventSystem.Broker.Receive<ShowPraisePhraseEvent>()
                .Subscribe(EnqueuePraiseMessage)
                .AddTo(_disposable);
        }

        private void Awake()
        {
            foreach (Transform child in _praisePhraseRoot)
            {
                Destroy(child.gameObject);
            }
        }

        private void EnqueuePraiseMessage(ShowPraisePhraseEvent phraseEvent)
        {
            if (_levelStateMachine.CurrentState.GetType() == typeof(PlayLevelState))
            {
                Debug.Log("EnqueuePraiseMessage");
                if (_activeMessages.Count < _maxSimultaneousMessages)
                {
                    ShowPraiseMessage(phraseEvent);
                }
                else
                {
                    _messageQueue.Enqueue(phraseEvent);
                }
            }
        }

        private void ShowPraiseMessage(ShowPraisePhraseEvent phraseEvent)
        {
            if (_levelStateMachine.CurrentState.GetType() != typeof(PlayLevelState) || !gameObject.activeInHierarchy)
                return;

            Debug.Log("ShowPraiseMessage");

            AdjustExistingMessages();

            PraiseText praiseText = Instantiate(_praiseTextPrefab, _praisePhraseRoot);
            if (praiseText == null) return;

            praiseText.Initialize(phraseEvent.PraisePhrase, phraseEvent.ScoreToAdd, OnMessageDestroyed);

            _activeMessages.Add(praiseText);

            if (_displayCoroutine == null)
            {
                _displayCoroutine = CoroutineRunner.Instance.StartCoroutine(DisplayMessagesWithDelay());
            }
        }

        private IEnumerator DisplayMessagesWithDelay()
        {
            while (_messageQueue.Count > 0)
            {
                yield return new WaitForSeconds(_messageDelay);

                if (this == null || !gameObject.activeInHierarchy || _messageQueue.Count == 0)
                {
                    yield break; // Завершаем корутину, если объект уничтожен или неактивен
                }

                if (_activeMessages.Count < _maxSimultaneousMessages)
                {
                    ShowPraiseMessage(_messageQueue.Dequeue());
                }
            }

            _displayCoroutine = null;
        }

        private void OnMessageDestroyed(PraiseText message)
        {
            // Удаляем сообщение из активных
            _activeMessages.Remove(message);

            // Проверяем, есть ли сообщения в очереди
            if (_messageQueue.Count > 0 && _activeMessages.Count < _maxSimultaneousMessages)
            {
                ShowPraiseMessage(_messageQueue.Dequeue());
            }
        }

        private void AdjustExistingMessages()
        {
            foreach (PraiseText existingText in _activeMessages)
            {
                existingText.SetInactiveStyle();
            }
        }

        public void Clear()
        {
            // Остановка текущей корутины
            if (_displayCoroutine != null)
            {
                CoroutineRunner.Instance.StopCoroutine(_displayCoroutine);
                _displayCoroutine = null;
            }

            // Уничтожение активных сообщений
            foreach (PraiseText message in _activeMessages)
            {
                if (message != null)
                {
                    message.Clear();
                }
            }

            _activeMessages.Clear();

            // Очистка очереди сообщений
            _messageQueue.Clear();
        }

        public void Dispose()
        {
            // Убиваем подписки
            _disposable?.Dispose();

            // Останавливаем корутину
            if (_displayCoroutine != null)
            {
                CoroutineRunner.Instance.StopCoroutine(_displayCoroutine);
                _displayCoroutine = null;
            }

            // Очищаем все активные сообщения и очередь
            Clear();
        }
    }
}
