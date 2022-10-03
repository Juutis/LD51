using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotator : MonoBehaviour
{
    private bool playing = false;
    private float rotateSpeed = 8.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playing) {
            transform.Rotate(Vector3.down, rotateSpeed * Time.deltaTime);
        }
    }

    public void Play() {
        playing = true;
    }

    public void Stop() {
        playing = false;
    }
}
