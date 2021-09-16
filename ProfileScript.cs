using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileScript : MonoBehaviour
{ 
    public Image character;
    public Sprite boyChar;
    DataAccess da;
    int count, i;

    // Use this for initialization
    void Start()
    {
        if (StaticClass.data.selectedChar == 1)
            character.overrideSprite = boyChar;

        da = new DataAccess();
        count = da.getNum("SELECT count(*) AS count FROM score WHERE user_id='" + StaticClass.data.user_id + "'", "count");

        //Set gameobjects to not active
        for (i = 15; i > count; i--)
        {
            GameObject.Find("Category" + i).SetActive(false);
            GameObject.Find("Difficulty" + i).SetActive(false);
            GameObject.Find("Score" + i).SetActive(false);
            GameObject.Find("Date" + i).SetActive(false);
        }

        if (count > 0)
        {
            string[] category = new string[15];
            string[] difficulty = new string[15];
            string[] score = new string[15];
            string[] date = new string[15];

            //Get data
            da = new DataAccess();
            category = da.getStringArray(15, "SELECT category FROM score WHERE user_id='" + StaticClass.data.user_id + "' ORDER BY score_id desc", "category");
            da = new DataAccess();
            difficulty = da.getStringArray(15, "SELECT difficulty FROM score WHERE user_id='" + StaticClass.data.user_id + "' ORDER BY score_id desc", "difficulty");
            da = new DataAccess();
            score = da.getStringArray(15, "SELECT score FROM score WHERE user_id='" + StaticClass.data.user_id + "' ORDER BY score_id desc", "score");
            da = new DataAccess();
            date = da.getStringArray(15, "SELECT date FROM score WHERE user_id='" + StaticClass.data.user_id + "' ORDER BY score_id desc", "date");

            //Put data
            for (i = 1; i <= count; i++)
            {
                if (category[i - 1] == "1")
                    GameObject.Find("Category" + i).GetComponent<Text>().text = "Drag";
                else
                    GameObject.Find("Category" + i).GetComponent<Text>().text = "Jumbled";

                if (difficulty[i - 1] == "1")
                    GameObject.Find("Difficulty" + i).GetComponent<Text>().text = "Easy";
                else if (difficulty[i - 1] == "2")
                    GameObject.Find("Difficulty" + i).GetComponent<Text>().text = "Medium";
                else
                    GameObject.Find("Difficulty" + i).GetComponent<Text>().text = "Hard";

                GameObject.Find("Score" + i).GetComponent<Text>().text = score[i - 1];
                GameObject.Find("Date" + i).GetComponent<Text>().text = date[i - 1];
            }
        }
    }
}
