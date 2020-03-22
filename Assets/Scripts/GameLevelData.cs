using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectMusic;

[CreateAssetMenu(fileName = "GameLevelData", menuName = "Scriptable Objects/Create Game Level Data Scriptable Object")]
public class GameLevelData : ScriptableObject
{
    public LevelData[] LevelDatas;
}

