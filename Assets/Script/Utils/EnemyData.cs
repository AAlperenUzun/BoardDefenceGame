using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "New Data", menuName = "EnemyData", order = 55)]
public class EnemyData : ScriptableObject
{
    public List<Enemy> enemies;
}

// DefenceItem sınıfınız
[System.Serializable]
public class Enemy
{
    public GridEnemyType enemyType;
    public float health;
    public float speed;
}

