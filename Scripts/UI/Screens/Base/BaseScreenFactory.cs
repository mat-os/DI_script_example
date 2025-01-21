using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.Screens.Base
{
    public class BaseScreenFactory : IFactory<Transform, Object, BaseScreen>
    {
        [Inject] private readonly DiContainer _container;

        public BaseScreen Create(Transform root, Object prefab) =>
            _container.InstantiatePrefabForComponent<BaseScreen>(prefab, root);
    }
}