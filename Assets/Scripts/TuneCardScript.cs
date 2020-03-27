using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProjectMusic;

public class TuneCardScript : MonoBehaviour
{
    private Button MButton;
    private Image GlowImage;
    //public ColorBlock buttonColorBlock;
    public int Key;
    public int CardIndex;
    public bool BCorrect;
    
    void Awake()
    {
        MButton = this.GetComponentInChildren<Button>();
        GlowImage = this.GetComponent<Image>();
        BCorrect = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetGlow(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Button GetButton()
    {
        return MButton;
    }

    public void SetButtonColor(Color ButtonColor)
    {
        MButton.GetComponent<Image>().color = ButtonColor;
       //ColorBlock Temp = MButton.colors;
       //Temp.normalColor = ButtonColor;
       // MButton.colors = Temp;
    }

    public void SetGlow(bool ToGlow)
    {
        Color Temp = GlowImage.color;
        if (!ToGlow)
        {
            Temp.a = 0.0f;
        }
        else
        {
            Temp.a = 0.8f;
        }
        GlowImage.color = Temp;
    }
}
