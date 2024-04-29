using System;
using System.Collections;
using PedometerU;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Person : MonoBehaviour
{
    private int totalSteps = 0;
    private Pedometer pedometer;
    Vector3 startVector;
    float singleStepDistance = 0.05f;
    public float compassSmooth = 50.5f;
    private float m_lastMagneticHeading = 0f;

    // Start is called before the first frame update
    void Start()
    {
        switch (PlayerPrefs.GetInt("IntValue"))
        {
            case 1:
                startVector = new Vector3(-1.1f, -1.9f, 0);
                transform.position = startVector;
                break;
            case 2:
                startVector = new Vector3(34.1f, 2.7f, 0);
                transform.position = startVector;
                GameObject go = GameObject.Find("Main Camera");
                go.transform.position = new Vector3(32.8f, 0, -392f);
                break;
            default:
                startVector = new Vector3(-1.1f, -1.9f, 0);
                transform.position = startVector;
                break;
        }

        pedometer = new Pedometer(OnStep);
        // Reset UI
        OnStep(0, 0);

        Input.location.Start();
        // Start the compass.
        Input.compass.enabled = true;

    }

    private void OnStep(int steps, double distance)
    {

        totalSteps = steps;
    }

    // Update is called once per frame
    //void Update()
    //{
    //    float distanceToMove = totalSteps * singleStepDistance;
    //    transform.position = startVector + new Vector3(0, distanceToMove, 0);

    //    float currentMagneticHeading = (float)Math.Round(Input.compass.magneticHeading, 2);
    //    if (m_lastMagneticHeading < currentMagneticHeading - compassSmooth || m_lastMagneticHeading > currentMagneticHeading + compassSmooth)
    //    {
    //        m_lastMagneticHeading = currentMagneticHeading;
    //        transform.rotation = Quaternion.Euler(0, 0, m_lastMagneticHeading);
    //    }
    //}

    void Update()
    {
        // Calculate the distance to move based on total steps
        float distanceToMove = totalSteps * singleStepDistance;

        // Get the current compass heading
        float currentMagneticHeading = (float)Math.Round(Input.compass.magneticHeading, 2);

        // Calculate the forward direction based on the compass heading
        Vector3 forwardDirection = Quaternion.Euler(0, 0, currentMagneticHeading) * Vector3.forward;

        // Move the object in the forward direction
        transform.position = startVector + forwardDirection * distanceToMove;

        // Rotate the object to match the compass heading
        transform.rotation = Quaternion.Euler(0, 0, currentMagneticHeading);
    }


    private void OnDisable()
    {
        // Release the pedometer
        pedometer.Dispose();
        pedometer = null;
    }

    void OnGUI()
    {
        // Calculate the center of the screen
        float centerX = Screen.width / 2;
        float centerY = Screen.height / 2;

        // Define the size of the text box
        float width = 700;
        float height = 400;

        // Calculate the position of the text box
        float x = centerX - (width / 2);
        float y = centerY - (height / 2);

        // Make a text field that modifies stringToEdit
        string outText = "Total Steps: " + totalSteps.ToString() + " compasssss : " + Input.compass.trueHeading.ToString();

        GUIStyle style = new GUIStyle(GUI.skin.textField);
        style.fontSize = 26;



        GUI.TextField(new Rect(x, y, width, height), outText, style);

    }
}