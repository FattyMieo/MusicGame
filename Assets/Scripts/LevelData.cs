﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectMusic;

[CreateAssetMenu(fileName = "NewLevel", menuName = "Scriptable Objects/Create New Level")]
public class LevelData : ScriptableObject
{
    public int Level = 0;
    public MusicNotePair[] Pairs;
    public NoteType[] Sequence;
    public NoteType[] Decoys;
}

[CreateAssetMenu(fileName = "GameLevelData", menuName = "Scriptable Objects/Create Game Level Data Scriptable Object")]
public class GameLevelData : ScriptableObject
{
    public LevelData[] LevelDatas;
}
