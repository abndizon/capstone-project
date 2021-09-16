using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System;
using System.IO;

public class JumbledScript : MonoBehaviour {

    public GameObject letterPanel;
    public GameObject gameOverPanel;
    public GameObject instructionPanel;
    public GameObject videoPanel;
    public GameObject infoPanel;
    public GameObject levelDonePanel;
    public GameObject optionPanel;
    public GameObject countdownPanel;
    public GameObject wordsGameObject;
    public VideoPlayer vid;
    public Toggle musicToggle;
    public Sprite mbutton;
    public Sprite mbuttondisable;
    GameObject tutorGameObject;
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
    public Sprite boyCharacterSprite;
    public Sprite girlCongrats;
    public Sprite girlNiceTry;
    public Sprite boyCongrats;
    public Sprite boyNiceTry;
    public Sprite jumbledIns1;
    public Sprite jumbledIns2;
    public Sprite jumbledEasy3;
    public Sprite jumbledMedium3;
    public Sprite jumbledHard3;
    public Sprite timedMedium;
    public Sprite timedHard;
    public Sprite startSpriteUntimed;
    public AudioSource wrongAudioSource;
    public AudioSource spellTheWordAudio;
    public AudioClip spellTheWordBoy;
    public AudioClip spellTheWordGirl;
    AudioSource wordSoundSource;
    public Text timeRemaining;
    public Text scoreText;
    public Text wordTrivia;
    public Text scoreOnGame;
    public Text fullWordTrivia;
    public Text overText;
    public Text currentAnswered;
    public Button nextButton;
    public Button prevButton;
    public Button trivBut;
    public Button wordSoundButton;
    public Button skipButton;
    public Button continueButton;
    public Button startButton;
    public Animator girlAnim;
    public Animator boyAnim;
    public Animator clockAnim;
    public VideoClip easyVideo;
    public VideoClip mediumVideo;
    public VideoClip hardVideo;
    public VideoClip easyVideoBoy;
    public VideoClip mediumVideoBoy;
    public VideoClip hardVideoBoy;
    public VideoClip finishVideo;
    DataAccess da;
    Image tilePic;
    System.Random rnd;
    int count,i,word_id,trivia_id,wordCheck,level, numOfAns, numOfTotal, sound_id, random;
    string word, imageFP, audioFP,answer, wrongAudio;
    float timer, tryAgainTimer;
    int[] randomizeIndex;
    bool isNotRandom,saved,scoreSaved,isWordOkay,donePlaying;
    public static bool isInstructionClose=false;
    public static bool isCountdownDone = false;
    public static float countdownTimer;
    public static bool done;

    void Awake()
    {
        if (countdownTimer == 3.30f && !isCountdownDone && isInstructionClose)
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
            if (!da.dataExist("SELECT word_id from jumbled where user_id = '" + StaticClass.data.user_id + "' and word_id IN (SELECT word_id from words where difficulty=" + StaticClass.difficulty + ")", "word_id"))
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
                else
                {
                    if (level == 3)
                    {
                        skipButton.gameObject.SetActive(false);
                    }
                }
            }
        }

        if (StaticClass.mode == "timed" || (StaticClass.difficulty!=level && level!=0))
        {
            wordsGameObject.SetActive(false);
        }

        if (wordsGameObject.activeInHierarchy)
        {
            da = new DataAccess();
            numOfAns = da.getNum("SELECT count(*) as count from jumbled where user_id='" + StaticClass.data.user_id + "' and word_id IN (SELECT word_id from words where difficulty=" + StaticClass.difficulty + ")", "count");
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
                if (!done)
                {
                    timer -= Time.deltaTime;
                    if (Convert.ToInt32(timer) == 0)
                    {
                        StaticClass.playSound(wordSoundSource);
                        done = true;
                    }
                }

                if (!wordSoundSource.isPlaying && !spellTheWordAudio.isPlaying && !wrongAudioSource.isPlaying && done && donePlaying)
                {
                    tapMeImage.gameObject.SetActive(true);
                }
                else
                {
                    tapMeImage.gameObject.SetActive(false);
                }

                if (tryAgainImage.gameObject.activeInHierarchy)
                {
                    tryAgainTimer -= Time.deltaTime;
                    if (Convert.ToInt32(tryAgainTimer) == 0)
                    {
                        tryAgainTimer = 2.5f;
                        tryAgainImage.gameObject.SetActive(false);
                    }
                }

                if (StaticClass.mode=="timed")
                {
                    if (Convert.ToInt32(TimerScript.seconds) < 10)
                        timeRemaining.text = TimerScript.minutes + ":0" + Convert.ToInt32(TimerScript.seconds).ToString();
                    else
                        timeRemaining.text = TimerScript.minutes + ":" + Convert.ToInt32(TimerScript.seconds).ToString();

                    if (Convert.ToInt32(TimerScript.seconds) <= 10 && Convert.ToInt32(TimerScript.minutes) == 0)
                    {
                        clockAnim.SetBool("IsNear", true);
                    }
                    scoreOnGame.text = Convert.ToInt32(TimerScript.score).ToString();
                }
                Debug.Log("Mouse Click: " + TimerScript.mouseClick);

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
            }

            else if (TimerScript.isPaused)
            {
                if (wordSoundSource.isPlaying || wrongAudioSource.isPlaying || spellTheWordAudio.isPlaying)
                {
                    wordSoundSource.Stop();
                    wrongAudioSource.Stop();
                    spellTheWordAudio.Stop();
                    if (StaticClass.data.selectedChar == 2)
                    {
                        girlAnim.SetBool("IsSpeaking", false);
                    }
                    else
                    {
                        boyAnim.SetBool("IsSpeaking", false);
                    }
                }
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
                    Debug.Log(TimerScript.triviaWord[TimerScript.indexInTriviaShown]);
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
        isNotRandom = true;
        isWordOkay = false;
        instructionPanel.SetActive(false);
        timer = 2.0f;
        donePlaying = false;
        done = false;

        if (StaticClass.mode=="timed")
        {
            countdownTimer = 3.30f;
            scoreSaved = false;
            if (Convert.ToInt32(TimerScript.seconds) < 10)
                timeRemaining.text = TimerScript.minutes + ":0" + Convert.ToInt32(TimerScript.seconds).ToString();
            else
                timeRemaining.text = TimerScript.minutes + ":" + Convert.ToInt32(TimerScript.seconds).ToString();
            gameOverPanel.SetActive(false);
        }
        else
        {
            infoPanel.SetActive(false);
            saved = false;
            isCountdownDone = true;
        }

        //Change character based on playerdata
        GameObject tut;
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
        wrongAudio = da.getString("SELECT * FROM tutor WHERE tutor_id=" + StaticClass.data.selectedChar + ";", "try_audio");

        //Count how many words are not yet answered
        da = new DataAccess();

        if (StaticClass.mode=="timed")
        {
            count = da.getNum("SELECT count (*) as count FROM words WHERE difficulty=" + StaticClass.difficulty, "count") - TimerScript.deduct;
        }
        else
        {
            count = da.getNum("SELECT count(*) as count FROM words WHERE word_id NOT IN (SELECT word_id FROM jumbled WHERE user_id='" + StaticClass.data.user_id + "') and difficulty=" + StaticClass.difficulty, "count");
        }
        Debug.Log("Remaining words: " + count);

        //If all words are answered, reset the data
        if (count <= 0)
        {
            if (StaticClass.mode=="timed")
            {
                TimerScript.tempWords = new string[50];
                TimerScript.arrayIndex = 0;
                TimerScript.deduct = 0;
            }
            else
            {
                da = new DataAccess();
                da.saveDataToDB("DELETE FROM jumbled where user_id = '" + StaticClass.data.user_id + "' and word_id IN(select word_id from words where difficulty = " + StaticClass.difficulty + ")");
            }
        }

        //Get word and data from database
        if (StaticClass.mode=="timed")
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
            da.getWords(ref word_id, ref trivia_id, ref imageFP, ref sound_id, ref word, "SELECT * FROM words WHERE word_id NOT IN (SELECT word_id FROM jumbled WHERE user_id='" + StaticClass.data.user_id + "') and difficulty=" + StaticClass.difficulty + " order by random() limit 1");
        }
        Debug.Log("Okay word: " + word + " word_id: " + word_id + " trivia_id: " + trivia_id + " imageFP: " + imageFP + " sound_id: " + sound_id);
        
        //Randomize index and put it into array
        randomizeIndex = new int[word.Length];
        rnd = new System.Random();

        for (i = 0; i < word.Length; i++)
        {
            randomizeIndex[i] = i;
        }

        while (isNotRandom)
        {
            randomizeIndex = randomizeIndex.OrderBy(x => rnd.Next()).ToArray();

            //Check if array is ascending
            for (i = 0; i < word.Length - 1; i++)
            {
                if (randomizeIndex[i] > randomizeIndex[i + 1])
                    isNotRandom = false;
            }
        }

        for (i = 0; i < word.Length; i++)
        {
            Debug.Log("RandomOrder: " + randomizeIndex[i]);
        }

        //Put letters on right place
        for (i = 9; i > word.Length; i-- )
        {
            GameObject.Find("Slot" + i).SetActive(false);
        }

        for (i = 1; i <= word.Length; i++)
        {
            tilePic = GameObject.Find("Slot" + i).GetComponent<Image>();
            tilePic.overrideSprite = Resources.Load("jumbled_letters/" + word[randomizeIndex[i-1]].ToString().ToUpper(), typeof(Sprite)) as Sprite;
            GameObject.Find("Slot" + i).name = word[randomizeIndex[i-1]].ToString().ToUpper();
        }

        //Change word picture
        wordPic.overrideSprite = Resources.Load("word_pics/" + imageFP, typeof(Sprite)) as Sprite;

        if (!Resources.Load("word_pics/" + imageFP))
        {
            wordPic.overrideSprite = LoadImage(Application.persistentDataPath + "/images/" + imageFP);
        }

        wordPic.preserveAspect = true;

        //Apply word audio
        da = new DataAccess();
        if(StaticClass.data.selectedChar == 1)
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
    }

    public void checkAnswer()
    {
        answer = "";
        //Get answer
        for (i = 0; i < word.Length; i++)
        {
            answer += letterPanel.transform.GetChild(i).gameObject.name.ToLower();
        }
        Debug.Log("Answer: " + answer);
        //Check if answer is right
        if (answer == word)
        {
            da = new DataAccess();
            if (StaticClass.mode == "timed")
            {
                TimerScript.deduct++;
                TimerScript.score++;
                TimerScript.triviaWord[TimerScript.indJumbled] = word;
                TimerScript.fullTrivia[TimerScript.indJumbled] = da.getTrivia(trivia_id);
                TimerScript.wordPic[TimerScript.indJumbled] = imageFP;
                TimerScript.audioWord[TimerScript.indJumbled] = audioFP;
                TimerScript.tempWords[TimerScript.arrayIndex] = word;
                TimerScript.indJumbled++;
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
                    da.saveDataToDB("INSERT INTO jumbled (word_id, user_id) VALUES (" + word_id + ", '" + StaticClass.data.user_id + "');");
                    saved = true;
                    Debug.Log("Saved Word Done");
                }
                if (count == 1)
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
                GameObject.Find("Praise Sprite").GetComponent<SpriteRenderer>().sprite = Resources.Load("praise_pics/" + StaticClass.praisePics[random], typeof(Sprite)) as Sprite;
                GameObject.Find("Praise Sprite").GetComponent<Animator>().SetBool("IsPraise", true);
                GameObject.Find("Praise Sprite").GetComponent<AudioSource>().Play();
                StaticClass.ReloadScene();
            }
        }
        //if answer is wrong
        else
        {
            tryAgainTimer = 2.5f;
            wrongAudioSource.clip = Resources.Load(wrongAudio, typeof(AudioClip)) as AudioClip;
            wrongAudioSource.Play();
            tryAgainImage.gameObject.SetActive(true);
        }
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
    public void setDoneToFalse()
    {
        Debug.Log("IsCLicked");
        done = false;
    }
    public void fixInstruction()
    {
        if (StaticClass.mode=="untimed")
        {
            ins1.sprite = jumbledIns1;
            ins2.sprite = jumbledIns2;
            startButton.GetComponent<Image>().sprite = startSpriteUntimed;

            if (StaticClass.difficulty==1)
                ins3.sprite = jumbledEasy3;
            else if (StaticClass.difficulty==2)
                ins3.sprite = jumbledMedium3;
            else
                ins3.sprite = jumbledHard3;
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
        if (StaticClass.mode == "timed")
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
