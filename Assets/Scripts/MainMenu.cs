using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] InputField inputField;
    [SerializeField] Text resultText;

    void Start()
    {
        TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, true);
    }

    public void ValidateInput()
    {
        string input = inputField.text;
        int intValue = 1;
        if (input.Length == 0) 
        {
            resultText.text = "Please enter the number";
            resultText.color = Color.red;
        }
        else
        {
            resultText.text = "";
            switch (input)
            {
                case "1":
                    intValue = 1; // Your integer value
                    break;
                case "2":
                    intValue = 2; // Your integer value
                    break;

            }
            PlayerPrefs.SetInt("IntValue", intValue);
            SceneManager.LoadScene("SampleScene");

        }
    }
}
