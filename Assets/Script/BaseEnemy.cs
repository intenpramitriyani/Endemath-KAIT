using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseEnemy
{
    public new string name; // Name of the enemy

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

    public Rarity rarity; // Rarity level of the enemy (e.g., Common, Uncommon, Rare, Super Rare)

    // Base and current health points (HP)
    public float baseHP;
    public float curHP;

    // Base and current mana points (MP) - if applicable in your game
    public float baseMP;
    public float curMP;

    // Base and current attack points (ATK)
    public float baseATK;
    public float curATK;

    // Base and current defense points (DEF)
    public float baseDEF;
    public float curDEF;
}
