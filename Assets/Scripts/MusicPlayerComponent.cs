 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectMusic;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct GameLevelInfo
{
    public int LevelNumber;
    public List<int> Sequence;
    public List<int> ShuffledSequence;

    public void ResetInfo()
    {
        LevelNumber = 0;
        Sequence.Clear();
        ShuffledSequence.Clear();
    }
}

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

[System.Serializable]
public enum EPlayingMode
{
    None = 0,
    SingleNote,
    Question,
    Hint,
    Answer,
    InitialDelay,
}

[RequireComponent(typeof(AudioSource))]
public class MusicPlayerComponent : MonoBehaviour
{
    //Singleton
    //!--------------------------------------------------
    private static MusicPlayerComponent m_Instance;

    public static MusicPlayerComponent Instance { get { return m_Instance; } }

    private AudioSource AudioSourceComp;

    //Playing Audio List Setting
    public EPlayingMode CurPlayingMode;
    private EPlayingMode NextPlayingMode;
    private int Iterator;
    private int AudioListSize;
    private float ClipTimer;
    private float CurrentClipLength;
    private float Delay;
    private float InitialDelayTimer;
    private float InitialDelay;
    private TuneCardScript CurPlayingCard;
    private List<TuneCardScript> PlayingTuneCards;
    public bool WinGame;
    public bool PlayingWrongNoteAnim;

    //Delegate
    public delegate void SequencePlayDelegate(bool isPlaying);
    public delegate void EndSequencePlayDelegate(bool isPlaying);
    
    public SequencePlayDelegate onSequencePlay;
    public EndSequencePlayDelegate onEndSequencePlay;

    //Level Data
    public GameLevelData AllLevelData;
    public GameLevelInfo LevelInfo;
    private LevelData CurrentLevelData;

    // Notes
    private MusicNotePair[] MusicNotes;
    
    /* public MusicNotePair[] MusicNotes = new MusicNotePair[(int)NoteType.Count]
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
     };*/

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
        AudioSourceComp.Stop();
    }

    void Start()
    {
        CurPlayingMode = EPlayingMode.None;
        Iterator = 0;
        AudioListSize = 0;
        ClipTimer = 0;
        CurrentClipLength = 0;
        PlayingWrongNoteAnim = false;
        WinGame = false;
        NextPlayingMode = EPlayingMode.None;
    }

    

    void Update()
    {
        if (CurPlayingMode == EPlayingMode.InitialDelay)
        {
            InitialDelayTimer += Time.deltaTime;
            if (InitialDelayTimer > InitialDelay)
            {
                PlayingTuneCards[Iterator].SetGlow(true);
                AudioSourceComp.Play();
                CurPlayingMode = NextPlayingMode;
                NextPlayingMode = EPlayingMode.None;
            }
        }
        else if (CurPlayingMode != EPlayingMode.None && CurPlayingMode != EPlayingMode.SingleNote)
        {
            LoopAudioList();
        }
        else if (CurPlayingMode == EPlayingMode.SingleNote)
        {
            if (!AudioSourceComp.isPlaying)
            {
                if (!PlayingWrongNoteAnim && CurPlayingCard != null)
                {
                    CurPlayingCard.SetGlow(false);
                    CurPlayingMode = EPlayingMode.None;
                    CurPlayingCard = null;
                }
                
                if (WinGame)
                {
                    onEndSequencePlay(true);
                }
            }
        }
    }

    public AudioSource GetAudioSource()
    {
        return AudioSourceComp;
    }

    LevelData RetrieveLevelData(int Level)
    {
        return AllLevelData.LevelDatas[Level];
    }

    public bool IsNextLevelAvailable()
    {
        //Debug.Log("Clevel " + CurrentLevelData.Level);
        //Debug.Log(AllLevelData.LevelDatas.Length);
        if (CurrentLevelData.Level + 1 >= AllLevelData.LevelDatas.Length)
        {
            return false;
        } 
        else
            return true;
    }

    void InitializeLevelInfo(int Level)
    {
        if (Level - 1 < -1)
            return;

        CurrentLevelData = RetrieveLevelData(Level);
        LevelInfo.LevelNumber = Level;

        
        MusicNotes = CurrentLevelData.Pairs;
        
        for (int i = 0; i < CurrentLevelData.Sequence.Length; ++i)
        {
            for (int j = 0; j < CurrentLevelData.Pairs.Length; ++j)
            {
                if (CurrentLevelData.Sequence[i] == CurrentLevelData.Pairs[j].Key)
                {
                    LevelInfo.Sequence.Add(j);
                }
            }
        }

        LevelInfo.ShuffledSequence = new List<int>(LevelInfo.Sequence);

        //Add Decoy if provided
        if (CurrentLevelData.Decoys.Length > 0)
        {
            for (int i = 0; i < CurrentLevelData.Decoys.Length; ++i)
            {
                for (int j = 0; j < CurrentLevelData.Pairs.Length; ++j)
                {
                    if (CurrentLevelData.Decoys[i] == CurrentLevelData.Pairs[j].Key)
                    {
                        LevelInfo.ShuffledSequence.Add(j);
                    }
                }
            }
        }
    }

    public GameLevelInfo RandomDSequenceElements(GameLevelInfo Info)
    {
        List<int> OriginalSequence = new List<int>(Info.ShuffledSequence);
        do
        {
            for (int i = 0; i < Info.ShuffledSequence.Count; i++)
            {
                int temp = Info.ShuffledSequence[i];
                int randomIndex = Random.Range(0, Info.ShuffledSequence.Count - 1);
                Info.ShuffledSequence[i] = Info.ShuffledSequence[randomIndex];
                Info.ShuffledSequence[randomIndex] = temp;
            }
        } while (OriginalSequence == Info.ShuffledSequence);
        return Info;
    }

    public void LoadGameLevel(int LevelIndex)
    {
        WinGame = false;
        PlayingWrongNoteAnim = false;
        LevelInfo.ResetInfo();
        InitializeLevelInfo(LevelIndex-1);
        SceneManager.LoadScene(1);
    }

    public void LoadLevel(int BuildIndex)
    {
        WinGame = false;
        PlayingWrongNoteAnim = false;
        if (BuildIndex == 0)
        {
            LevelInfo.ResetInfo();
        }
        SceneManager.LoadScene(BuildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public GameLevelInfo GetGameLevelInfo()
    {
        LevelInfo = RandomDSequenceElements(LevelInfo);
        return LevelInfo;
    }
   
    public void PlayMusicNote(NoteType Note)
    {
        AudioSourceComp.PlayOneShot(MusicNotes[(int)Note].Value);
    }

    public bool PlayAudioListT(float DelayBetweenAudio, float InitialDelay, List<TuneCardScript> TuneCards, EPlayingMode PlayMode)
    {
        if (CurPlayingMode != EPlayingMode.None)
        {
            return false;
        }
        else
        {
            Iterator = 0;
            ClipTimer = 0;
            Delay = DelayBetweenAudio;
            PlayingTuneCards = new List<TuneCardScript>(TuneCards);
            AudioListSize = PlayingTuneCards.Count-1;
            AudioSourceComp.clip = MusicNotes[PlayingTuneCards[Iterator].Key].Value;
            CurrentClipLength = MusicNotes[PlayingTuneCards[Iterator].Key].Value.length + Delay;
            

            if (InitialDelay > 0)
            {
                CurPlayingMode = EPlayingMode.InitialDelay;
                NextPlayingMode = PlayMode;
                InitialDelayTimer = 0;
                this.InitialDelay = InitialDelay;
            }
            else
            {
                CurPlayingMode = PlayMode;
                AudioSourceComp.Play();
                PlayingTuneCards[Iterator].SetGlow(true);
            }
            return true;
        }
    }

    public bool PlayAudioListT()
    {
        if (CurPlayingMode != EPlayingMode.None)
        {
            return false;
        }
        else
        {
            CurPlayingMode = EPlayingMode.Question;
            Iterator = 0;
            ClipTimer = 0;
            AudioListSize = LevelInfo.Sequence.Count-1;
            AudioSourceComp.clip = MusicNotes[LevelInfo.Sequence[Iterator]].Value;
            CurrentClipLength = MusicNotes[LevelInfo.Sequence[Iterator]].Value.length;
            AudioSourceComp.Play();
            onSequencePlay(true);
            return true;
        }
    }

    public bool PlayTuneCard(TuneCardScript TuneCard)
    {
        if (CurPlayingMode != EPlayingMode.None)
        {
            return false;
        }
        else
        {
            CurPlayingMode = EPlayingMode.SingleNote;
            CurPlayingCard = TuneCard;
            CurPlayingCard.SetGlow(true);
            PlayMusicNote((NoteType)CurPlayingCard.Key);
            return true;
        }
    }
    
    void LoopAudioList()
    {
        ClipTimer += Time.deltaTime;
        if (ClipTimer >= CurrentClipLength)
        {
            ClipTimer = 0;
            AudioSourceComp.Stop();

            if (Iterator < AudioListSize)
            {
                Iterator++;
                if (CurPlayingMode == EPlayingMode.Question)
                {
                    AudioSourceComp.clip = MusicNotes[LevelInfo.Sequence[Iterator]].Value;
                    CurrentClipLength = MusicNotes[LevelInfo.Sequence[Iterator]].Value.length;
                }
                else if (CurPlayingMode == EPlayingMode.Hint)
                {
                    PlayingTuneCards[Iterator - 1].SetGlow(false);
                    AudioSourceComp.clip = MusicNotes[PlayingTuneCards[Iterator].Key].Value;
                    CurrentClipLength = MusicNotes[PlayingTuneCards[Iterator].Key].Value.length + Delay;
                    PlayingTuneCards[Iterator].SetGlow(true);
                }
                else if (CurPlayingMode == EPlayingMode.Answer)
                {
                    PlayingTuneCards[Iterator - 1].SetGlow(false);
                    AudioSourceComp.clip = MusicNotes[PlayingTuneCards[Iterator].Key].Value;
                    CurrentClipLength = MusicNotes[PlayingTuneCards[Iterator].Key].Value.length + Delay;
                    PlayingTuneCards[Iterator].SetGlow(true);
                }
                AudioSourceComp.Play();
            }
            else
            {
                if (CurPlayingMode == EPlayingMode.Question)
                {
                    onSequencePlay(false);
                }
                else if (CurPlayingMode != EPlayingMode.None)
                {
                    PlayingTuneCards[Iterator].SetGlow(false);
                    if (CurPlayingMode == EPlayingMode.Answer)
                    {
                        onEndSequencePlay(false);
                    }
                }
                CurPlayingMode = EPlayingMode.None;
            }
        }
    }
}
