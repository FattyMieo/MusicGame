using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectMusic;

public class KeyComponent : MonoBehaviour
{
    public NoteType Type = NoteType.Unused;
    
    public void OnPressKey()
    {
        if (GameManager.Instance)
        {
            GameManager.Instance.MusicPlayer.PlayMusicNote(Type);
        }
    }
}
