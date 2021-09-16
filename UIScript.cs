using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;
using UnityEngine.EventSystems;
using System.IO;

public class UIScript : MonoBehaviour
{
    public Image bg;
    public Image title;
    public Image sBg;
    public Image opBg;
    public Image character;
    public Image cuBg;
    public Text welcomeText;
    public Text askText;
    public Sprite mbutton;
    public Sprite mbuttondisable;
    public Sprite boyCharacterSprite;
    public Button opButton;
    public Button startButton;
    public Button avatarButton;
    public Button changeUserButton;
    public Button saveButton;
    public Button addButton;
    public Button usersBackButton;
    public Button deleteButton;
    public Button renameButton;
    public Button selectButton;
    public Button userBackButton;
    public Button userNextButton;
    public Toggle musicToggle;
    public InputField renameField;
    public GameObject usersPanel;
    public GameObject renamePanel;
    public GameObject usernameExistPanel;
    public GameObject menuPanel;
    public GameObject askUserPanel;
    public GameObject updatePanel;
    private AudioSource gameMusic;
    GameObject lastGameObjectSelected;
    string selectedUser;
    string[] names;
    int numOfUsers, i, num, userCountPerPage;
    bool isUserPanelOpened=false;
    DataAccess da;
    //
    void Start()
    {
        selectedUser = "";
        StaticClass.category = 1;

        StartCoroutine(getLatestWords());

        //Change character based on data
        if (StaticClass.data.selectedChar == 1)
            character.overrideSprite = boyCharacterSprite;

        //Get music
        GameObject musicGO = GameObject.FindGameObjectWithTag("data");
        gameMusic = musicGO.GetComponent<AudioSource>();

        //For music button sprite
        if (gameMusic.mute)
        {
            musicToggle.image.overrideSprite = mbuttondisable;
            musicToggle.isOn = false;
        }
        else
        {
            musicToggle.image.overrideSprite = mbutton;
            musicToggle.isOn = true;
        }
            
        //Display Welcome text based on name of user
        welcomeText.text += StaticClass.data.user_id + "!";

        if (StaticClass.isBackSelected)
        {
            StaticClass.openGameobject(usersPanel);
            openUserPanel();
            if (StaticClass.askUser)
            {
                usersBackButton.gameObject.SetActive(false);
            }
            StaticClass.isBackSelected = false;
        }

        if (!StaticClass.isPageSelected)
        {
            resetUserPanelData();
        }
        else
        {
            StaticClass.isPageSelected = false;
        }

        //Reset staticClass mode
        StaticClass.mode = "";

        if (StaticClass.askUser)
        {
            menuPanel.SetActive(false);
            askUserPanel.SetActive(true);
            askText.text = "Are you '" + StaticClass.data.user_id.ToUpper() + "'?";
        }

        Debug.Log("PlayerDATA USER: " + StaticClass.data.user_id);
    }

    void Update()
    {
        if (renamePanel.activeInHierarchy)
        {
            if (renameField.text.Length > 1 && renameField.text.Length<=15 || renameField.text.ToUpper() == StaticClass.data.user_id.ToUpper())
            {
                saveButton.enabled = true;
                saveButton.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }
            else
            {
                saveButton.enabled = false;
                saveButton.GetComponent<Image>().color = new Color32(160, 160, 160, 100);
            }
        }
    }

    public void GameMusic()
    {
        if (musicToggle.isOn)
        {
            gameMusic.mute = false;
            musicToggle.image.overrideSprite = mbutton;
        }
        else
        {
            gameMusic.mute = true;
            musicToggle.image.overrideSprite = mbuttondisable;
        }
    }
    public void changeCharacter()
    {
        StaticClass.changeChar = true;
    }
    public void addUser()
    {
        StaticClass.changeChar = false;
    }
    public void changeIsStart ()
    {
        StaticClass.isStart = false;
    } 
    public void openUserPanel()
    {
        da = new DataAccess();
        numOfUsers = da.getNum("SELECT count(*) AS count FROM user_data", "count");

        if (numOfUsers > 5)
        {
            if (StaticClass.currentpage > 1)
            {
                userCountPerPage = numOfUsers - ((StaticClass.currentpage - 1) * 5);
            }
            else
                userCountPerPage = 5;
        }
        else
            userCountPerPage = numOfUsers;

        if (!isUserPanelOpened)
        {
            //Set buttons to not active based on number of users
            for (i = 5; i > userCountPerPage; i--)
            {
                GameObject.Find("Button" + i).SetActive(false);
            }
            isUserPanelOpened = true;
        }

        StaticClass.lastUserIndex = 5 * (StaticClass.currentpage-1);

        //Get users then highlight the selected user
        da = new DataAccess();
        names = da.getStringArray(numOfUsers, "SELECT user_id FROM user_data ORDER BY user_id;", "user_id");
        for (i = 0; i < userCountPerPage; i++)
        {
            GameObject.FindGameObjectWithTag("user" + (i + 1)).GetComponent<Text>().text = names[StaticClass.lastUserIndex];
            if (names[StaticClass.lastUserIndex].ToUpper() == StaticClass.data.user_id.ToUpper())
            {
                GameObject.FindGameObjectWithTag("user" + (i + 1)).GetComponentInParent<Image>().color = new Color32(98, 253, 0, 255);
                lastGameObjectSelected = GameObject.FindGameObjectWithTag("user" + (i + 1)).GetComponent<Transform>().parent.gameObject;
                selectedUser = lastGameObjectSelected.GetComponentInChildren<Text>().text;
                Debug.Log("Last Game Object Selected: " + lastGameObjectSelected);
                Debug.Log("Selected User: " + selectedUser);
            }
            else
            {
                GameObject.FindGameObjectWithTag("user" + (i + 1)).GetComponentInParent<Image>().color = new Color32(255, 255, 255, 255);
            }
            StaticClass.lastUserIndex++;
            checkUserToSelected();
        }

        if (selectedUser == "")
        {
            selectButton.enabled = false;
            selectButton.GetComponent<Image>().color = new Color32(160, 160, 160, 100);

            if (StaticClass.data.user_id == "")
                usersBackButton.gameObject.SetActive(false);
        }
        else
        {
            selectButton.enabled = true;
            selectButton.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }

        if ((numOfUsers-StaticClass.userCount)>0)
            userNextButton.gameObject.SetActive(true);
        else
            userNextButton.gameObject.SetActive(false);

        if (StaticClass.currentpage>1)
            userBackButton.gameObject.SetActive(true);
        else
            userBackButton.gameObject.SetActive(false);

        //if (numOfUsers == 5)
        //{
        //    addButton.enabled = false;
        //    addButton.GetComponent<Image>().color = new Color32(160, 160, 160, 100);
        //}
    }
    public void selectUser()
    {
        if (lastGameObjectSelected != null)
        {
            lastGameObjectSelected.GetComponent<Image>().color = new Color32(255,255,255,255);
        }
        lastGameObjectSelected = EventSystem.current.currentSelectedGameObject;
        selectedUser = lastGameObjectSelected.GetComponentInChildren<Text>().text;
        lastGameObjectSelected.GetComponent<Image>().color = new Color32(98,253,0,255);

        if (selectedUser == "")
        {
            selectButton.enabled = false;
            selectButton.GetComponent<Image>().color = new Color32(160, 160, 160, 100);

            usersBackButton.gameObject.SetActive(false);
        }
        else
        {
            selectButton.enabled = true;
            selectButton.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }

        checkUserToSelected();

        Debug.Log("Selected User: " + selectedUser);
        Debug.Log("Last Game Object Selected: " + lastGameObjectSelected);
    }
    public void clickSelectButton()
    {
        da = new DataAccess();
        num = da.getNum("SELECT tutor_id FROM user_data WHERE upper(user_id)=upper('" + selectedUser + "')","tutor_id");
        StaticClass.Save(num, selectedUser);
        StaticClass.ReloadScene();
    }
    public void deleteUser()
    {
        if (numOfUsers>1)
        {
            da = new DataAccess();
            da.saveDataToDB("PRAGMA foreign_keys = ON; DELETE from user_data WHERE upper(user_id)=upper('" + selectedUser + "');");
            selectedUser = "";

            StaticClass.isBackSelected = true;
            clickSelectButton();
        }
        else
        {
            File.Delete(Application.persistentDataPath + "/PlayerData.dat");
            da = new DataAccess();
            da.saveDataToDB("PRAGMA foreign_keys = ON; DELETE from user_data WHERE upper(user_id)=upper('" + selectedUser + "');"); 
            //Reset values of variables in StaticClass
            StaticClass.isStart=true;
            StaticClass.changeChar = false;
            StaticClass.isBackSelected = false;
            StaticClass.changeScenes(0);
        }
    }
    public void renameUser()
    {
        renameField.text = selectedUser;
    }
    public void saveUsername()
    {
        da = new DataAccess();
        if (renameField.text.ToUpper() == selectedUser.ToUpper())
        {
            renamePanel.SetActive(false);
        }
        else
        {
            if (da.dataExist("SELECT user_id FROM user_data WHERE upper(user_id)=upper('" + renameField.text + "');", "user_id"))
            {
                usernameExistPanel.SetActive(true);
            }
            else
            {
                da = new DataAccess();
                da.saveDataToDB("PRAGMA foreign_keys = ON; UPDATE user_data SET user_id = '" + renameField.text + "' WHERE user_id='" + selectedUser + "';");

                if (selectedUser.ToUpper() == StaticClass.data.user_id.ToUpper())
                {
                    StaticClass.Save(StaticClass.data.selectedChar, renameField.text);
                    welcomeText.text = "Welcome, " + renameField.text;
                }

                renamePanel.SetActive(false);
                selectedUser = renameField.text;
                lastGameObjectSelected.GetComponentInChildren<Text>().text = selectedUser;
            }
        }
    }
    public void setAskUserToFalse()
    {
        StaticClass.askUser = false;
    }
    public void checkUserToSelected()
    {
        if (StaticClass.data.user_id.ToUpper() == selectedUser.ToUpper() && StaticClass.data.user_id.ToUpper()!="" && selectedUser.ToUpper()!="")
        {
            deleteButton.enabled = true;
            renameButton.enabled = true;
            deleteButton.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            renameButton.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
        else
        {
            deleteButton.enabled = false;
            renameButton.enabled = false;
            deleteButton.GetComponent<Image>().color = new Color32(103, 103, 103, 100);
            renameButton.GetComponent<Image>().color = new Color32(103, 103, 103, 100);
        }
    }
    public void nextPage()
    {
        StaticClass.currentpage++;
        StaticClass.userCount += 5;
        StaticClass.isBackSelected = true;
        StaticClass.isPageSelected = true;
        StaticClass.ReloadScene();
    }

    public void previousPage()
    {
        StaticClass.currentpage--;
        StaticClass.userCount -= 5;
        StaticClass.isBackSelected = true;
        StaticClass.isPageSelected=true;
        StaticClass.ReloadScene();
    }

    public void resetUserPanelData()
    {
        StaticClass.currentpage = 1;
        StaticClass.lastUserIndex = 0;
        StaticClass.userCount = 5;
    }
    IEnumerator getLatestWords()
    {
        int soundID;
        string[] soundName;
        string[] wordLine;
        string[] dataInput;
        string[] imgExt = { ".jpg", ".jpeg", ".png" };
        string[] arr = { "girl", "boy" };
        WWW www;
        WWWForm form;

        WWW wordsData = new WWW("http://localhost/spell%20wizard%20admin/getUpdates.php");
        yield return wordsData;

        string wordsDataString = wordsData.text;
        Debug.Log("Words Data: " + wordsDataString);

        if (wordsDataString.Contains("|"))
        {
            Debug.Log("Update needed");
            updatePanel.SetActive(true);

            wordLine = wordsDataString.Split(Convert.ToChar(10));
            
            //Download Files
            foreach (string line in wordLine)
            {
                if (line.Contains("|"))
                {
                    dataInput = line.Split('|');

                    //Download Image
                    for (int k = 0; k < 3; k++)
                    {
                        www = new WWW("http://localhost/images/" + dataInput[2] + imgExt[k]);
                        StartCoroutine(downloadFiles(www, Application.persistentDataPath + "/images/" + dataInput[2] + imgExt[k]));
                    }

                    //Get sound_id
                    form = new WWWForm();
                    form.AddField("defIDPost", dataInput[6]);

                    www = new WWW("http://localhost/spell%20wizard%20admin/getSoundID.php", form);
                    yield return www;
                    soundID = Convert.ToInt32(www.text);

                    string[] soundIDArr = { dataInput[4], soundID.ToString() };

                    for (int k=0; k<2; k++)
                    {
                        //Get from db
                        form = new WWWForm();
                        form.AddField("soundIDPost", soundIDArr[k]);

                        www = new WWW("http://localhost/spell%20wizard%20admin/getSoundName.php", form);
                        yield return www;
                        soundName = www.text.Split('|');

                        //Download Audio
                        for (int z = 0; z < 2; z++)
                        {
                            www = new WWW("http://localhost/audios/" + soundName[z] + ".mp3");
                            StartCoroutine(downloadFiles(www, Application.persistentDataPath + "/audios/" + soundName[z] + ".mp3"));
                        }

                        //Add sound to db
                        da = new DataAccess();
                        da.saveDataToDB("INSERT INTO sound (sound_id, girl, boy) VALUES (" + soundIDArr[k] + ", '" + soundName[0] + "', '" + soundName[1] + "');");
                    }

                    //Add word to db
                    da = new DataAccess();
                    da.saveDataToDB("INSERT INTO words (word_id, word, image, difficulty, sound_id, trivia_id, def_id) VALUES ('" + dataInput[0] + "', '" + dataInput[1] + "', '" + dataInput[2] + "', '" + dataInput[3] + "', '" + dataInput[4] + "', '" + dataInput[5] + "', '" + dataInput[6] + "');");

                    //Set status
                    form = new WWWForm();
                    form.AddField("wordIDPost", dataInput[0]);

                    www = new WWW("http://localhost/spell%20wizard%20admin/setStatus.php", form);
                    yield return www;

                    //Update Panel
                    updatePanel.SetActive(false);
                }
            }

            //foreach (string line in wordLine)
            //{
            //    if (line.Contains("|"))
            //    {
            //        dataInput = line.Split('|');

            //        da = new DataAccess();
            //        da.saveDataToDB("INSERT INTO words (word, image, difficulty, sound_id, trivia_id, def_id, status) VALUES (" + dataInput[0] + ", " + dataInput[1] + ", " + dataInput[2] + ", " + dataInput[3] + ", " + dataInput[4] + ", " + dataInput[5] + ", " + dataInput[6] + ");");
            //    }
            //}
        }
    }
    IEnumerator downloadFiles(WWW www, string savePath)
    {
        yield return www;

        //Check if we failed to send
        if (string.IsNullOrEmpty(www.error))
        {
            Debug.Log("Success");

            //Save Image
            saveFiles(savePath, www.bytes);
        }
        else
        {
            Debug.Log("Error: " + www.error);
        }
    }
    void saveFiles(string path, byte[] fileBytes)
    {
        //Create Directory if it does not exist
        if (!Directory.Exists(Path.GetDirectoryName(path)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
        }

        try
        {
            File.WriteAllBytes(path, fileBytes);
            Debug.Log("File saved");
            //Debug.Log("Saved Data to: " + path.Replace("/", "\\"));
        }
        catch (Exception e)
        {
            //Debug.LogWarning("Failed To Save Data to: " + path.Replace("/", "\\"));
            Debug.LogWarning("Error: " + e.Message);
        }
    }
}
