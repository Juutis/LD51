using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStartGame : MonoBehaviour
{
    [SerializeField]
    private GameObject container;
    void Start()
    {
        /*if (!Application.isEditor)
        {*/
        Time.timeScale = 0f;
        container.SetActive(true);
        /*}
        else
        {
            Destroy(gameObject);
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey)
        {
            Time.timeScale = 1f;
            Destroy(gameObject);
        }
    }
}
