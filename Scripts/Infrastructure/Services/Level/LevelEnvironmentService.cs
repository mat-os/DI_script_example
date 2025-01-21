using Configs;
using UnityEngine;

namespace Game.Scripts.Configs.Level
{
    public class LevelEnvironmentService
    {
        public LevelEnvironmentService()
        {

        }

        public void SetupEnvironment(EnvironmentConfig environmentConfig)
        {
            RenderSettings.skybox = environmentConfig.SkyboxMaterial;
            ChangeFog(environmentConfig.FogSettings);
            SetupLighting(environmentConfig.GradientLightingSettings);
        }

        private void ChangeFog(FogSettings fogSettings)
        {
            RenderSettings.fog = fogSettings.IsFogEnabled;
            RenderSettings.fogMode = FogMode.Linear;
            if (RenderSettings.fog)
            {
                RenderSettings.fogColor = fogSettings.FogColor;
                RenderSettings.fogStartDistance = fogSettings.FogStart;
                RenderSettings.fogEndDistance = fogSettings.FogEnd;
            }
        }
        private void SetupLighting(GradientLightingSettings lightingSettings)
        {
            RenderSettings.ambientSkyColor = lightingSettings.SkyColor;
            RenderSettings.ambientEquatorColor = lightingSettings.EquatorColor;
            RenderSettings.ambientGroundColor = lightingSettings.GroundColor;
        }
    }
}