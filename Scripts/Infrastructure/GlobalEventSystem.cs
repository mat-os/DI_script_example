using Game.Scripts.Configs;
using Game.Scripts.Configs.Player;
using Game.Scripts.Configs.Vfx;
using Game.Scripts.Infrastructure.Services.Tutorial;
using Game.Scripts.LevelElements;
using Game.Scripts.LevelElements.Collisions;
using Game.Scripts.LevelElements.Player;
using Game.Scripts.LevelElements.Tutorial;
using RootMotion.Dynamics;
using UniRx;
using UnityEngine;

namespace Game.Scripts.Infrastructure
{
    public class GlobalEventSystem
    {
        // Создаём глобальный MessageBroker для передачи сообщений
        private static readonly IMessageBroker _messageBroker = new MessageBroker();

        // Публичный доступ к MessageBroker
        public static IMessageBroker Broker => _messageBroker;
    }
    
    public class StartPlayLevelEvent { }
    public class PlayerCarEnterBoosterZoneEvent { }

    public class PlayerCarHitWallEvent
    {
        public float CarSpeed { get; set; }
    }
    public class PlayerCollectTrophyEvent { }
    public class DisableTrophyOnLevelEvent { }
    public class PlayerEnterLevelCompleteTrigger { }
    public class PlayerEnterLevelWithJumpTutorialTrigger { }
    public class TapGameStartEvent { }
    public class BowlingPinFallEvent { }
    public class PlayerEnterSlowMotionTriggerEvent
    {
        public ESlowMotionType SlowMotionType { get; set; }
        public bool IsStart { get; set; }
    }

    public class PlayerExplodeEvent
    {
        public Vector3 ExplodePosition { get; set; }
        public float ExplosionForce { get; set; }
    }

    public class CarEnterSpeedSlowDownTrigger
    {
        public float SpeedBrakePercent { get; set; }
        public float CarDamage { get; set; }
        public Collision Collision { get; set; }
    }

    public class TapGameEndEvent
    {
        public float Result { get; set; }
    }

    public class ShowPraisePhraseEvent
    {
        public string PraisePhrase { get; set; }
        public int ScoreToAdd { get; set; }
    }
    public class PlayVfxEvent
    {
        public VfxEffectType VfxEffectType { get; set; }
        public Vector3 Position { get; set; }
    }    
    public class DestroyObjectEvent
    {
        public EScoreType ScoreType { get; set; }
        public Vector3 DestroyPosition { get; set; }
    }

    public class HitPeopleEvent
    {
        public EScoreType ScoreType { get; set; }
        public Vector3 DestroyPosition { get; set; }
        public Vector3 VfxPosition { get; set; }
    }

    public class CarNPCDestroyedEvent
    {
        public CarNPC CarNPC { get; set; }
    }
    public class TriggerTutorialEvent
    {
        public ETutorialStep TutorialStep { get; set; }
    }
    //Когда в мишень попали
    public class TargetHitEvent
    {
        public TargetHitView TargetHitView;
        public Vector3 HitPosition { get; set; }
    }
    //Когда сбиваем обьекты миссии
    public class MissionObjectDestroyEvent
    {
        public Vector3 HitPosition { get; set; }
    }    
    public class BrokeBoneEvent
    {
        public int TotalBrokenBones { get; set; }
    }    
    public class PlayerBodyPartTakeDamageEvent
    {
        public Muscle.Group MuscleGroup;
        public EExtendedMuscleGroup EExtendedMuscleGroup{ get; set; }
        public float Damage{ get; set; }
        public float TotalDamageOnBodyPart{ get; set; }
    }
    public class ChangeCameraOnDialogueEvent
    {
        public ECameraType CameraType { get; set; }
    }    
}