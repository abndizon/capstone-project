using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour {

    public static bool gameOver;
    public static int minutes;
    public static float seconds;
    public static int score;
    public static int indJumbled;
    public static int index;
    public static int mouseClick;
    public static Vector3 startingPos;
    public static GameObject firstClicked;
    public static string[] triviaWord;
    public static string[] fullTrivia;
    public static string[] wordPic;
    public static string[] audioWord;
    public static string[] tempWords;
    public static bool triviaShown;
    public static int indexInTriviaShown;
    public static int arrayIndex;
    public static int deduct;
    public static bool isPaused;
    public static bool isInstructionClose;
    
    void Start ()
    {
        isInstructionClose = false;
        isPaused = false;
        gameOver = false;
        minutes = 2;
        seconds = 0.0f;
        score = 0;
        mouseClick = 0;
        deduct = 0;
        startingPos = new Vector3();
        triviaWord = new string[50];
        fullTrivia = new string[50];
        wordPic = new string[50];
        audioWord = new string[50];
        tempWords = new string[50];
        index = 0;
        arrayIndex = 0;
        triviaShown = false;
        indexInTriviaShown = 0;
        indJumbled = 0;
    }
	void Update () {
	    
        if (StaticClass.mode=="timed")
        {
            if (!gameOver && (JumbledScript.isCountdownDone || DraggingScript.isCountdownDone) && !isPaused && (DraggingScript.done || JumbledScript.done))
            {
                if (seconds<=0.0f && minutes!=0)
                {
                    minutes--;
                    seconds = 59.0f;
                }
                else if (seconds>=0.0f)
                {
                    seconds -= Time.deltaTime;
                }
                else
                {
                    gameOver = true;
                    Debug.Log("Timer Done");
                }
            }
        }
	}
}
