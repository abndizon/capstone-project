using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GeneralScript : MonoBehaviour {

    public void changeScene(int index)
    {
        StaticClass.changeScenes(index);
    }
    public void playAudio(AudioSource aud)
    {
        StaticClass.playSound(aud);
    }
    public void ReloadCurrentScene()
    {
        StaticClass.ReloadScene();
    }
    public void showTrivia(GameObject pan)
    {
        pan.SetActive(true);
        TimerScript.triviaShown = true;
    }
    public void nextTrivia ()
    {
        TimerScript.indexInTriviaShown++;
    }
    public void prevTrivia()
    {
        TimerScript.indexInTriviaShown--;
    }
    public void closeTrivia(GameObject go)
    {
        go.SetActive(false);
        TimerScript.indexInTriviaShown = 0;
    }
    public void nextInstruction (GameObject go)
    {
        go.SetActive(true);
    }
    public void backInstruction (GameObject go)
    {
        go.SetActive(false);
    }
    public void startGame(ref bool isInstructionClose)
    {
        isInstructionClose = true;
    }
    public void pauseGame(GameObject go)
    {
        TimerScript.isPaused = true;
        go.SetActive(true);
    }
    public void resumeGame(GameObject go)
    {
        TimerScript.isPaused = false;
        go.SetActive(false);
    }
    public void closeGameobject (GameObject go)
    {
        StaticClass.closeGameobject(go);
    }
    public void openGameobject (GameObject go)
    {
        StaticClass.openGameobject(go);
    }
    public void DestroyGameObject(string str)
    {
        Destroy(GameObject.FindGameObjectWithTag(str));
    }
    public void resetGame ()
    {
        JumbledScript.isInstructionClose = false;
        DraggingScript.isInstructionClose = false;
        DraggingScript.isCountdownDone = false;
        JumbledScript.isCountdownDone = false;
    }
   public void resetTutorialIndex()
    {
        StaticClass.tutorialIndex=0;
        StaticClass.isTutorialStart = true;
    }
    public void nextWordTutorial()
    {
        StaticClass.tutorialIndex++;
    }
    public void prevWordTutorial()
    {
        StaticClass.tutorialIndex--;
    }
    public void setTutorialToFalse()
    {
        StaticClass.isTutorialStart = false;
    }
    public void unmuteMusic()
    {
        GameObject.Find("data GO").GetComponent<AudioSource>().mute = false;
    }
    public void changeGameStart()
    {
        StaticClass.gameStart = true;
    }
    public void openIfTimed(GameObject go)
    {
        if (StaticClass.mode=="timed")
        {
            StaticClass.openGameobject(go);
        }
    }
    public void selectNo()
    {
        StaticClass.data.user_id = "";
    }
    public void nextScene()
    {
        if (StaticClass.difficulty==3)
        {
            DestroyGameObject("praise");
            DestroyGameObject("timer");
            resetGame();
            StaticClass.changeScenes(8);
        }
        else if (StaticClass.difficulty<3)
        {
            StaticClass.difficulty++;
            changeGameStart();
            resetGame();
            ReloadCurrentScene();
        }
    }
    public void exitGame()
    {
        Application.Quit();
    }
}
