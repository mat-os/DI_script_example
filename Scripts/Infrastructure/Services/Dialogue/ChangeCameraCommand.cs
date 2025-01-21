using UnityEngine;
using Fungus;
using Game.Scripts.Infrastructure;
using Game.Scripts.LevelElements;

[CommandInfo("Other",
    "ChangeCameraCommand",
    "Custom Change Camera Command.")]
[AddComponentMenu("")]
public class ChangeCameraCommand : Command
{
    public ECameraType CameraType;
    public override void OnEnter()
    {
        GlobalEventSystem.Broker.Publish(new ChangeCameraOnDialogueEvent { CameraType = CameraType });
        Continue();
    }
}