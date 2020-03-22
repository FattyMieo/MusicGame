using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProjectMusic;

public class TuneCardScript : MonoBehaviour
{
    private Button MButton;
    private Image GlowImage;
    public ColorBlock buttonColorBlock;
    public int Key;
    public int CardIndex;
    public bool BCanInteract;
    
    void Awake()
    {
        MButton = this.GetComponentInChildren<Button>();
        GlowImage = this.GetComponent<Image>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
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
        ColorBlock Temp;
        Temp.normalColor = ButtonColor;
        MButton.colors = Temp;
    }

    public void SetGlow(bool ToGlow)
    {
        Color Temp;
        if (ToGlow)
        {
            Temp.a = 0;
        }
        else
        {
            Temp.a = 0.8f;
        }
        GlowImage.color = Temp;
    }
}
