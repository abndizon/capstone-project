using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
//
public class TutorialScript : MonoBehaviour {

    public Image wordPic;
    public Text definitionText;
    public GameObject lettersPanel;
    public Button backButton;
    public Button nextButton;
    DataAccess da;
    Image tilePic;
    AudioSource wordSound, letterSound, defSound;
    Animator animObj;
    float timer;
    int i, tutorialLetterIndex, sound_id2;
    string oneWord, letterFP, audioFP, defFP;
    bool wordDonePlaying, defDonePlaying, wordDonePlaying2;

    void Start()
    {

        if (StaticClass.isTutorialStart)
        {
            //Get number of words
            da = new DataAccess();
            StaticClass.numOfWords = da.getNum("SELECT count (*) as count FROM words", "count");

            //Initialization
            StaticClass.word_id = new int[StaticClass.numOfWords];
            StaticClass.def_id = new int[StaticClass.numOfWords];
            StaticClass.imageFP = new string[StaticClass.numOfWords];
            StaticClass.sound_id = new int[StaticClass.numOfWords];
            StaticClass.words = new string[StaticClass.numOfWords];

            //Get all words
            da = new DataAccess();
            da.getWordsArray(ref StaticClass.word_id, ref StaticClass.def_id, ref StaticClass.imageFP, ref StaticClass.sound_id, ref StaticClass.words, "SELECT * FROM words order by random()");
        }

        //Initialization
        wordDonePlaying = false;
        tutorialLetterIndex = 0;
        timer = 0.0f;

        //Put letters on right place
        for (i = 9; i > StaticClass.words[StaticClass.tutorialIndex].Length; i--)
        {
            GameObject.Find("Slot" + i).SetActive(false);
        }

        for (i = 1; i <= StaticClass.words[StaticClass.tutorialIndex].Length; i++)
        {
            oneWord = StaticClass.words[StaticClass.tutorialIndex];
            tilePic = GameObject.Find("Slot" + i).GetComponent<Image>();
            tilePic.overrideSprite = Resources.Load("jumbled_letters/" + oneWord[i - 1].ToString().ToUpper(), typeof(Sprite)) as Sprite;
        }

        //Change word picture
        wordPic.overrideSprite = Resources.Load("word_pics/" + StaticClass.imageFP[StaticClass.tutorialIndex], typeof(Sprite)) as Sprite;

        if (!Resources.Load("word_pics/" + StaticClass.imageFP[StaticClass.tutorialIndex]))
        {
            wordPic.overrideSprite = LoadImage(Application.persistentDataPath + "/images/" + StaticClass.imageFP[StaticClass.tutorialIndex]);
        }

        wordPic.preserveAspect = true;

        //Change definition
        da = new DataAccess();
        definitionText.text = da.getString("SELECT def FROM definition WHERE def_id=" + StaticClass.def_id[StaticClass.tutorialIndex], "def");

        //Apply word audio
        da = new DataAccess();
        if (StaticClass.data.selectedChar == 1)
        {
            audioFP = da.getString("SELECT boy FROM sound where sound_id=" + StaticClass.sound_id[StaticClass.tutorialIndex], "boy");
        }
        else
        {
            audioFP = da.getString("SELECT girl FROM sound where sound_id=" + StaticClass.sound_id[StaticClass.tutorialIndex], "girl");
        }
        wordSound = wordPic.GetComponent<AudioSource>();
        wordSound.clip = Resources.Load("word_audio/" + audioFP, typeof(AudioClip)) as AudioClip;

        if (!Resources.Load("word_audio/" + audioFP))
        {
            StartCoroutine(LoadClip(Application.persistentDataPath + "/audios/" + audioFP + ".wav", wordSound));
        }

        StaticClass.playSound(wordSound);

        //Apply definition sound
        da = new DataAccess();
        if (StaticClass.data.selectedChar == 1)
        {
            defFP = da.getString("SELECT boy FROM sound WHERE sound_id IS (SELECT sound_id FROM definition WHERE def_id=" + StaticClass.def_id[StaticClass.tutorialIndex] + ")", "boy");
        }
        else
        {
            defFP = da.getString("SELECT girl FROM sound WHERE sound_id IS (SELECT sound_id FROM definition WHERE def_id=" + StaticClass.def_id[StaticClass.tutorialIndex] + ")", "girl");
        }
        defSound = definitionText.GetComponent<AudioSource>();
        defSound.clip = Resources.Load("word_audio/" + defFP, typeof(AudioClip)) as AudioClip;

        if (!Resources.Load("word_audio/" + defFP))
        {
            LoadClip(Application.persistentDataPath + "/audios/" + defFP + ".wav", defSound);
        }

        timer = 1.5f; //((oneWord.Length + 1) * 2.0f);

        //Check buttons
        if (StaticClass.tutorialIndex == 0)
        {
            backButton.enabled = false;
            backButton.gameObject.SetActive(false);
        }
        else if (StaticClass.tutorialIndex == (StaticClass.numOfWords - 1))
        {
            nextButton.enabled = false;
            nextButton.gameObject.SetActive(false);
        }
        lettersPanel.GetComponent<HorizontalLayoutGroup>().enabled = false;

        letterSound = GameObject.Find("Slot" + (tutorialLetterIndex + 1)).GetComponent<AudioSource>();
    }

    void Update()
    {
        Debug.Log("Tutorial Letter Index: " + tutorialLetterIndex);
        if (!wordDonePlaying)
        {
            if (!wordSound.isPlaying)
            {
                wordDonePlaying = true;
            }
        }
        else
        {
            if (tutorialLetterIndex < oneWord.Length)
            {
                if (Convert.ToInt32(timer)==0)
                {
                    letterSound = GameObject.Find("Slot" + (tutorialLetterIndex + 1)).GetComponent<AudioSource>();
                    da = new DataAccess();
                    sound_id2 = da.getNum("SELECT sound_id FROM alphabet where letter = '" + oneWord[tutorialLetterIndex].ToString().ToUpper() + "'", "sound_id");
                    da = new DataAccess();
                    if (StaticClass.data.selectedChar == 1)
                    {
                        letterFP = da.getString("SELECT boy FROM sound where sound_id=" + sound_id2, "boy");
                    }
                    else
                    {
                        letterFP = da.getString("SELECT girl FROM sound where sound_id=" + sound_id2, "girl");
                    }
                    letterSound.clip = Resources.Load("letter_audio/" + letterFP, typeof(AudioClip)) as AudioClip;

                    animObj = GameObject.Find("Slot" + (tutorialLetterIndex + 1)).GetComponent<Animator>();
                    animObj.SetBool("PlayAnim", true);

                    StaticClass.playSound(letterSound);
                    tutorialLetterIndex++;
                    timer = 1.5f;
                }
                
            }
            else if (Convert.ToInt32(timer)==0)
            {
                if (!wordDonePlaying2)
                {
                    if (!wordSound.isPlaying)
                    {
                        StaticClass.playSound(wordSound);
                        wordDonePlaying2 = true;
                        timer = 1.5f;
                    }
                }
                else
                {
                    if (!defDonePlaying)
                    {
                        if (!defSound.isPlaying)
                        {
                            StaticClass.playSound(defSound);
                            defDonePlaying = true;
                        }
                    }
                }
            }
            timer -= Time.deltaTime;
        }
    }

    public void playAgainSound()
    {
        StaticClass.isTutorialStart = false;
        StaticClass.ReloadScene();
    }

    IEnumerator LoadClip(string path, AudioSource auSource)
    {
        if (File.Exists(path))
        {
            Debug.Log("INSIDE!!");
            WWW www = new WWW(path);
            yield return www;

            AudioClip clip = www.GetAudioClip(false, false, AudioType.WAV);

            auSource.clip = clip;
        }
    }
    Sprite LoadImage(string path)
    {
        string[] fileExt = { ".png", ".jpeg", ".jpg" };
        Sprite sprite = null;
        string filePath;

        for (int s = 0; s < 3; s++)
        {
            filePath = path + fileExt[s];

            if (File.Exists(filePath))
            {
                // read image and store in a byte array
                byte[] byteArray = File.ReadAllBytes(filePath);

                //create a texture and load byte array to it
                Texture2D sampleTexture = new Texture2D(2, 2);

                // the size of the texture will be replaced by image size
                bool isLoaded = sampleTexture.LoadImage(byteArray);

                if (isLoaded)
                {
                    sprite = Sprite.Create(sampleTexture, new Rect(0, 0, sampleTexture.width, sampleTexture.height), new Vector2(0.5f, 0.5f));
                }
            }

        }
        return sprite;
    }
}
