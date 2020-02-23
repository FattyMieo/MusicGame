using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct MusicButton
{
    public int Key;
    public Button Value;
}

public class GameFlowScript : MonoBehaviour
{
    // Button Setups
    public Button BeginAnswerButton;
    public Button PlayMusicButton;
    public MusicButton[] MusicNoteButtons;

    // Solution Setups
    public int[] Solutions;

    // Answers
    public bool IsAnswering = false;
    public List<int> Answers;

    private void Start()
    {
        // Setup delegates
        BeginAnswerButton.onClick.AddListener(OnBeginAnswer);
        PlayMusicButton.onClick.AddListener(PlayMusic);

        for(int i = 0; i < MusicNoteButtons.Length; ++i)
        {
            MusicButton CurrentButton = MusicNoteButtons[i];
            CurrentButton.Value.onClick.AddListener(delegate { OnMusicNoteClicked(CurrentButton.Key); });
        }
    }

    void OnMusicNoteClicked(int Key)
    {
        SoundManagerScript soundManager = Object.FindObjectOfType<SoundManagerScript>(); // Convert to singleton
        soundManager.GetComponent<AudioSource>().PlayOneShot(soundManager.audioClipsList[Key]);

        // If player is answering, also records answer to list
        if (IsAnswering)
        {
            Answers.Add(Key);

            // If there are enough answers, complete the game
            if(Answers.Count >= Solutions.Length)
            {
                OnAnswerComplete();
            }
        }
    }

    void OnBeginAnswer()
    {
        Answers.Clear();
        IsAnswering = true;
    }

    void OnAnswerComplete()
    {
        IsAnswering = false;

        // Check answers with solutions
        bool IsCorrectAnswer = true;
        for(int i = 0; i < Solutions.Length && i < Answers.Count; ++i)
        {
            int AnswerInt = Answers[i];
            int SolutionInt = Solutions[i];

            if (AnswerInt != SolutionInt)
            {
                IsCorrectAnswer = false;
                break;
            }
        }

        // Results
        if(IsCorrectAnswer)
        {
            Debug.Log("Correct! Congratulations!");
        }
        else
        {
            Debug.Log("Incorrect Answer!");
        }
    }

    void PlayMusic()
    {
        SoundManagerScript soundManager = Object.FindObjectOfType<SoundManagerScript>(); // Convert to singleton
        soundManager.PlayAudioList();
    }
}
