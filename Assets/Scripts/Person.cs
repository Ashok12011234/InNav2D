using System;
using System.Collections;
using PedometerU;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class Person : MonoBehaviour
{
    private int totalSteps = 0;
    //private Pedometer pedometer;
    Vector3 startVector;
    float singleStepDistance = 0.1f;
    public float compassSmooth = 50.5f;
    private float m_lastMagneticHeading = 0f;

    public float loLim = 0.005f; // level to fall to the low state 0.005
    public float hiLim = 0.1f; // level to go to high state (and detect step) 0.1 
    public int steps = 0; // step counter - counts when comp state goes high private 
    bool stateH = false; // comparator state

    public float fHigh = 20.0f; // noise filter control - reduces frequencies above fHigh private 10
    public float curAcc = 0f; // noise filter 
    public float fLow = 0.05f; // average gravity filter control - time constant about 1/fLow 0.1
    float avgAcc = 0f;
  
    void Awake()
    {
        avgAcc = Input.acceleration.magnitude; // initialize avg filter
    }

    // Start is called before the first frame update
    void Start()
    {
        switch (PlayerPrefs.GetInt("IntValue"))
        {
            case 1:
                startVector = new Vector3(-0.714f, -2.55f, 0);
                transform.position = startVector;
                break;
            case 2:
                startVector = new Vector3(34.1f, 2.7f, 0);
                transform.position = startVector;
                GameObject go = GameObject.Find("Main Camera");
                go.transform.position = new Vector3(32.8f, 0, -392f);
                break;
            default:
                startVector = new Vector3(-0.714f, -2.55f, 0);
                transform.position = startVector;
                break;
        }

        //pedometer = new Pedometer(OnStep);
        // Reset UI
        //OnStep(0, 0);

        Input.location.Start();
        // Start the compass.
        Input.compass.enabled = true;

    }

    //private void OnStep(int steps, double distance)
    //{

    //    totalSteps = steps;
    //}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Main Menu");
        }
    }

    //private void OnDisable()
    //{
    //    // Release the pedometer
    //    pedometer.Dispose();
    //    pedometer = null;
    //}

    void OnGUI()
    {
        // Calculate the center of the screen
        float centerX = Screen.width / 2;
        float centerY = Screen.height / 2;

        // Define the size of the text box
        float width = 500;
        float height = 50;

        // Define the margin from the top of the screen
        float margin = 150;

        // Calculate the position of the text box
        float x = centerX - (width / 2);
        float y = margin;

        // Make a text field that modifies stringToEdit
        string outText = "Total Steps: " + steps.ToString() + " compass : " + Input.compass.trueHeading.ToString();

        GUIStyle style = new GUIStyle(GUI.skin.textField);
        style.fontSize = 26;

        GUI.TextField(new Rect(x, y, width, height), outText, style);

    }

    void FixedUpdate()
    {
        // Get the current compass heading
        float currentMagneticHeading = (float)Math.Round(Input.compass.magneticHeading, 2);


        // Rotate the object to match the compass heading
        transform.rotation = Quaternion.Euler(0, 0, -currentMagneticHeading);

        // filter input.acceleration using Lerp
        curAcc = Mathf.Lerp(curAcc, Input.acceleration.magnitude, Time.deltaTime * fHigh);
        avgAcc = Mathf.Lerp(avgAcc, Input.acceleration.magnitude, Time.deltaTime * fLow);
        float delta = curAcc - avgAcc; // gets the acceleration pulses
        if (!stateH)
        { // if state == low...
            if (delta > hiLim)
            { // only goes high if input > hiLim
                stateH = true;
                steps++; // count step when comp goes high
                movePerson();
            }
        }
        else
        {
            if (delta < loLim)
            { // only goes low if input < loLim 
                stateH = false;
            }
        }
    }

    double DegreesToRadians(double degrees)
    {
        return (Math.PI / 180) * degrees;
    }

    void movePerson()
    {
        double y = 0;
        double x = 0;

        if (m_lastMagneticHeading >= 0 && m_lastMagneticHeading < 90)
        {
            y = -Math.Sin(DegreesToRadians(m_lastMagneticHeading)) * singleStepDistance;
            x = Math.Cos(DegreesToRadians(m_lastMagneticHeading)) * singleStepDistance;
        }
        else if (m_lastMagneticHeading >= 90 && m_lastMagneticHeading < 180)
        {
            y = -Math.Sin(DegreesToRadians(m_lastMagneticHeading - 90)) * singleStepDistance;
            x = -Math.Cos(DegreesToRadians(m_lastMagneticHeading - 90)) * singleStepDistance;
        }
        else if (m_lastMagneticHeading >= 180 && m_lastMagneticHeading < 270)
        {
            y = Math.Sin(DegreesToRadians(m_lastMagneticHeading - 180)) * singleStepDistance;
            x = -Math.Cos(DegreesToRadians(m_lastMagneticHeading - 180)) * singleStepDistance;
        }
        else if (m_lastMagneticHeading >= 270 && m_lastMagneticHeading < 360)
        {
            y = Math.Sin(DegreesToRadians(m_lastMagneticHeading - 270)) * singleStepDistance;
            x = Math.Cos(DegreesToRadians(m_lastMagneticHeading - 270)) * singleStepDistance;
        }
        else
        {
            Console.WriteLine("default case");
        }

        transform.Translate(new Vector3((float)x, (float)y, 0));
    }

}