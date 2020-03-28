using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicGameButton : MonoBehaviour
{
    protected Button MButton;
    protected Image GlowImage;

    public virtual void Awake()
    {
        MButton = this.GetComponentInChildren<Button>();
        GlowImage = this.GetComponent<Image>();
        SetGlow(false);
    }
    
    public Button GetButton()
    {
        return MButton;
    }

    public void SetButtonColor(Color ButtonColor)
    {
        MButton.GetComponent<Image>().color = ButtonColor;
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
