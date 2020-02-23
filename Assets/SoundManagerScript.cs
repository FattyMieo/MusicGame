using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{
    
    public int iterator;
    public float clipTimer;
    public float currentClipLength;
    public List<AudioClip> audioClipsList;

    AudioSource m_MyAudioSource;
    List<AudioClip> answerClipsList;
    bool bPlayAudioList;

    // Start is called before the first frame update
    void Start()
    {
        bPlayAudioList = false;
        iterator = 0;
        m_MyAudioSource = GetComponent<AudioSource>();
        m_MyAudioSource.clip = audioClipsList[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (bPlayAudioList)
        {
            LoopAudioList();
        }
    }

    public void PlayAudioList()
    {
        if (!bPlayAudioList)
        {
            bPlayAudioList = true;
            iterator = 0;
            clipTimer = 0;
            m_MyAudioSource.clip = audioClipsList[iterator];
            currentClipLength = audioClipsList[iterator].length;
            m_MyAudioSource.Play();
        }
    }

    void LoopAudioList()
    {
        if (iterator < audioClipsList.Count-1)
        {
            clipTimer += Time.deltaTime;
            if (clipTimer >= currentClipLength)
            {
                clipTimer = 0;
                m_MyAudioSource.Stop();
                
                iterator++;
                m_MyAudioSource.clip = audioClipsList[iterator];
                currentClipLength = audioClipsList[iterator].length;
                m_MyAudioSource.Play();
            }
        }
        else
        {
            bPlayAudioList = false;
        }
    }
}
