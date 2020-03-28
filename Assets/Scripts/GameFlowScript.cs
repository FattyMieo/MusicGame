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
    public Button PlayAllCardButton;
    public MusicGameButton PlayMusicButton;
    public GameObject GameMenuPanel;
    public Button MainMenuButton;
    public Button ReloadButton;

    public Text SessionStateText;
    public Text PlayTuneCardsText;
    public Text LevelText;
    
    public Color[] ChanceIndicator;   // color to indicate how many chances left, start with the last index
    public Color[] CorrectIndicator;  // 0 is the main color, 1 is the supportive color
    private int ChancesLeft;
    
    
    public float DistanceFromCenter;
    public int ButtonPressedLimit;
    

    //TuneCard Setups
    private int TotalTuneCards;
    public GameObject TuneCardPrefab;
    public List<TuneCardScript> TuneCards;
    
    // Answers
    public List<int> Answers;

    // Level Data
    public GameLevelInfo CurrentLevelInfo;

    void Start()
    {
        CurrentLevelInfo = MusicPlayerComponent.Instance.GetGameLevelInfo();
        //condition ? first_expression : second_expression;
        LevelText.text = "Level " + (CurrentLevelInfo.LevelNumber < 10 ? "0" + (CurrentLevelInfo.LevelNumber+1).ToString() : (CurrentLevelInfo.LevelNumber + 1).ToString());
        
        ChancesLeft = ChanceIndicator.Length-1;
        PlayAllCardButton.GetComponent<Image>().color = CorrectIndicator[0];
        
        TotalTuneCards = CurrentLevelInfo.DisorganizedSequence.Count;
        SpawnTuneCards();
        MusicPlayerComponent.Instance.SetTuneCardList(TuneCards);

        // Setup delegates
        PlayMusicButton.GetButton().onClick.AddListener(PlayFullMusic);
        PlayAllCardButton.onClick.AddListener(PlayAllTuneCards);
        MainMenuButton.onClick.AddListener(delegate { MusicPlayerComponent.Instance.LoadLevel(0); });
        ReloadButton.onClick.AddListener(delegate { MusicPlayerComponent.Instance.LoadLevel(1); });

        ToggleInteractableButtons(true);
        GameMenuPanel.SetActive(false);

        MusicPlayerComponent.Instance.PlaySequenceButton = PlayMusicButton;
        //!Bug currently skip the first audio source
        PlayFullMusic();
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
            CardScript.Key = CurrentLevelInfo.DisorganizedSequence[i];
            CardScript.CardIndex = i;
            CardScript.SetButtonColor(CorrectIndicator[0]);
            CardScript.GetButton().onClick.AddListener(delegate { OnMusicNoteClicked(CardScript.Key,CardScript.CardIndex); });
            TuneCards.Add(CardScript);
        }
    }
    

    void OnMusicNoteClicked(int Key,int Index)
    {
        if (!MusicPlayerComponent.Instance.GetAudioSource().isPlaying)
        {
            MusicPlayerComponent.Instance.CurrentTuneCardIndex = Index;
            MusicPlayerComponent.Instance.PlayMusicNote((NoteType)Key);
            Answers.Add(Key);

            if (CheckCurrentSequence())
            {
                OnCorrectStreak(Index);
            }
            else
            {
                OnFailedStreak();
            }
            
        }
    }

    void OnCorrectStreak(int TuneCardIndex)
    {
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
            EndSession(true);
        }
    }

    void OnFailedStreak()
    {
        Answers.Clear();
        PlayAllCardButton.GetComponent<Image>().color = ChanceIndicator[ChancesLeft];
        for (int i = 0; i < TuneCards.Count; ++i)
        {
            TuneCards[i].BCorrect = false;
            TuneCards[i].GetButton().interactable = true;
            TuneCards[i].SetButtonColor(ChanceIndicator[ChancesLeft]);
        }

        if (ChancesLeft <= 0)
        {
            EndSession(false);
        }
        else if (ChancesLeft > 0)
        {
            ChancesLeft -= 1;
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
        PlayAllCardButton.interactable = BInteractable;
        PlayMusicButton.GetButton().interactable = BInteractable;

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

        GameMenuPanel.SetActive(true);
    }
    
    void PlayFullMusic()
    {
        if(!MusicPlayerComponent.Instance.GetAudioSource().isPlaying)
            MusicPlayerComponent.Instance.InitiatePlayAudioList(true);
    }

    void PlayAllTuneCards()
    {
        if (!MusicPlayerComponent.Instance.GetAudioSource().isPlaying && ButtonPressedLimit > 0)
        {
            ButtonPressedLimit -= 1;
            PlayTuneCardsText.text = ButtonPressedLimit.ToString();
            MusicPlayerComponent.Instance.InitiatePlayAudioList(false);
        }
    }
}
