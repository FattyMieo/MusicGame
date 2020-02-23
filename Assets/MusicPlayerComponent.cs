using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectMusic;

[System.Serializable]
public struct MusicNotePair
{
    public NoteType Key;
    public AudioClip Value;

    public MusicNotePair(NoteType Note)
    {
        Key = Note;
        Value = null;
    }

    public MusicNotePair(int Note)
    {
        Key = (NoteType)Note;
        Value = null;
    }
}

[RequireComponent(typeof(AudioSource))]
public class MusicPlayerComponent : MonoBehaviour
{
    public AudioSource AudioSourceComp;
    public MusicNotePair[] MusicNotes = new MusicNotePair[(int)NoteType.Count]
    {
        new MusicNotePair(0),
        new MusicNotePair(1),
        new MusicNotePair(2),
        new MusicNotePair(3),
        new MusicNotePair(4),
        new MusicNotePair(5),
        new MusicNotePair(6),
        new MusicNotePair(7),
        new MusicNotePair(8),
        new MusicNotePair(9),
        new MusicNotePair(10),
        new MusicNotePair(11),
        new MusicNotePair(12)
    };

    void Awake()
    {
        AudioSourceComp = GetComponent<AudioSource>();
    }
    
    public void PlayMusicNote(NoteType Note)
    {
        Debug.Log(MusicNotes[(int)Note].Value);
        AudioSourceComp.PlayOneShot(MusicNotes[(int)Note].Value);
    }
}
