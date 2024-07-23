using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseEnemy : BaseClass
{
    public enum Type
    {
        GRASS,
        FIRE,
        WATER,
        ELECTRIC
    }

    public enum Rarity
    {
        COMMON,
        UNCOMMON,
        RARE,
        SUPERRARE
    }

    public Type EnemyType; // Type of the enemy (e.g., Grass, Fire, Water, Electric)

    public Rarity rarity; // Rarity level of the enemy (e.g., Common, Uncommon, Rare, Super Rare
}