using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BasePlayer
{
    public new string name; // Name of the player

    public float baseHP; // Base health points (HP)
    public float curHP; // Current health points (HP)

    public float baseMP; // Base mana points (MP)
    public float curMP; // Current mana points (MP)

    public int stamina; // Stamina attribute
    public int intellect; // Intellect attribute
    public int dexterity; // Dexterity attribute
    public int agility; // Agility attribute
}
