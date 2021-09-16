using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountdownScript : MonoBehaviour {

    public static float lifetime;

    void Start()
    {
        lifetime = 3.30f;
    }

    // Update is called once per frame
    void Update () {
        lifetime -= Time.deltaTime;
        Debug.Log(Convert.ToInt32(lifetime));
        if (Convert.ToInt32(lifetime) < 0)
        {
            gameObject.SetActive(false);
        }
    }
}
