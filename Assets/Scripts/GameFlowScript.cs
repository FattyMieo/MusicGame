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
    public Button PlayMusicButton;
    public Color[] ButtonIndicator;
    public float DistanceFromCenter;
    public int ButtonPressedLimit;
    

    //TuneCard Setups
    private int TotalTuneCards;
    public GameObject TuneCardPrefab;
    public List<TuneCardScript> TuneCards;

    // Solution Setups
    public int[] Solution;
    public int[] RandomizedKeyArray;

    // Answers
    public List<int> Answers;

    void Start()
    {
        MusicPlayerComponent.Instance.SetAudioList(Solution);
        TotalTuneCards = Solution.Length;

        // Setup delegates
        PlayMusicButton.onClick.AddListener(PlayMusic(false));
        PlayAllCardButton.onClick.AddListener(PlayMusic(true));

        RandomArrayElement();
        SpawnTuneCards();

        PlayMusic();
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
            CardScript.Key = RandomizedKeyArray[i];
            CardScript.GetButton().onClick.AddListener(delegate { OnMusicNoteClicked(CardScript.Key); });
            TuneCards.Add(CardScript);
        }
    }

    void RandomArrayElement()
    {
        RandomizedKeyArray = (int[])Solution.Clone();
        for (int i = 0; i < RandomizedKeyArray.Length; i++)
        {
            int temp = RandomizedKeyArray[i];
            int randomIndex = Random.Range(0, RandomizedKeyArray.Length-1);
            RandomizedKeyArray[i] = RandomizedKeyArray[randomIndex];
            RandomizedKeyArray[randomIndex] = temp;
        }
    }

    void OnMusicNoteClicked(int Key)
    {
        if (!MusicPlayerComponent.Instance.PlayAudioList)
        {
            MusicPlayerComponent.Instance.PlayMusicNote((NoteType)Key);
            /*Answers.Add(Key);

            if (CheckAnswer())
            {
                Debug.Log("Correct! Congratulations!");
            }
            else
            {
                Debug.Log("Incorrect Answer!");
            }*/
        }
    }

    public bool CheckAnswer()
    {
        for (int i = 0; i < Answers.Count; ++i)
        {
            int AnswerInt = Answers[i];
            int SolutionInt = Solution[i];

            if (AnswerInt != SolutionInt)
            {
                Answers.Clear();
               /* for (int j = 0; j < MusicNoteButtons.Length; ++j)
                {
                    MusicNoteButtons[j].Value.colors = ColorBlock.defaultColorBlock;
                }*/
                break;
            }
        }
        if (Solution.Length == Answers.Count)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void PlayMusic(bool useRandomizedAudio)
    {
        MusicPlayerComponent.Instance.InitiatePlayAudioList(useRandomizedAudio);
    }
}
