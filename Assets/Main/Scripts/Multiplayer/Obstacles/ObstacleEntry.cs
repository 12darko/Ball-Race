using _Main.Scripts.Multiplayer.Multiplayer.Obstacles;
using UnityEngine;

[System.Serializable]
public class ObstacleEntry
{
    [Tooltip("Obstacle objesi — otomatik doldurulur, elle de değiştirebilirsin.")]
    public Obstacle obstacle;

    [Tooltip("Bu obstacle hiç spawn olmasın mı?")]
    public bool enabled = true;

    [Tooltip("Spawn şansı (0 = hiç, 100 = her zaman).")]
    [Range(0, 100)]
    public int spawnChance = 100;
}