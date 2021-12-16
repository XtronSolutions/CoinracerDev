using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

[Serializable]
public class TimeUI
{
    [Tooltip("UI Reference for timer text as texMeshGUI")]
    public TextMeshProUGUI TimeText;
}

public class TimeHandler : MonoBehaviour
{
    public TimeUI UITime;//instance of class TimeUI which handles UI content
    public static TimeHandler Instance; //creating static instance of the TimeHandler class

    [HideInInspector]
    public double TotalSeconds = 0;//float to store total seconds elapsed
    [HideInInspector]
    public bool timerIsRunning = false;//bool to handle timer (running/stopping)

    float timeSpanConversionHours;//var to hold hours after converstion from seconds
    float timeSpanConversiondMinutes;//var to hold minutes after converstion from seconds
    float timeSpanConversionSeconds;//var to hold seconds after converstion from float seconds
    float timeSpanConversionMiliSeconds;//var to hold mili seconds after converstion from float seconds

    string textfieldHours;//string store converstion of hours into string for display
    string textfieldMinutes;//string store converstion of minutes into string for display
    string textfieldSeconds;//string store converstion of seconds into string for display
    string textfieldMiliSeconds;//string store converstion of mili seconds into string for display

    string MainTime;//string to store complete time

    void Start()
    {
        Instance = this;

        if(!Constants.IsMultiplayer)
            timerIsRunning = true;
    }

    void Update()
    {
        if (timerIsRunning && Constants.MoveCar)
        {
            TotalSeconds += Time.deltaTime;
            ConvertTime(TotalSeconds);
            DisplayScreenTime();
        }
    }

    public void ConvertTime(double _sec)
    {
        //Store TimeSpan into variable.
        timeSpanConversionHours = TimeSpan.FromSeconds(_sec).Hours;
        timeSpanConversiondMinutes = TimeSpan.FromSeconds(_sec).Minutes;
        timeSpanConversionSeconds = TimeSpan.FromSeconds(_sec).Seconds;
        timeSpanConversionMiliSeconds = TimeSpan.FromSeconds(_sec).Milliseconds/10;

        //Convert TimeSpan variables into strings for textfield display
        textfieldHours = timeSpanConversionHours.ToString();
        textfieldMinutes = timeSpanConversiondMinutes.ToString();
        textfieldSeconds = timeSpanConversionSeconds.ToString();
        textfieldMiliSeconds = timeSpanConversionMiliSeconds.ToString();
    }

    public void DisplayScreenTime()
    {
        //Display the time given the number of digits.
        if (textfieldMinutes.Length == 2 && textfieldSeconds.Length == 2)
        { 
            MainTime = textfieldHours + ":" + textfieldMinutes + ":" + textfieldSeconds+":"+textfieldMiliSeconds; 
        }
        else if (textfieldMinutes.Length == 2 && textfieldSeconds.Length == 1)
        { 
            MainTime = textfieldHours + ":" + textfieldMinutes + ":0" + textfieldSeconds + ":" + textfieldMiliSeconds;
        }
        else if (textfieldMinutes.Length == 1 && textfieldSeconds.Length == 1)
        { 
            MainTime = textfieldHours + ":0" + textfieldMinutes + ":0" + textfieldSeconds + ":" + textfieldMiliSeconds; 
        }
        else if (textfieldMinutes.Length == 1 && textfieldSeconds.Length == 2)
        { 
            MainTime = textfieldHours + ":0" + textfieldMinutes + ":" + textfieldSeconds + ":" + textfieldMiliSeconds; 
        }
        else
        { 
            MainTime = textfieldHours + ":" + textfieldMinutes + ":" + textfieldSeconds + ":" + textfieldMiliSeconds; 
        }

        UITime.TimeText.text = MainTime;
    }
}
