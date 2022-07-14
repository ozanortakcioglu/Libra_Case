using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level Parameters", fileName = "Levelx")]
public class LevelInfo : ScriptableObject
{
    public int width;
    public int height;
    [Space(10)]
    public Vector2Int[] brickPos;


}
