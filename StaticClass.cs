using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public static class StaticClass {

    public static PlayerData data;
    public static int numOfWords;
    public static bool isStart=true;
    public static bool changeChar = false;
    public static bool isBackSelected = false;
    public static bool isTutorialStart;
    public static bool askUser = false;
    public static bool isPageSelected= false;
    public static int category;
    public static int currentpage = 1;
    public static int lastUserIndex;
    public static int userCount=5;
    public static string mode = "";
    public static int difficulty;
    public static int tutorialIndex;
    public static bool gameStart=true;
    public static string[] words;
    public static int[] word_id;
    public static string[] imageFP;
    public static int[] sound_id;
    public static int[] def_id;
    public static string[] praisePics = { "Good Job", "Great", "Nice One", "Perfect", "Well Played" };

    public static void Save(int characternum,string user_id)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/PlayerData.dat",FileMode.Create);
        data = new PlayerData();
        data.selectedChar = characternum;
        data.user_id = user_id;
        bf.Serialize(file, data);
        file.Close();
    }
    public static bool Load()
    {
        if (File.Exists(Application.persistentDataPath + "/PlayerData.dat"))
        {
            Debug.Log(Application.persistentDataPath);
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/PlayerData.dat", FileMode.Open);
            data = (PlayerData)bf.Deserialize(file);
            file.Close();
            return true;
        }
        else
            return false;
    }
    public static void changeScenes(int index)
    {
        SceneManager.LoadScene(index);
    }
    public static void changeCharacter(Image character, Image characterOver, Sprite boyCharacterSprite)
    {
        if (data.selectedChar==1)
        {
            character.overrideSprite = boyCharacterSprite;
            characterOver.overrideSprite = boyCharacterSprite;
        }
    }
    public static void playSound(AudioSource sound)
    {
        sound.Play();
    }
    public static void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("Scene Reloaded");
    }
    public static void openGameobject(GameObject go)
    {
        go.SetActive(true);
    }
    public static void closeGameobject(GameObject go)
    {
        go.SetActive(false);
    }
}
