using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class FinalSceneScript : MonoBehaviour {

    bool muted=false;
    public VideoPlayer vid;
    public VideoClip girlVideo;
    public VideoClip boyVideo;
    public GameObject videoPanel;
    public Button skipButton;
    DataAccess da;
    int level;

	// Use this for initialization
	void Start () {

        if (GameObject.Find("data GO").GetComponent<AudioSource>().mute)
            muted = true;
        else
            GameObject.Find("data GO").GetComponent<AudioSource>().mute = true;

        if (StaticClass.data.selectedChar == 1)
            vid.clip = boyVideo;
        else
            vid.clip = girlVideo;

        //Set Audio Output to AudioSource
        vid.audioOutputMode = VideoAudioOutputMode.AudioSource;

        //Assign the Audio from Video to AudioSource to be played
        vid.EnableAudioTrack(0, true);
        vid.SetTargetAudioSource(0, vid.GetComponent<AudioSource>());

        //Play Video
        vid.Play();

        //Play Sound
        vid.GetComponent<AudioSource>().Play();

        da = new DataAccess();
        level = da.getNum("SELECT level FROM level where category=" + StaticClass.category + " and mode='" + StaticClass.mode + "' and user_id='" + StaticClass.data.user_id + "';", "level");
        if (level==5)
        {
            skipButton.gameObject.SetActive(true);
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (videoPanel.activeInHierarchy)
        {
            if (Convert.ToInt32(vid.time) == Convert.ToInt32(vid.clip.length))
            {
                videoPanel.SetActive(false);
                GameMusic();
            }
        }
    }

    public void updateLevel()
    {
        if (level == 4)
        {
            da = new DataAccess();
            da.saveDataToDB("UPDATE level SET level = 5 WHERE category=" + StaticClass.category + " and mode='" + StaticClass.mode + "' and user_id='" + StaticClass.data.user_id + "';");
        }
    }

    public void GameMusic()
    {
        if (!muted)
            GameObject.Find("data GO").GetComponent<AudioSource>().mute = false;
    }
}
