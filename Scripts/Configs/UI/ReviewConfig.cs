using System.Collections.Generic;
using Game.Scripts.Core.CurrencyService;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = nameof(ReviewConfig), menuName = "Configs/" + nameof(ReviewConfig))]
    public class ReviewConfig : ScriptableObject
    {
        public List<Review> Reviews = new List<Review>();
    }
    [System.Serializable]
    public class Review
    {
        public string MagazineName;
        public string Text;
        public string Score;
        [Header("Reward")]
        public int Reward;
        public ECurrencyType CurrencyType;
    }
}