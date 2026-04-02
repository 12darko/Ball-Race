using System.Collections.Generic;
using _Main.Scripts.Multiplayer.Multiplayer.Customize;
using UnityEngine;

namespace Main.Scripts.Player.Database
{
    [CreateAssetMenu(fileName = "CosmeticsDatabase", menuName = "Game/Cosmetics Database")]   
    public class CosmeticDatabase : ScriptableObject
    {
        [Header("Market Sırasına Göre Toplar")]
        public List<CustomizeItem> allBalls;

        [Header("Market Sırasına Göre Şapkalar")]
        public List<CustomizeItem> allHats;

        [Header("Market Sırasına Göre Yüzler")]
        public List<CustomizeItem> allFaces;
    
        // Yardımcı Fonksiyon: Index güvenli mi?
        public bool IsBallIndexValid(int index)
        {
            return index >= 0 && index < allBalls.Count;
        }
    }
}