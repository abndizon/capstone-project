using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DfficultyScript : MonoBehaviour {

    public Image character;
    public Sprite boyCharacterSprite;
    public Text modeCategory;
    public Button mediumButton;
    public Button hardButton;
    public Button finalSceneButton;
    public Sprite mediumLock;
    public Sprite hardLock;
    DataAccess da;
    int level;
	void Awake () {
        if (StaticClass.data.selectedChar == 1)
            character.overrideSprite = boyCharacterSprite;

        if (StaticClass.mode=="untimed")
            modeCategory.text = (CategoryScript.categoryname + " - Story").ToUpper();
        else
            modeCategory.text = (CategoryScript.categoryname + " - " + StaticClass.mode).ToUpper();

        da=new DataAccess();
        if (da.dataExist("SELECT level FROM level WHERE user_id='" + StaticClass.data.user_id + "' and category=" + StaticClass.category + " and mode='" + StaticClass.mode + "'", "level"))
        {
            da = new DataAccess();
            level = da.getNum("SELECT level FROM level WHERE user_id='" + StaticClass.data.user_id + "' and category=" + StaticClass.category + " and mode='" + StaticClass.mode + "'", "level");
            if (level==2)
            {
                hardButton.enabled = false;
                hardButton.GetComponent<Image>().sprite = hardLock;
            }
            else if (level>=4)
            {
                finalSceneButton.gameObject.SetActive(true);
            }
        }
        else
        {
            mediumButton.enabled = false;
            mediumButton.GetComponent<Image>().sprite = mediumLock;
            hardButton.enabled = false;
            hardButton.GetComponent<Image>().sprite = hardLock;
        }
	}

    public void selectEasy()
    {
        StaticClass.difficulty = 1;
    }
    public void selectMedium()
    {
        StaticClass.difficulty = 2;
    }
    public void selectHard()
    {
        StaticClass.difficulty = 3;
    }
    public void showGameBasedOnCategory()
    {
        if (StaticClass.category==1)
        {
            StaticClass.changeScenes(4);
        }
        else
        {
            StaticClass.changeScenes(5);
        }
    }
}
