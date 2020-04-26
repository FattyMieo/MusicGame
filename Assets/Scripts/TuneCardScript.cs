using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TuneCardScript : MusicGameButton
{
    public int Key;
    public int CardIndex;
    public bool BCorrect;

    public override void Awake()
    {
        base.Awake();
        BCorrect = false;
    }
    
    
    /*
        RectTransform CardRT = Card.GetComponent<RectTransform>();
        float x = DistanceFromCenter * Mathf.Sin(Angle);
        float y = DistanceFromCenter * Mathf.Cos(Angle);
        CardRT.anchoredPosition = new Vector2(x, y);
        Angle += OffsetAngle;
     */

    /*public CheckCorrectness(Color RightColor,Color WrongColor)
    {
        if (BCorrect)
        {
            MButton.interactable = false;
            SetButtonColor(RightColor);
        }
        else
        {
            MButton.interactable = true;
            SetButtonColor(WrongColor);
        }
    }*/
}
