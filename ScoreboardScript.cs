using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//
public class ScoreboardScript : MonoBehaviour {

    DataAccess da;
    int i,count;
    public Sprite dragOrange;
    public Sprite jumbledOrange;
    public Button dragButton;
    public Button jumbledButton;
	// Use this for initialization
	void Start () {

        Debug.Log("Category:" + StaticClass.category);

        if (StaticClass.category == 1)
        {
            jumbledButton.GetComponent<Image>().sprite = jumbledOrange;
        }
        else
        {
            dragButton.GetComponent<Image>().sprite = dragOrange;
        }

        da = new DataAccess();
        count = da.getNum("SELECT count(*) AS count FROM score where category=" + StaticClass.category, "count");

        //Set gameobjects to not active
        for (i = 10; i > count; i--)
        {
            GameObject.Find("Number" + i).SetActive(false);
            GameObject.Find("Username" + i).SetActive(false);
            GameObject.Find("Score" + i).SetActive(false);
            GameObject.Find("Difficulty" + i).SetActive(false);
        }

        if (count > 0)
        {
            if (count > 10)
                count = 10;

            string[] usernames = new string[10];
            string[] score = new string[10];
            string[] difficulty = new string[10];

            //Get data
            da = new DataAccess();
            usernames = da.getStringArray(10, "SELECT user_id FROM score where category="+ StaticClass.category + " ORDER BY score desc", "user_id");
            da = new DataAccess();
            score = da.getStringArray(10, "SELECT score FROM score where category=" + StaticClass.category + " ORDER BY score desc", "score");
            da = new DataAccess();
            difficulty = da.getStringArray(10, "SELECT difficulty FROM score WHERE category=" + StaticClass.category + " ORDER BY score desc", "difficulty");

            //Put data
            for (i = 1; i <= count; i++)
            {
                GameObject.Find("Username" + i).GetComponent<Text>().text = usernames[i - 1];
                GameObject.Find("Score" + i).GetComponent<Text>().text = score[i - 1];

                if (difficulty[i - 1] == "1")
                    GameObject.Find("Difficulty" + i).GetComponent<Text>().text = "Easy";
                else if (difficulty[i - 1] == "2")
                    GameObject.Find("Difficulty" + i).GetComponent<Text>().text = "Medium";
                else
                    GameObject.Find("Difficulty" + i).GetComponent<Text>().text = "Hard";
            }
        }
    }
	
    public void selectDrag()
    {
        StaticClass.category = 1;
    }
    public void selectJumbled()
    {
        StaticClass.category = 2;
    }
}
