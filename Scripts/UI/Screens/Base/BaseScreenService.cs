using System;
using System.Collections.Generic;
using Game.Scripts.Utils.Debug;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.Screens.Base
{
    public abstract class BaseScreenService<T> : MonoBehaviour where T : BaseScreen
    {
        [SerializeField] private T[] _prefabs;

        private readonly Dictionary<Type, T> _screenPrefabs = new();
        private readonly Dictionary<Type, T> _screens = new();

        private readonly List<Type> _activeScreens = new();

        private BaseScreen.Factory _screenFactory;

        [Inject]
        private void Construct(BaseScreen.Factory screenFactory)
        {
            _screenFactory = screenFactory;
        }

        private void Awake()
        {
            foreach (T prefab in _prefabs)
            {
                Type type = prefab.GetType();
                _screenPrefabs.Add(type, prefab);
            }
        }

#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
        public T GetScreen<T>() where T : BaseScreen
#pragma warning restore CS0693 // Type parameter has the same name as the type parameter from outer type
        {
            Type type = typeof(T);
            if (_screens.TryGetValue(type, out var screen))
                return screen as T;

            var currentScreen = CreateScreen(type);
            _screens.Add(type, currentScreen);

            return currentScreen as T;
        }

#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
        public T ShowScreen<T>() where T : BaseScreen
#pragma warning restore CS0693 // Type parameter has the same name as the type parameter from outer type
        {
            T screen = GetScreen<T>();
            if (screen == default)
            {
                CustomDebugLog.LogError("No contains screen");
                return default;
            }

            _activeScreens.Add(screen.GetType());

            OpenScreen(screen);

            return screen;
        }

        private T CreateScreen(Type type)
        {
            T prefab = _screenPrefabs[type];
            T screen = _screenFactory.Create(transform, prefab) as T;
            Transform tr = screen.transform;
            tr.position = transform.position;
            tr.rotation = Quaternion.identity;
            tr.localScale = Vector3.one;
            screen.OnCreate();

            return screen;
        }

#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
        public void CloseScreen<T>() where T : BaseScreen
#pragma warning restore CS0693 // Type parameter has the same name as the type parameter from outer type
        {
            Type type = typeof(T);
            if (!_screens.ContainsKey(type))
            {
                CustomDebugLog.LogError("No contains screen");
                return;
            }

            _activeScreens.Remove(type);

            CloseScreen(_screens[type]);
        }

        public void CloseAll()
        {
            foreach (Type type in _activeScreens)
                CloseScreen(_screens[type]);

            _activeScreens.Clear();
        }

#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
        private async void OpenScreen<T>(T screen) where T : BaseScreen
#pragma warning restore CS0693 // Type parameter has the same name as the type parameter from outer type
        {
            await screen.OnOpenStart();
            await screen.PlayOpenAnimation();
            await screen.OnOpenComplete();
        }

        private async void CloseScreen(T screen)
        {
            await screen.OnCloseStart();
            await screen.PlayCloseAnimation();
            await screen.OnCloseComplete();
        }
    }
}