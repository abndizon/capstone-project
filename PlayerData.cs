using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

[System.Serializable]
public class PlayerData {
    
    [SerializeField]
    public int selectedChar;
    public string user_id;
}
