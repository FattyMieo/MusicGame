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
    //Singleton
    //!--------------------------------------------------
    private static MusicPlayerComponent m_Instance;

    public static MusicPlayerComponent Instance { get { return m_Instance; } }

    public AudioSource AudioSourceComp;
    public bool PlayAudioList;
    int Iterator;
    float ClipTimer;
    float CurrentClipLength;
    
    public int[] AudioList;
    public int[] RandomizedAudioList;
    private int[] CurrentAudioList;

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
        new MusicNotePair(12),
        new MusicNotePair(13)
    };

    void Awake()
    {
        if (m_Instance != null && m_Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            m_Instance = this;
        }
        DontDestroyOnLoad(this.gameObject);

        AudioSourceComp = GetComponent<AudioSource>();
    }

    void Start()
    {
        CurrentAudioList = AudioList;
        Iterator = 0;
        ClipTimer = 0;
        CurrentClipLength = 0;
    }

    void Update()
    {
        if (PlayAudioList)
        {
            LoopAudioList();
        }
    }

    public void SetAudioList(int[] Sequence)
    {
        AudioList = Sequence;
    }

    public void PlayMusicNote(NoteType Note)
    {
        //Debug.Log(MusicNotes[(int)Note].Value);
        AudioSourceComp.PlayOneShot(MusicNotes[(int)Note].Value);
    }

    public void InitiatePlayAudioList(bool IsRandomized)
    {
        if (!PlayAudioList)
        {
            PlayAudioList = true;
            Iterator = 0;
            ClipTimer = 0;
            if (!IsRandomized)
            {
                CurrentAudioList = AudioList;
            }
            else
            {
                CurrentAudioList = RandomizedAudioList;
            }
            AudioSourceComp.clip = MusicNotes[CurrentAudioList[Iterator]].Value;
            CurrentClipLength = MusicNotes[CurrentAudioList[Iterator]].Value.length;

            AudioSourceComp.Play();
        }
    }

    void LoopAudioList()
    {
        if (Iterator < CurrentAudioList.Length - 1)
        {
            ClipTimer += Time.deltaTime;
            if (ClipTimer >= CurrentClipLength)
            {
                ClipTimer = 0;
                AudioSourceComp.Stop();

                Iterator++;
                
                AudioSourceComp.clip = MusicNotes[CurrentAudioList[Iterator]].Value;
                CurrentClipLength = MusicNotes[CurrentAudioList[Iterator]].Value.length;
                AudioSourceComp.Play();
            }
        }
        else
        {
            PlayAudioList = false;
        }
    }
 }
