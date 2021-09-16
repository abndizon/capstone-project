using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//
public class AskNameScript : MonoBehaviour {

    public InputField nameField;
    public Button okayButton;
    public Button quitButton;
    public GameObject backButton;
    public GameObject userExistPanel;
    public Image okayImage;  
    public static string username;
    DataAccess da;

    void Start()
    {
        if (StaticClass.Load() && StaticClass.isStart)
        {
            StaticClass.askUser = true;
            StaticClass.changeScenes(2);
        }
        if (!StaticClass.isStart)
        {
            backButton.SetActive(true);
            quitButton.gameObject.SetActive(false);
        }
        if (username != "" || username != null)
        {
            nameField.text = username;
        }
    }
    void Update()
    {
        if (nameField.text.Length > 1 && nameField.text.Length <= 15)
        {
            okayButton.enabled = true;
            okayImage.color = new Color32(255, 255, 255, 255);
        }
        else
        {
            okayButton.enabled = false;
            okayImage.color = new Color32(160, 160, 160, 100);
        }
    }
	public void selectOkay()
    {
        da = new DataAccess();
        username = nameField.text;
        if (da.dataExist("SELECT user_id FROM user_data WHERE upper(user_id)=upper('" + username + "');", "user_id"))
        {
            userExistPanel.SetActive(true);
            nameField.text = "";
        }
        else
        {
            StaticClass.changeScenes(1);
        }
    }
    public void selectBack()
    {
        username = "";
        StaticClass.isBackSelected = true;
    }
}
