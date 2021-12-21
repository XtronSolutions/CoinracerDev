using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class LogResponse { public string response; }
public class LogHandler : MonoBehaviour
{
    public static LogHandler Instance;
    string level = "";
    string logglyURL = "http://logs-01.loggly.com/inputs/727238b0-fd29-4d10-9a73-79a36a700c25/tag/Unity3D";
    public void OnEnable(){

        if(!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
	    Application.logMessageReceived += HandleLog;
        #endif
    }

    //Remove callback when object goes out of scope
    public void OnDisable(){
        #if UNITY_WEBGL && !UNITY_EDITOR
	    Application.logMessageReceived -= HandleLog;
        #endif
    }

    public void HandleLog(string logString, string stackTrace, LogType type)
    {

        //Initialize WWWForm and store log level as a string
        level = type.ToString();
        var loggingForm = new WWWForm();

        //Add log message to WWWForm
        loggingForm.AddField("LEVEL", level);
        loggingForm.AddField("Message", logString);
        loggingForm.AddField("Stack_Trace", stackTrace);

        //Add any User, Game, or Device MetaData that would be useful to finding issues later
        loggingForm.AddField("Device_Model", SystemInfo.deviceModel);
        SendLogs(loggingForm);
    }

    async public void SendLogs(WWWForm loggingForm)
    {
        string response = await PushLogs(loggingForm);

        if (response == "" || response == "null" || response == string.Empty)
            Debug.LogError("error sending logs to loggly");
    }
    public async Task<string> PushLogs(WWWForm _form)
    {
        UnityWebRequest webRequest = UnityWebRequest.Post(logglyURL, _form);
        await webRequest.SendWebRequest();
        LogResponse data = JsonUtility.FromJson<LogResponse>(System.Text.Encoding.UTF8.GetString(webRequest.downloadHandler.data));
        return data.response;
    }
}
