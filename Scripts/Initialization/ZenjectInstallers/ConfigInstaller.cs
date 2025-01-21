using Configs;
using Game.Configs;
using Game.Scripts.Configs;
using Game.Scripts.Customization;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Game.Scripts.Initialization.ZenjectInstallers
{
    public class ConfigInstaller : MonoInstaller
    {
        [SerializeField, Required]private GameConfig _gameConfig;
        [SerializeField, Required]private UIConfig _uiConfig;
        [SerializeField, Required]private EffectsConfig _effectsConfig;
        [SerializeField, Required]private PrefabRepository _prefabRepository;
        [SerializeField, Required]private LevelsRepository _levelsRepository;
        [SerializeField, Required]private VibrationConfig _vibrationConfig;
        [SerializeField, Required]private MissionRepository _missionRepository;
        [SerializeField, Required]private CustomizationShopConfig _customizationShopConfig;

        
        public override void InstallBindings()
        {
            Container.Bind<GameConfig>().FromInstance(_gameConfig).AsSingle();
            Container.Bind<UIConfig>().FromInstance(_uiConfig).AsSingle();
            Container.Bind<EffectsConfig>().FromInstance(_effectsConfig).AsSingle();
            Container.Bind<PrefabRepository>().FromInstance(_prefabRepository).AsSingle();
            Container.Bind<LevelsRepository>().FromInstance(_levelsRepository).AsSingle();
            Container.Bind<VibrationConfig>().FromInstance(_vibrationConfig).AsSingle();
            Container.Bind<MissionRepository>().FromInstance(_missionRepository).AsSingle();
            Container.Bind<CustomizationShopConfig>().FromInstance(_customizationShopConfig).AsSingle();
        }
    }
}