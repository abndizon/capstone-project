using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.IO;
//
public class DraggingScript : MonoBehaviour {

    public VideoPlayer vid;
    public Image letterPrefab;
    public Image character;
    public Image trivColor;
    public Image wordPic;
    public Image triviaPic;
    public Image greetingImage;
    public Image greetingLevel;
    public Image ins1;
    public Image ins2;
    public Image ins3;
    public Image tryAgainImage;
    public Image tapMeImage;
    public Text txt;
    public Text scoreText;
    public Text wordTrivia;
    public Text scoreOnGame;
    public Text fullWordTrivia;
    public Text currentAnswered;
    public Text overText;
    public GameObject wordsGameObject;
    public GameObject gameOverPanel;
    public GameObject instructionPanel;
    public GameObject infoPanel;
    public GameObject videoPanel;
    public GameObject levelDonePanel;
    public GameObject countdownPanel;
    public GameObject optionPanel;
    public Toggle musicToggle;
    public Sprite mbutton;
    public Sprite mbuttondisable;
    GameObject tutorGameObject;
    public Sprite boyCharacterSprite;
    public Sprite girlCongrats;
    public Sprite girlNiceTry;
    public Sprite boyCongrats;
    public Sprite boyNiceTry;
    public Sprite dragIns1;
    public Sprite dragIns2;
    public Sprite dragEasy3;
    public Sprite dragMedium3;
    public Sprite dragHard3;
    public Sprite timedMedium;
    public Sprite timedHard;
    public Sprite startSpriteUntimed;
    public Transform lettersParent;
    public AudioSource wrongAudioSource;
    AudioSource wordSoundSource;
    public AudioSource spellTheWordAudio;
    public AudioClip spellTheWordGirl;
    public AudioClip spellTheWordBoy;
    public Button trivBut;
    public Button nextButton;
    public Button prevButton;
    public Button skipButton;
    public Button wordSoundButton;
    public Button continueButton;
    public Button startButton;
    public Animator girlAnim;
    public Animator boyAnim;
    public Animator clockAnim;
    public VideoClip finishVideo;
    public VideoClip easyVideo;
    public VideoClip mediumVideo;
    public VideoClip hardVideo;
    public VideoClip easyVideoBoy;
    public VideoClip mediumVideoBoy;
    public VideoClip hardVideoBoy;
    GameObject[] placeholders;
    GameObject[] lettersSlotArray;
    GameObject tut;
    char[] alphabetLetters;
    char[] randomizeArray;
    int word_id, trivia_id, i, count, x, maxLength, wordCheck, letters, level, remaining, numOfAns, numOfTotal, sound_id, random;
    string imageFP, audioFP, word, answer, wrongAudio;
    float timer, tryAgainTimer;
    bool scoreSaved,isWordOkay,saved,donePlaying;
    DataAccess da;
    System.Random rnd;
    public static bool isInstructionClose=false;
    public static bool isCountdownDone = false;
    public static float countdownTimer;
    public static bool done;
    //
    void Start () {

        if (countdownTimer==3.30f && !isCountdownDone && isInstructionClose)
        {
            countdownPanel.SetActive(true);
        }

        if (StaticClass.data.selectedChar == 1)
            spellTheWordAudio.clip = spellTheWordBoy;
        else
            spellTheWordAudio.clip = spellTheWordGirl;

        fixInstruction();

        // For music button sprite
        if (GameObject.Find("data GO").GetComponent<AudioSource>().mute)
        {
            musicToggle.image.overrideSprite = mbuttondisable;
            musicToggle.isOn = false;
        }
        else
        {
            musicToggle.image.overrideSprite = mbutton;
            musicToggle.isOn = true;
        }

        da = new DataAccess();
        level = da.getNum("SELECT level FROM level where category=" + StaticClass.category + " and mode='" + StaticClass.mode + "' and user_id='" + StaticClass.data.user_id + "';", "level");

        if (StaticClass.mode == "timed" || !StaticClass.gameStart)
        {
            videoPanel.SetActive(false);
        }
        else if (StaticClass.mode == "untimed")
        { 
            if (StaticClass.difficulty == 1)
            {
                if (StaticClass.data.selectedChar == 1)
                    vid.clip = easyVideoBoy;
                else
                    vid.clip = easyVideo;
            }
            else if (StaticClass.difficulty == 2)
            {
                if (StaticClass.data.selectedChar == 1)
                    vid.clip = mediumVideoBoy;
                else
                    vid.clip = mediumVideo;
            }
            else
            {
                if (StaticClass.data.selectedChar == 1)
                    vid.clip = hardVideoBoy;
                else
                    vid.clip = hardVideo;
            }
            //Set Audio Output to AudioSource
            vid.audioOutputMode = VideoAudioOutputMode.AudioSource;

            //Assign the Audio from Video to AudioSource to be played
            vid.EnableAudioTrack(0, true);
            vid.SetTargetAudioSource(0, vid.GetComponent<AudioSource>());

            //Play Video
            vid.Play();

            //Play Sound
            vid.GetComponent<AudioSource>().Play();

            GameObject.Find("data GO").GetComponent<AudioSource>().mute = true;

            da = new DataAccess();
            if (!da.dataExist("SELECT word_id from dragging where user_id = '" + StaticClass.data.user_id + "' and word_id IN (SELECT word_id from words where difficulty=" + StaticClass.difficulty + ")", "word_id"))
            {
                if (StaticClass.difficulty == 1)
                {
                    da = new DataAccess();
                    if (!da.dataExist("SELECT level FROM level where category=" + StaticClass.category + " and mode='" + StaticClass.mode + "' and user_id='" + StaticClass.data.user_id + "';", "level"))
                    {
                        skipButton.gameObject.SetActive(false);
                    }
                }
                else if (StaticClass.difficulty == 2)
                {
                    if (level == 2)
                    {
                        skipButton.gameObject.SetActive(false);
                    }
                }
                else if (StaticClass.difficulty == 3)
                {
                    if (level == 3)
                    {
                        skipButton.gameObject.SetActive(false);
                    }
                }
            }
        }

        if (StaticClass.mode=="timed" || (StaticClass.difficulty!=level && level!=0))
        {
            wordsGameObject.SetActive(false);
        }

        if (wordsGameObject.activeInHierarchy)
        {
            da = new DataAccess();
            numOfAns = da.getNum("SELECT count(*) as count from dragging where user_id='" + StaticClass.data.user_id + "' and word_id IN (SELECT word_id from words where difficulty=" + StaticClass.difficulty + ")", "count");
            da = new DataAccess();
            numOfTotal = da.getNum("SELECT count(*) as count from words where difficulty=" + StaticClass.difficulty, "count");
            currentAnswered.text = numOfAns + "/" + numOfTotal;
        }

        if (isInstructionClose)
        {
            StartGame();
        }
    }

    void Update()
    {
        Debug.Log("IS" + isCountdownDone);
        if (countdownPanel.activeInHierarchy)
        {
            countdownTimer -= Time.deltaTime;
            if (Convert.ToInt32(countdownTimer) < 0)
            {
                isCountdownDone = true;
            }
        }
        if (isCountdownDone)
        {
            if (!wordSoundSource.isPlaying && !spellTheWordAudio.isPlaying && !wrongAudioSource.isPlaying && done && donePlaying)
            {
                tapMeImage.gameObject.SetActive(true);
            }
            else
            {
                tapMeImage.gameObject.SetActive(false);
            }

            if (!donePlaying)
            {
                if (!countdownPanel.activeInHierarchy)
                {
                    if (spellTheWordAudio != null)
                    {
                        StaticClass.playSound(spellTheWordAudio);
                        donePlaying = true;
                    }

                }
            }
            if (!(TimerScript.gameOver) && !TimerScript.isPaused && !levelDonePanel.activeInHierarchy)
            {
                //Initialization
                count = 0;
                answer = "";

                if (!done)
                {
                    timer -= Time.deltaTime;
                    if (Convert.ToInt32(timer)==0)
                    {
                        StaticClass.playSound(wordSoundSource);
                        done = true;
                    }
                }

                if (tryAgainImage.gameObject.activeInHierarchy)
                {
                    tryAgainTimer -= Time.deltaTime;
                    if (Convert.ToInt32(tryAgainTimer)==0)
                    {
                        tryAgainTimer = 2.5f;
                        tryAgainImage.gameObject.SetActive(false);
                    }
                }

                if (StaticClass.mode=="timed")
                {
                    if (Convert.ToInt32(TimerScript.seconds) <= 10 && Convert.ToInt32(TimerScript.minutes) == 0)
                    {
                        clockAnim.SetBool("IsNear", true);
                    }

                    if (Convert.ToInt32(TimerScript.seconds) < 10)
                        txt.text = TimerScript.minutes + ":0" + Convert.ToInt32(TimerScript.seconds).ToString();
                    else
                        txt.text = TimerScript.minutes + ":" + Convert.ToInt32(TimerScript.seconds).ToString();
                    scoreOnGame.text = Convert.ToInt32(TimerScript.score).ToString();
                }

                //Check if word sound is playing, if yes=play tutor animation
                if (StaticClass.data.selectedChar == 2)
                {
                    if (wordSoundSource.isPlaying || wrongAudioSource.isPlaying || spellTheWordAudio.isPlaying)
                        girlAnim.SetBool("IsSpeaking", true);
                    else
                        girlAnim.SetBool("IsSpeaking", false);
                }
                else
                {
                    if (wordSoundSource.isPlaying || wrongAudioSource.isPlaying || spellTheWordAudio.isPlaying)
                        boyAnim.SetBool("IsSpeaking", true);
                    else
                        boyAnim.SetBool("IsSpeaking", false);
                }

                //Count if placeholders has letters
                for (i = 0; i < word.Length; i++)
                {
                    if (placeholders[i].transform.childCount > 0)
                    {
                        count++;
                    }
                }
                Debug.Log("Count: " + count);

                //Gets answer if all placeholders has letters
                if (count == word.Length)
                {
                    for (i = 0; i < word.Length; i++)
                    {
                        answer += placeholders[i].transform.GetChild(0).gameObject.name.ToLower();
                    }
                    Debug.Log("Answer: " + answer);

                    //Check if answer is right
                    if (answer == word)//
                    { 
                        da = new DataAccess();
                        if (StaticClass.mode == "timed")
                        {
                            TimerScript.deduct++;
                            TimerScript.score++;
                            TimerScript.triviaWord[TimerScript.index] = word;
                            TimerScript.fullTrivia[TimerScript.index] = da.getTrivia(trivia_id);
                            TimerScript.wordPic[TimerScript.index] = imageFP;
                            TimerScript.audioWord[TimerScript.index] = audioFP;
                            TimerScript.tempWords[TimerScript.arrayIndex] = word;
                            TimerScript.index++;
                            TimerScript.arrayIndex++;
                        }
                        else
                        {
                            StaticClass.gameStart = false;
                            numOfAns++;
                            currentAnswered.text = numOfAns + "/" + numOfTotal;

                            if (!saved)
                            {
                                da = new DataAccess();
                                da.saveDataToDB("INSERT INTO dragging (word_id, user_id) VALUES (" + word_id + ", '" + StaticClass.data.user_id + "');");
                                saved = true;
                                Debug.Log("Saved Word Done");
                            }
                            if (remaining==1)
                            {
                                // Check if score passed the next level
                                if (StaticClass.difficulty == 1)
                                {
                                    da = new DataAccess();
                                    if (!da.dataExist("SELECT level FROM level where category=" + StaticClass.category + " and mode='" + StaticClass.mode + "' and user_id='" + StaticClass.data.user_id + "';", "level"))
                                    {
                                        da = new DataAccess();
                                        da.saveDataToDB("INSERT INTO level (level, mode, category, user_id) VALUES ( 2, '" + StaticClass.mode + "', " + StaticClass.category + ", '" + StaticClass.data.user_id + "');");
                                        levelDonePanel.SetActive(true);
                                    }

                                }
                                else if (StaticClass.difficulty == 2)
                                {
                                    if (level == 2)
                                    {
                                        da = new DataAccess();
                                        da.saveDataToDB("UPDATE level SET level = 3 WHERE category=" + StaticClass.category + " and mode='" + StaticClass.mode + "' and user_id='" + StaticClass.data.user_id + "';");
                                        levelDonePanel.SetActive(true);
                                    }
                                }
                                else
                                {
                                    if (level == 3)
                                    {
                                        da = new DataAccess();
                                        da.saveDataToDB("UPDATE level SET level = 4 WHERE category=" + StaticClass.category + " and mode='" + StaticClass.mode + "' and user_id='" + StaticClass.data.user_id + "';");
                                        levelDonePanel.SetActive(true);
                                    }
                                    //videoPanel.GetComponentInChildren<VideoPlayer>().clip = finishVideo;
                                }
                            }
                        }
                        if (!levelDonePanel.activeInHierarchy)
                        {
                            random = rnd.Next(0, 4);
                            Debug.Log("RandomNumber: " + random);
                            GameObject.Find("Praise Sprite").GetComponent<SpriteRenderer>().sprite= Resources.Load("praise_pics/" + StaticClass.praisePics[random], typeof(Sprite)) as Sprite;
                            GameObject.Find("Praise Sprite").GetComponent<Animator>().SetBool("IsPraise", true);
                            GameObject.Find("Praise Sprite").GetComponent<AudioSource>().Play();
                            StaticClass.ReloadScene();
                        }
                    }
                    //if answer is wrong
                    else
                    {
                        for (i = 0; i < word.Length; i++)
                        {
                            GameObject temp = placeholders[i].transform.GetChild(0).gameObject;
                            for (x = 0; x < letters; x++)
                            {
                                if (lettersSlotArray[x].transform.childCount == 0)
                                {
                                    temp.transform.SetParent(lettersSlotArray[x].transform);
                                } 
                            }
                        }
                        tryAgainTimer = 2.5f;
                        wrongAudioSource.clip = Resources.Load(wrongAudio, typeof(AudioClip)) as AudioClip;
                        wrongAudioSource.Play();
                        tryAgainImage.gameObject.SetActive(true);
                    }
                }
            }

            else if (TimerScript.isPaused)
            {
                stopWordSound();
                clockAnim.SetBool("IsNear", false);
            }

            else if (TimerScript.gameOver) //if GameOver
            {
                gameOverPanel.SetActive(true);
                clockAnim.SetBool("IsNear", false);
                if (TimerScript.score == 0)
                {
                    trivBut.enabled = false;
                    trivColor.color = new Color32(139, 139, 139, 255);
                }

                if (TimerScript.score > 1)
                    scoreText.text = "You spelled " + TimerScript.score + " words correctly!";
                else if (TimerScript.score <= 1)
                    scoreText.text = "You spelled " + TimerScript.score + " word correctly!";

                //Check if score passed the next level
                if (StaticClass.difficulty == 1)
                {
                    da = new DataAccess();
                    if (!da.dataExist("SELECT level FROM level where category=" + StaticClass.category + " and mode='" + StaticClass.mode + "' and user_id='" + StaticClass.data.user_id + "';", "level"))
                    {
                        if (TimerScript.score >= 12)
                        {
                            if (StaticClass.data.selectedChar == 1)
                                greetingImage.sprite = boyCongrats;
                            else
                                greetingImage.sprite = girlCongrats;
                            da = new DataAccess();
                            da.saveDataToDB("INSERT INTO level (level, mode, category, user_id) VALUES ( 2, '" + StaticClass.mode + "', " + StaticClass.category + ", '" + StaticClass.data.user_id + "');");
                        }
                        else
                        {
                            if (StaticClass.data.selectedChar == 1)
                                greetingImage.sprite = boyNiceTry;
                            else
                                greetingImage.sprite = girlNiceTry;
                            continueButton.enabled = false;
                            continueButton.GetComponent<Image>().color = new Color32(139, 139, 139, 255);
                        }

                    }

                }
                else if (StaticClass.difficulty == 2)
                {
                    if (level == 2)
                    {
                        if (TimerScript.score >= 10)
                        {
                            if (StaticClass.data.selectedChar == 1)
                                greetingImage.sprite = boyCongrats;
                            else
                                greetingImage.sprite = girlCongrats;
                            da = new DataAccess();
                            da.saveDataToDB("UPDATE level SET level = 3 WHERE category=" + StaticClass.category + " and mode='" + StaticClass.mode + "' and user_id='" + StaticClass.data.user_id + "';");
                        }
                        else
                        {
                            if (StaticClass.data.selectedChar == 1)
                                greetingImage.sprite = boyNiceTry;
                            else
                                greetingImage.sprite = girlNiceTry;
                            continueButton.enabled = false;
                            continueButton.GetComponent<Image>().color = new Color32(139, 139, 139, 255);
                        }
                    }
                }
                else
                {
                    if (level == 3)
                    {
                        if (TimerScript.score >= 8)
                        {
                            if (StaticClass.data.selectedChar == 1)
                                greetingImage.sprite = boyCongrats;
                            else
                                greetingImage.sprite = girlCongrats;
                        }
                        else
                        {
                            if (StaticClass.data.selectedChar == 1)
                                greetingImage.sprite = boyNiceTry;
                            else
                                greetingImage.sprite = girlNiceTry;
                        }
                    }
                    continueButton.enabled = false;
                    continueButton.GetComponent<Image>().color = new Color32(139, 139, 139, 255);
                }

                //Save score to Database
                if (!scoreSaved)
                {
                    da = new DataAccess();
                    da.saveDataToDB("INSERT INTO score (`score`, `category`, `user_id`, difficulty, date) VALUES (" + TimerScript.score + ", " + StaticClass.category + ", '" + StaticClass.data.user_id + "', " + StaticClass.difficulty + ", '" + DateTime.Now.ToString("M/dd/yyyy") + "');");
                    scoreSaved = true;
                }
                Destroy(GameObject.FindGameObjectWithTag("timer"));

                if (TimerScript.triviaShown)
                {
                    wordTrivia.text = TimerScript.triviaWord[TimerScript.indexInTriviaShown].ToUpper();
                    fullWordTrivia.text = TimerScript.fullTrivia[TimerScript.indexInTriviaShown];

                    triviaPic.overrideSprite = Resources.Load("word_pics/" + TimerScript.wordPic[TimerScript.indexInTriviaShown], typeof(Sprite)) as Sprite;

                    if (!Resources.Load("word_pics/" + TimerScript.wordPic[TimerScript.indexInTriviaShown]))
                    {
                        wordPic.overrideSprite = LoadImage(Application.persistentDataPath + "/images/" + TimerScript.wordPic[TimerScript.indexInTriviaShown]);
                    }

                    wordSoundButton.GetComponent<AudioSource>().clip = Resources.Load("word_audio/" + TimerScript.audioWord[TimerScript.indexInTriviaShown], typeof(AudioClip)) as AudioClip;

                    if (!Resources.Load("word_audio/" + TimerScript.audioWord[TimerScript.indexInTriviaShown]))
                    {
                        StartCoroutine(LoadClip(Application.persistentDataPath + "/audios/" + TimerScript.audioWord[TimerScript.indexInTriviaShown] + ".wav", wordSoundSource));
                    }

                    overText.text = (TimerScript.indexInTriviaShown + 1) + "/" + TimerScript.score; 

                    if (TimerScript.triviaWord[TimerScript.indexInTriviaShown + 1] != null)
                    {
                        nextButton.enabled = true;
                        nextButton.gameObject.SetActive(true);
                    }
                    else
                    {
                        nextButton.enabled = false;
                        nextButton.gameObject.SetActive(false);
                    }

                    if (TimerScript.indexInTriviaShown == 0)
                    {
                        prevButton.enabled = false;
                        prevButton.gameObject.SetActive(false);
                    }
                    else
                    {
                        prevButton.enabled = true;
                        prevButton.gameObject.SetActive(true);
                    }
                }
            }
        }
        if (StaticClass.mode == "untimed")
        {
            if (StaticClass.gameStart)
            {
                if (videoPanel.activeInHierarchy)
                {
                    Debug.Log("vid Time: " + Convert.ToInt32(vid.time));
                    Debug.Log("Vid Length: " + Convert.ToInt32(vid.clip.length));
                    if (Convert.ToInt32(vid.time) == Convert.ToInt32(vid.clip.length))
                    {
                        GameMusic();
                        vid.clip = null;
                        videoPanel.SetActive(false);
                    }
                }
            }
        }
    }

    public void StartGame()
    {
        //Initialization
        isInstructionClose = true;
        instructionPanel.SetActive(false);
        lettersSlotArray = new GameObject[11];
        maxLength = 9;
        done = false;
        donePlaying = false;
        timer = 2.0f;

        if (StaticClass.mode=="timed")
        {
            countdownTimer = 3.30f;
            scoreSaved = false;
            gameOverPanel.SetActive(false);

            if (Convert.ToInt32(TimerScript.seconds) < 10)
                txt.text = TimerScript.minutes + ":0" + Convert.ToInt32(TimerScript.seconds).ToString();
            else
                txt.text = TimerScript.minutes + ":" + Convert.ToInt32(TimerScript.seconds).ToString();
        }
        else
        {
            infoPanel.SetActive(false);
            saved = false;
            isCountdownDone = true;
        }

        //Change character based on playerdata
        if (StaticClass.data.selectedChar == 1)
        {
            tut = GameObject.Find("GirlTutor");
            wordSoundSource = GameObject.Find("BoyTutor").GetComponent<AudioSource>();
            tutorGameObject = GameObject.Find("BoyTutor");
            greetingLevel.sprite = boyCongrats;
        }
        else
        {
            tut = GameObject.Find("BoyTutor");
            wordSoundSource = GameObject.Find("GirlTutor").GetComponent<AudioSource>();
            tutorGameObject = GameObject.Find("GirlTutor");
            greetingLevel.sprite = girlCongrats;
        }
        tut.SetActive(false);

        //Get tutor data
        da = new DataAccess();
        wrongAudio = da.getString("SELECT * FROM tutor WHERE tutor_id=" + StaticClass.data.selectedChar + ";","try_audio");

        //Count how many words are not yet answered
        da = new DataAccess();

        if (StaticClass.mode == "timed")
        {
            remaining = da.getNum("SELECT count (*) as count FROM words WHERE difficulty=" + StaticClass.difficulty, "count") - TimerScript.deduct;
        }
        else
        {
            remaining = da.getNum("SELECT count(*) as count FROM words WHERE word_id NOT IN (SELECT word_id FROM dragging WHERE user_id='" + StaticClass.data.user_id + "') and difficulty=" + StaticClass.difficulty, "count");
        }
        Debug.Log("Remaining words: " + remaining);

        //If all words are answered, reset the data
        if (remaining <= 0)
        {
            if (StaticClass.mode == "timed")
            {
                TimerScript.tempWords=new string[50];
                TimerScript.arrayIndex = 0;
                TimerScript.deduct = 0;
            }
            else
            {
                da = new DataAccess();
                da.saveDataToDB("DELETE FROM dragging where user_id = '" + StaticClass.data.user_id + "' and word_id IN(select word_id from words where difficulty = " + StaticClass.difficulty + ")");
            }
        }

        //Get word and data from database
        if (StaticClass.mode == "timed")
        {
            while (!isWordOkay)
            {
                da = new DataAccess();
                da.getWords(ref word_id, ref trivia_id, ref imageFP, ref sound_id, ref word, "SELECT * FROM words WHERE difficulty=" + StaticClass.difficulty + " order by random() limit 1");
                Debug.Log("Timed word: " + word + " word_id: " + word_id + " trivia_id: " + trivia_id + " imageFP: " + imageFP + " sound_id: " + sound_id);

                wordCheck = 0;
                //Check if it is answered in the game
                for (i = 0; i < 50; i++)
                {
                    if (word == TimerScript.tempWords[i])
                        wordCheck++;
                }

                if (wordCheck == 0)
                    isWordOkay = true;
            }
        }
        else
        {
            da = new DataAccess();
            da.getWords(ref word_id, ref trivia_id, ref imageFP, ref sound_id, ref word, "SELECT * FROM words WHERE word_id NOT IN (SELECT word_id FROM dragging WHERE user_id='" + StaticClass.data.user_id + "') and difficulty=" + StaticClass.difficulty + " order by random() limit 1");
        }
        Debug.Log("Okay word: " + word + " word_id: " + word_id + " trivia_id: " + trivia_id + " imageFP: " + imageFP + " sound_id: " + sound_id);

        //Place how many slots based on word length
        for (i = 0; i < (9 - word.Length); i++)
        {
            GameObject slot = GameObject.Find("Slot" + maxLength);
            slot.SetActive(false);
            GameObject img = GameObject.Find("Image" + maxLength);
            img.SetActive(false);
            maxLength--;
        }

        //Put letters into array
        alphabetLetters = new char[11];

        for (i = 0; i < word.Length; i++)
        {
            alphabetLetters[letters] = Char.ToUpper(word[i]);
            letters++;
        }

        //Get extra letters
        da = new DataAccess();

        if (StaticClass.difficulty == 1)
        {
            da.getExtraLetters(7 - word.Length).CopyTo(alphabetLetters, letters);
            letters = 7;
        }
        else if (StaticClass.difficulty == 2)
        {
            da.getExtraLetters(9 - word.Length).CopyTo(alphabetLetters, letters);
            letters = 9;
        }
        else
        {
            da.getExtraLetters(11 - word.Length).CopyTo(alphabetLetters, letters);
            letters = 11;
        }

        //Randomize alphabet array and put it into new array
        randomizeArray = new char[letters];
        rnd = new System.Random();

        for (i = 0; i < letters; i++)
        {
            randomizeArray[i] = alphabetLetters[i];
        }
        randomizeArray = randomizeArray.OrderBy(x => rnd.Next()).ToArray();

        //Place letter slot based on letters
        for (i = 11; i > letters; i--)
        {
            GameObject.Find("LetterSlot" + i).SetActive(false);
        }

        //Put letters on right place
        for (i = 0; i < letters; i++)
        {
            Debug.Log("Letters: " + alphabetLetters[i]);

            lettersSlotArray[i] = GameObject.Find("LetterSlot" + (i + 1));
            lettersSlotArray[i].transform.GetChild(0).name = randomizeArray[i].ToString();
            lettersSlotArray[i].transform.GetChild(0).GetComponent<Image>().overrideSprite = Resources.Load(randomizeArray[i].ToString(), typeof(Sprite)) as Sprite;
        }

        Debug.Log("Letters: " + letters);

        //Change word picture
        wordPic.overrideSprite = Resources.Load("word_pics/" + imageFP, typeof(Sprite)) as Sprite;

        if (!Resources.Load("word_pics/" + imageFP))
        {
            wordPic.overrideSprite = LoadImage(Application.persistentDataPath + "/images/" + imageFP);
        }

        wordPic.preserveAspect = true;

        //Apply word audio
        da = new DataAccess();
        if (StaticClass.data.selectedChar == 1)
        {
            audioFP = da.getString("SELECT boy FROM sound where sound_id=" + sound_id, "boy");
        }
        else
        {
            audioFP = da.getString("SELECT girl FROM sound where sound_id=" + sound_id, "girl");
        }
        wordSoundSource.clip = Resources.Load("word_audio/" + audioFP, typeof(AudioClip)) as AudioClip;

        if (!Resources.Load("word_audio/" + audioFP))
        {
            StartCoroutine(LoadClip(Application.persistentDataPath + "/audios/" + audioFP + ".wav", wordSoundSource));
        }

        //Put into gameobject array the placeholders
        placeholders = GameObject.FindGameObjectsWithTag("placeholder");
        Array.Sort(placeholders, (x, y) => String.Compare(x.name, y.name));
    }

    public void stopWordSound()
    {
        if (wordSoundSource.isPlaying || spellTheWordAudio.isPlaying || wrongAudioSource.isPlaying)
        {
            wordSoundSource.Stop();
            spellTheWordAudio.Stop();
            wrongAudioSource.Stop();
            if (StaticClass.data.selectedChar == 2)
            {
                girlAnim.SetBool("IsSpeaking", false);
            }
            else
            {
                boyAnim.SetBool("IsSpeaking", false);
            }
        }
    }
    public void setDoneToFalse()
    {
        Debug.Log("IsCLicked");
        done = false;
    }

    public void GameMusic()
    {
        if (musicToggle.isOn)
        {
            GameObject.Find("data GO").GetComponent<AudioSource>().mute = false;
            musicToggle.image.overrideSprite = mbutton;
        }
        else
        {
            GameObject.Find("data GO").GetComponent<AudioSource>().mute = true;
            musicToggle.image.overrideSprite = mbuttondisable;
        }
    }
    public void fixInstruction()
    {
        if (StaticClass.mode=="untimed")
        {
            ins1.sprite = dragIns1;
            ins2.sprite = dragIns2;
            startButton.GetComponent<Image>().sprite = startSpriteUntimed;

            if (StaticClass.difficulty==1)
            {
                ins3.sprite = dragEasy3;
            }
            else if (StaticClass.difficulty==2)
            {
                ins3.sprite = dragMedium3;
            }
            else
            {
                ins3.sprite = dragHard3;
            }
        }
        else if (StaticClass.mode == "timed")
        {
            if (StaticClass.difficulty == 2)
                ins3.sprite = timedMedium;
            else if (StaticClass.difficulty == 3)
                ins3.sprite = timedHard;
        }
    }
    public void resetCountdown()
    {
      if (StaticClass.mode=="timed")
      {
            CountdownScript.lifetime = 3.30f;
            countdownTimer = 3.30f;
            isCountdownDone = false;

            TimerScript.minutes = 2;
      }
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


