using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardConfig", menuName = "Configs/new Card Config")]
public class CardConfig : ScriptableObject
{
    [SerializeField]
    public Card Card;
}
