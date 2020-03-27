using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    public Button[] LevelButtons;
    public Button QuitButton;
    // Start is called before the first frame update
    void Start()
    {
        /*for (int i = 0; i < LevelButtons.Length; ++i)
        {
            LevelButtons[i].onClick.AddListener(delegate { MusicPlayerComponent.Instance.LoadGameLevel(i); });
        }*/
        LevelButtons[0].onClick.AddListener(delegate { MusicPlayerComponent.Instance.LoadGameLevel(1); });
        LevelButtons[1].onClick.AddListener(delegate { MusicPlayerComponent.Instance.LoadGameLevel(2); });
        LevelButtons[2].onClick.AddListener(delegate { MusicPlayerComponent.Instance.LoadGameLevel(3); });
        QuitButton.onClick.AddListener(delegate { MusicPlayerComponent.Instance.QuitGame(); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
