using UnityEngine;

namespace Game.Scripts.UI.Widgets
{
    public abstract class ProgressBar : MonoBehaviour, IProgressBar
    {
        public abstract void SetProgress(float currentLevel, float maxLevel);
    }
}