using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScript : MonoBehaviour {

    int selected;     //0-none 1-boy, 2-girl
    int indexOut;
    float timer = 1.5f;
    public Button boy;
    public Button girl;
    public Button select;
    public GameObject backButton;
    public GameObject charUpdatedPanel;
    public Image selectColor;
    public Sprite boyHighlight;
    public Sprite boyBox;
    public Sprite girlHighlight;
    public Sprite girlBox;
    GeneralScript gs;

    void Awake()
    {
        if (StaticClass.changeChar)
        {
            backButton.SetActive(false);
            selected = StaticClass.data.selectedChar;

            if (selected==1)
            {
                selectBoyCharacter();
            }
            else
            {
                selectGirlCharacter();
            }
        }
        else
        {
            selected = 0;
        }
    }

    void Update()
    {
        if (indexOut!=0)
        {
            timer -= Time.deltaTime;
        }
        if (Convert.ToInt32(timer) == 0)
        {
            DataAccess da = new DataAccess();
            da.saveDataToDB("UPDATE user_data SET tutor_id=" + selected + " WHERE user_id='" + StaticClass.data.user_id + "'");
            StaticClass.Save(selected, StaticClass.data.user_id);
            StaticClass.changeScenes(indexOut);
            Debug.Log("Updated");
        }
    }

    public void selectBoyCharacter()
    {
        if (selected==2)
        {
            girl.image.overrideSprite = girlBox;
        }
        boy.image.overrideSprite = boyHighlight;
        selected = 1;
        enableSelect();
    }
    public void selectGirlCharacter()
    {
        if (selected == 1)
        {
            boy.image.overrideSprite = boyBox;
        }
        girl.image.overrideSprite = girlHighlight;
        selected = 2;
        enableSelect();
    }

    public void enableSelect()
    {
        selectColor.color = new Color32(255, 255, 255, 255);
        select.enabled = true;
    }
    public void onSelect (int index)
    {
        if (StaticClass.Load())
        {
            if (StaticClass.changeChar)
            {
                if (StaticClass.data.selectedChar!=selected)
                {
                    charUpdatedPanel.SetActive(true);
                    indexOut = index;
                }
                else
                {
                    StaticClass.changeScenes(index);
                }
            }
            else
            {
                InsertNewUser(index);
            }
        }
        else
        {
            InsertNewUser(index);
        }
    }
    public void InsertNewUser(int index)
    {
        DataAccess da = new DataAccess();
        da.saveDataToDB("INSERT INTO user_data (user_id, tutor_id) VALUES ('" + AskNameScript.username + "'," + selected + ");");
        StaticClass.Save(selected, AskNameScript.username);
        AskNameScript.username = "";
        StaticClass.changeScenes(index);
        Debug.Log("Saved");
    }
    public void setAskUserToFalse()
    {
        StaticClass.askUser = false;
    }
}
