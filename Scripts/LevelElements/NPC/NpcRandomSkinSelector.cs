using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.LevelElements.NPC
{
    public class NpcRandomSkinSelector : MonoBehaviour
    {
        [SerializeField] private GameObject[] _skins;
        private void Start()
        {
            var random = Random.Range(0, _skins.Length);
            for (int i = 0; i < _skins.Length; i++)
            {
                _skins[i].SetActive(i == random);
            }
        }
    }
}