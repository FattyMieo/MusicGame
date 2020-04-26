using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProjectMusic;

[System.Serializable]
public struct MusicButton
{
    public int Index;
    public int Key;
    public Button Value;
    public int PressedLimit;
}


public class GameFlowScript : MonoBehaviour
{
    //GameplaySetup
    public GameObject GameplayPanel;
    public Button HintButton; //hint button
    public MusicGameButton SongButton; //song button
    public GameObject GameMenuPanel;
    public Button MainMenuButton;
    public Button MainMenuButton2;
    public Button ReloadButton;
    public Button NextLevelButton;

    public Text SessionStateText;
    public Text HintLimitText;
    public Text LevelText;
    
    public Color[] ChanceIndicator;   // color to indicate how many chances left, start with the last index
    public Color[] CorrectIndicator;  // 0 is the main color, 1 is the supportive color
    public Color[] HintLimitIndicator;
    private int ChancesLeft;
    private int PlayHintLimit;
    public float HintDelay;
    public float HintInitialDelay;
    
    //TuneCard Setups
    private int TotalTuneCards;
    public float DistanceFromCenter;
    public GameObject TuneCardPrefab;
    public List<TuneCardScript> TuneCards;
    
    // Answers
    public List<int> Answers;
    private List<TuneCardScript> AnswerTCSequence;

    // Level Data
    public GameLevelInfo CurrentLevelInfo;

    void Start()
    {
        //bind to MusicPlayerComponent delegate
        MusicPlayerComponent.Instance.onSequencePlay = OnSongButtonClicked;
        MusicPlayerComponent.Instance.onEndSequencePlay = PlayEndSequence;

        CurrentLevelInfo = MusicPlayerComponent.Instance.GetGameLevelInfo();
        //condition ? first_expression : second_expression;
        LevelText.text = "Level " + (CurrentLevelInfo.LevelNumber < 10 ? "0" + (CurrentLevelInfo.LevelNumber+1).ToString() : (CurrentLevelInfo.LevelNumber + 1).ToString());
        
        ChancesLeft = ChanceIndicator.Length-1;
        PlayHintLimit = HintLimitIndicator.Length-1;
        Answers = new List<int>();
        AnswerTCSequence = new List<TuneCardScript>();
        HintButton.GetComponent<Image>().color = HintLimitIndicator[PlayHintLimit];
        TotalTuneCards = CurrentLevelInfo.ShuffledSequence.Count;
        SpawnTuneCards();

        // Setup delegates for button
        SongButton.GetButton().onClick.AddListener(PlaySong);
        HintButton.onClick.AddListener(PlayHint);
        MainMenuButton.onClick.AddListener(delegate { MusicPlayerComponent.Instance.LoadLevel(0); });
        MainMenuButton2.onClick.AddListener(delegate { MusicPlayerComponent.Instance.LoadLevel(0); });
        ReloadButton.onClick.AddListener(delegate { MusicPlayerComponent.Instance.LoadLevel(1); });

        ToggleInteractableButtons(true);
        GameMenuPanel.SetActive(false);
        
        PlaySong();
    }
    
    void SpawnTuneCards()
    {
         float OffsetAngle = 360.0f/(float)TotalTuneCards;

        //Convert the angle from degree to radian
        OffsetAngle = (OffsetAngle * Mathf.PI) / 180.0f;

        float Angle = 0;

         for(int i = 0;i< TotalTuneCards; ++i)
         {           
            GameObject Card = Instantiate(TuneCardPrefab,transform.position,Quaternion.identity);
            Card.transform.SetParent(GameplayPanel.transform, false);

            //Formula to get the position of a point along the circle
            //! y = r * sin(delta), x = r * cos(delta)
            RectTransform CardRT = Card.GetComponent<RectTransform>();
            float x = DistanceFromCenter * Mathf.Sin(Angle);
            float y = DistanceFromCenter * Mathf.Cos(Angle);
            CardRT.anchoredPosition = new Vector2(x, y);
            Angle += OffsetAngle;

            //Setting up the script
            TuneCardScript CardScript = Card.GetComponent<TuneCardScript>();
            CardScript.Key = CurrentLevelInfo.ShuffledSequence[i];
            CardScript.CardIndex = i;
            CardScript.SetButtonColor(ChanceIndicator[ChancesLeft]);
            CardScript.GetButton().onClick.AddListener(delegate { OnMusicNoteClicked(CardScript.CardIndex); });
            TuneCards.Add(CardScript);
        }
    }


    void OnMusicNoteClicked(int CardIndex)
    {
        if (MusicPlayerComponent.Instance.PlayTuneCard(TuneCards[CardIndex]))
        {
            Answers.Add(TuneCards[CardIndex].Key);
            AnswerTCSequence.Add(TuneCards[CardIndex]);

            if (CheckCurrentSequence())
            {
                OnCorrectStreak(CardIndex);
            }
            else
            {
                PlayFailAnimation(CardIndex);
                //OnFailedStreak();
            }
        }
    }
    
    void OnCorrectStreak(int TuneCardIndex)
    {
        HintButton.GetComponent<Image>().color = Color.white;
        SongButton.SetButtonColor(Color.white);
        TuneCards[TuneCardIndex].BCorrect = true;
        for (int i = 0; i < TuneCards.Count; ++i)
        {
            if (TuneCards[i].BCorrect)
            {
                TuneCards[i].SetButtonColor(CorrectIndicator[0]);
                TuneCards[i].GetButton().interactable = false;
            }
            else
                TuneCards[i].SetButtonColor(CorrectIndicator[1]);
        }
        if (Answers.Count == CurrentLevelInfo.Sequence.Count)
        {
            ToggleInteractableButtons(false);
            MusicPlayerComponent.Instance.WinGame = true;
        }
    }

    void OnFailedStreak()
    {
        MusicPlayerComponent.Instance.PlayingWrongNoteAnim = false;
        HintButton.GetComponent<Image>().color = HintLimitIndicator[PlayHintLimit];
        SongButton.SetButtonColor(Color.grey);
        ChancesLeft -= 1;
        if (ChancesLeft <= 0)
        {
            ChancesLeft = 0;
            EndSession(false);
        }
        Answers.Clear();
        AnswerTCSequence.Clear();
        for (int i = 0; i < TuneCards.Count; ++i)
        {
            TuneCards[i].BCorrect = false;
            TuneCards[i].GetButton().interactable = true;
            TuneCards[i].SetButtonColor(ChanceIndicator[ChancesLeft]);
        }
    }

    void PlayFailAnimation(int CardIndex)
    {
        MusicPlayerComponent.Instance.PlayingWrongNoteAnim = true;
        TuneCards[CardIndex].SetButtonColor(Color.red);


        RectTransform rect = TuneCards[CardIndex].GetComponent<RectTransform>();
        if (rect != null)
        {
            float TweenTargetLoc = rect.anchoredPosition.x + 5.0f;
            //LeanTween.moveX(rect, 5.0f, 0.1f).setLoopPingPong(2).setOnComplete(Done);
            LeanTween.moveX(rect, TweenTargetLoc, 0.1f).setLoopPingPong(4).setOnComplete(OnFailedStreak);
        }
    }

    public bool CheckCurrentSequence()
    {
        for (int i = 0; i < Answers.Count; ++i)
        {
            if (Answers[i] != CurrentLevelInfo.Sequence[i])
            {
                return false;
            }
        }
        return true;
    }

    void ToggleInteractableButtons(bool BInteractable)
    {
        HintButton.interactable = BInteractable;
        SongButton.GetButton().interactable = BInteractable;

        for (int i = 0; i < TuneCards.Count; ++i)
        {
            TuneCards[i].GetButton().interactable = BInteractable;
        }
    }



    void EndSession(bool BWin)
    {
        ToggleInteractableButtons(false);
        if (BWin)
            SessionStateText.text = "Congratulation!";
        else
            SessionStateText.text = "GG, You Lost.";

        if (MusicPlayerComponent.Instance.IsNextLevelAvailable())
        {
            NextLevelButton.gameObject.SetActive(true);
            NextLevelButton.onClick.RemoveAllListeners();
            NextLevelButton.onClick.AddListener(delegate { MusicPlayerComponent.Instance.LoadGameLevel(CurrentLevelInfo.LevelNumber + 2); });
        }
        else
        {
            NextLevelButton.gameObject.SetActive(false);
        }
        GameMenuPanel.SetActive(true);
    }
    
    void PlaySong()
    {
        if (Answers.Count == 0)
            MusicPlayerComponent.Instance.PlayAudioListT();
    }

    void PlayHint()
    {
        if (PlayHintLimit > 0 && Answers.Count == 0)
        {
            if (MusicPlayerComponent.Instance.PlayAudioListT(HintDelay, HintInitialDelay, TuneCards, EPlayingMode.Hint))
            {
                PlayHintLimit -= 1;
                HintButton.GetComponent<Image>().color = HintLimitIndicator[PlayHintLimit];
                HintLimitText.text = PlayHintLimit.ToString();
            }
        }
    }

    void PlayEndSequence(bool StartPlay)
    {
        if (StartPlay)
        {
            MusicPlayerComponent.Instance.PlayAudioListT(0, 0, AnswerTCSequence, EPlayingMode.Answer);
        }
        else
        {
            EndSession(true);
        }
    }

    void OnSongButtonClicked(bool StartPlay)
    {
        if (StartPlay)
        {
            HintButton.GetComponent<Image>().color = Color.white;
            SongButton.SetGlow(true);
            SongButton.SetButtonColor(Color.green);
            for (int i = 0; i < TuneCards.Count; ++i)
            {
                TuneCards[i].SetButtonColor(Color.white);
            }
        }
        else
        {
            HintButton.GetComponent<Image>().color = HintLimitIndicator[PlayHintLimit];
            SongButton.SetGlow(false);
            SongButton.SetButtonColor(Color.grey);
            for (int i = 0; i < TuneCards.Count; ++i)
            {
                TuneCards[i].SetButtonColor(ChanceIndicator[ChancesLeft]);
            }
        }
    }
}
