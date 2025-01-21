using Configs;
using UnityEngine;

namespace Game.Scripts.LevelElements.Car.CarMovement
{
    public interface ICarMovement
    {
        void Move(Vector3 inputVector);           
        void Rotate(Vector3 inputVector);           
        float GetSpeed();
        float GetHorizontalInertia();
        float GetMaxSpeed();
        void SetSpeedMultiplier(float multiplier);
        void UpgradeCarParameters(CarUpgradeStep carUpgradeInfo);
        void ApplyCollisionSlowdown(float percent);
        void ApplyImmediateSpeedBoost(float immediateBoostSpeedAmount);
    }
}