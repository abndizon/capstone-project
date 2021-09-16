using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CategoryScript : MonoBehaviour {

    public Image character;
    public Image modeImage;
    public Sprite boyCharacterSprite;
    public Sprite untimedSprite;
    public Sprite timedSprite;
    public static string categoryname;
	// Use this for initialization
	void Start () {
        if (StaticClass.data.selectedChar == 1)
            character.overrideSprite = boyCharacterSprite;
        if (StaticClass.mode=="")
            StaticClass.mode = "untimed";
        else
        {
            if (StaticClass.mode == "timed")
                modeImage.overrideSprite = timedSprite;
            else
                modeImage.overrideSprite = untimedSprite;
        }
	}
    public void changeMode()
    {
        if (StaticClass.mode=="untimed")
        {
            modeImage.overrideSprite = timedSprite;
            StaticClass.mode = "timed";
        }
        else
        {
            modeImage.overrideSprite = untimedSprite;
            StaticClass.mode = "untimed";
        }
    }
    public void selectJumbled()
    {
        categoryname = "jumbled";
        StaticClass.category = 2;
    }
    public void selectDrag()
    {
        categoryname = "drag";
        StaticClass.category = 1;
    }
}
