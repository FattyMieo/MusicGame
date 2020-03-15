using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProjectMusic;

[System.Serializable]
public struct MusicButton
{
    public int Key;
    public Button Value;
    public int limit;
}

public class GameFlowScript : MonoBehaviour
{
    // Button Setups
    public Button BeginAnswerButton;
    public Button PlayMusicButton;
    public MusicButton[] MusicNoteButtons;
    

    // Solution Setups
    public int[] Solution;
    public int[] RandomizeKeyArray;

    // Answers
    public List<int> Answers;

    private void Start()
    {
        MusicPlayerComponent.Instance.SetAudioList(Solution);

        // Setup delegates
        PlayMusicButton.onClick.AddListener(PlayMusic);
        RandomArrayElement();

        for (int i = 0; i < MusicNoteButtons.Length; ++i)
        {
            MusicButton CurrentButton = MusicNoteButtons[i];
            CurrentButton.Key = RandomizeKeyArray[i];

            CurrentButton.Value.onClick.AddListener(delegate { OnMusicNoteClicked(CurrentButton.Key); });
        }
        
    }

    void RandomArrayElement()
    {
        RandomizeKeyArray = (int[])Solution.Clone();
        for (int index = 0; index < RandomizeKeyArray.Length; index++)
        {
            int temp = RandomizeKeyArray[index];
            int randomIndex = Random.Range(0, RandomizeKeyArray.Length-1);
            RandomizeKeyArray[index] = RandomizeKeyArray[randomIndex];
            RandomizeKeyArray[randomIndex] = temp;
        }
    }

    void OnMusicNoteClicked(int Key)
    {
        if (!MusicPlayerComponent.Instance.PlayAudioList)
        {
            MusicPlayerComponent.Instance.PlayMusicNote((NoteType)Key);
            Answers.Add(Key);

            if (CheckAnswer())
            {
                Debug.Log("Correct! Congratulations!");
            }
            else
            {
                Debug.Log("Incorrect Answer!");
            }
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

    void PlayMusic()
    {
        MusicPlayerComponent.Instance.InitiatePlayAudioList();
    }
}
