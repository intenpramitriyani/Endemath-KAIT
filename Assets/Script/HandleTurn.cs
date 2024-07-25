using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YourGameNamespace;


[System.Serializable]
public class HandleTurn
{
    public string Attacker; // Name of the attacker
    public string Type;
    public GameObject AttackersGameObject; // GameObject of the attacker
    public GameObject AttackersTarget; // GameObject of the target
}