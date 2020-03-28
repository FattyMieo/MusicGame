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
    public List<int> DisorganizedSequence;

    public void ResetInfo()
    {
        LevelNumber = 0;
        Sequence.Clear();
        DisorganizedSequence.Clear();
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

[RequireComponent(typeof(AudioSource))]
public class MusicPlayerComponent : MonoBehaviour
{
    //Singleton
    //!--------------------------------------------------
    private static MusicPlayerComponent m_Instance;

    public static MusicPlayerComponent Instance { get { return m_Instance; } }

    private AudioSource AudioSourceComp;
    private bool PlayAudioList;
    private bool PlaySequence;
    private int Iterator;
    private float ClipTimer;
    private float CurrentClipLength;

    //Level Data
    public GameLevelData AllLevelData;
    private LevelData CurrentLevelData;

    //List of Audio and Notes
    private MusicNotePair[] MusicNotes;
    public GameLevelInfo LevelInfo;
    private List<int> CurrentAudioList;

    public int CurrentTuneCardIndex;
    private List<TuneCardScript> M_TuneCards;
    public MusicGameButton PlaySequenceButton;
    
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
        PlaySequence = true;
        CurrentTuneCardIndex = -1;
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
        else if (AudioSourceComp.isPlaying)
        {
            
            if (CurrentTuneCardIndex >= 0)
            {
                M_TuneCards[CurrentTuneCardIndex].SetGlow(true);
            }
        }
        else
        {
            if (CurrentTuneCardIndex >= 0)
            {
                M_TuneCards[CurrentTuneCardIndex].SetGlow(false);
                CurrentTuneCardIndex = -1;
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

        LevelInfo.DisorganizedSequence = new List<int>(LevelInfo.Sequence);

        //Add Decoy if provided
        if (CurrentLevelData.Decoys.Length > 0)
        {
            for (int i = 0; i < CurrentLevelData.Decoys.Length; ++i)
            {
                for (int j = 0; j < CurrentLevelData.Pairs.Length; ++j)
                {
                    if (CurrentLevelData.Decoys[i] == CurrentLevelData.Pairs[j].Key)
                    {
                        LevelInfo.DisorganizedSequence.Add(j);
                    }
                }
            }
        }
    }

    public GameLevelInfo RandomDSequenceElements(GameLevelInfo Info)
    {
        for (int i = 0; i < Info.DisorganizedSequence.Count; i++)
        {
            int temp = Info.DisorganizedSequence[i];
            int randomIndex = Random.Range(0, Info.DisorganizedSequence.Count - 1);
            Info.DisorganizedSequence[i] = Info.DisorganizedSequence[randomIndex];
            Info.DisorganizedSequence[randomIndex] = temp;
        }
        return Info;
    }

    public void LoadGameLevel(int LevelIndex)
    {
        InitializeLevelInfo(LevelIndex-1);
        SceneManager.LoadScene(1);
    }

    public void LoadLevel(int BuildIndex)
    {
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
    
    //Dangerous refactor later on
    public void SetTuneCardList(List<TuneCardScript> tuncards)
    {
        M_TuneCards = tuncards;
    }

    public void PlayMusicNote(NoteType Note)
    {
        AudioSourceComp.PlayOneShot(MusicNotes[(int)Note].Value);
    }

    public void InitiatePlayAudioList(bool IsSequence)
    {
        PlaySequence = IsSequence;
        if (!AudioSourceComp.isPlaying)
        {
            PlayAudioList = true;
            Iterator = 0;
            ClipTimer = 0;

            if (PlaySequence)
            {
                PlaySequenceButton.SetGlow(true);
                PlaySequenceButton.SetButtonColor(Color.green);
                CurrentAudioList = new List<int>(LevelInfo.Sequence);
            }
            else
            {
                CurrentAudioList = new List<int>(LevelInfo.DisorganizedSequence);
                CurrentTuneCardIndex = Iterator;
                M_TuneCards[CurrentTuneCardIndex].SetGlow(true);
            }

            AudioSourceComp.clip = MusicNotes[CurrentAudioList[Iterator]].Value;
            CurrentClipLength = MusicNotes[CurrentAudioList[Iterator]].Value.length;

            AudioSourceComp.Play();
        }
    }



    void LoopAudioList()
    {
        if (Iterator < CurrentAudioList.Count)
        {
            ClipTimer += Time.deltaTime;
            if (ClipTimer >= CurrentClipLength)
            {
                ClipTimer = 0;
                AudioSourceComp.Stop();

                if (!PlaySequence)
                {
                    M_TuneCards[CurrentTuneCardIndex].SetGlow(false);
                }

                Iterator++;

                if (Iterator < CurrentAudioList.Count)
                {
                    if (!PlaySequence)
                    {
                        CurrentTuneCardIndex = Iterator;
                        M_TuneCards[CurrentTuneCardIndex].SetGlow(true);
                    }

                    AudioSourceComp.clip = MusicNotes[CurrentAudioList[Iterator]].Value;
                    CurrentClipLength = MusicNotes[CurrentAudioList[Iterator]].Value.length;

                    AudioSourceComp.Play();
                }
                else
                {
                    PlaySequenceButton.SetGlow(false);
                    PlaySequenceButton.SetButtonColor(Color.white);
                    CurrentTuneCardIndex = -1;
                    PlaySequence = true;
                    PlayAudioList = false;
                }
            }
        }
    }
 }
