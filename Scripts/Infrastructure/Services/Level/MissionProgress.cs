using Configs;

namespace Game.Scripts.Infrastructure.Services
{
    public class MissionProgress
    {
        public LevelMission Mission { get; private set; }
        public float CurrentValue { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsActive { get; set; }

        public MissionProgress(LevelMission mission)
        {
            Mission = mission;
            CurrentValue = 0f;
        }
    }
}