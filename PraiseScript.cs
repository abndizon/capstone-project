using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PraiseScript : MonoBehaviour {

    float lifetime;

    void Start()
    {
        lifetime = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.GetComponent<Animator>().GetBool("IsPraise"))
        {
            lifetime -= Time.deltaTime;
            Debug.Log(Convert.ToInt32(lifetime));
            if (Convert.ToInt32(lifetime) < 0)
            {
                gameObject.GetComponent<Animator>().SetBool("IsPraise", false);
            }
        }
        else
            lifetime = 1.0f;
    }
}
