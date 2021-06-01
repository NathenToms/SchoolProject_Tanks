using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotThing : MonoBehaviour
{
    public enum shotFrom { Player, Enemy, Environment };
   [HideInInspector] public shotFrom whoSent = shotFrom.Enemy;
   
}
