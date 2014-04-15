using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary> 
/// 将Console里的log显示到屏幕上
/// </summary>

public class LogRenderer : MonoBehaviour {

    public static bool Render_Debug = true;
    public static bool Render_Warning = true;
    public static bool Render_Error = true;

    /// <summary> 屏幕边距 </summary>
    public static readonly Vector2 margin = new Vector2 (20, 20);

    struct LogInfo {
        public string content;
        //public string stackString = string.Empty;
        public LogType type;
        public float removeTime;
        public Vector2 size;

        public LogInfo(string _log, string _callStack, LogType _type, float _removeTime, Vector2 _size) {
            content = _log;
            //stackString = _callStack;
            type = _type;
            removeTime = _removeTime;
            size = _size;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // Parameters
    ///////////////////////////////////////////////////////////////////////////////

    //public bool showLog = true;
    //public bool showWarning = true;
    //public bool showError = true;

    /// <summary> max log display count </summary>
    public int maxCount = 120;
    /// <summary> log display time </summary>
    public float elapseTime = 10f;

    public GUIStyle logStyle;
    public GUIStyle waringStyle;
    public GUIStyle errorStyle;

    private List<LogInfo> logList = new List<LogInfo>();
    private GUIStyle[] styleList = new GUIStyle[(int)LogType.Exception + 1];

    ///////////////////////////////////////////////////////////////////////////////
    // Functions
    ///////////////////////////////////////////////////////////////////////////////

	void Awake () {
#if MJ_DEBUG
        Application.RegisterLogCallback(AddLog);
#endif
        DontDestroyOnLoad (gameObject);
        useGUILayout = false;

        styleList[(int)LogType.Log] = logStyle;
        styleList[(int)LogType.Warning] = waringStyle;
        styleList[(int)LogType.Error] = errorStyle;
        styleList[(int)LogType.Exception] = errorStyle;
        styleList[(int)LogType.Assert] = errorStyle;
	}

    //void Update() {
    //    if (Input.GetMouseButtonDown(0)) {
    //        Debug.Log("it is a log info");
    //    }
    //    if (Input.GetMouseButtonDown(1)) {
    //        Debug.LogWarning("it is a waring info");
    //    }
    //    if (Input.GetMouseButtonDown(2)) {
    //        Debug.LogError("it is a error info");
    //    }
    //}

    void OnGUI() {
        float y = margin.y;
        float curTime = Time.time;
        for (int i = logList.Count - 1; i >= 0 ; --i) {
            LogInfo log = logList[i];
            if (curTime > log.removeTime) {
                logList.RemoveAt(i);
            }
            else {
                GUIStyle style = styleList[(int)log.type];
                GUI.Label(new Rect(margin.x, y, log.size.x, log.size.y), log.content, style);
                y += log.size.y;
            }
        }
    }

    void AddLog(string _log, string _callStack, LogType _type)
    {
        switch (_type) {
        case LogType.Log:
            if (Render_Debug == false) {
                return;
            }
            break;
        case LogType.Warning:
            if (Render_Warning == false) {
                return;
            }
            break;
        case LogType.Error:
        case LogType.Exception:
        case LogType.Assert:
            if (Render_Error == false) {
                return;
            }
            break;
        }
        
        if(logList.Count >= maxCount){
            logList.RemoveAt(0);
        }
        Vector2 size = styleList[(int)_type].CalcSize(new GUIContent(_log));
        logList.Add(new LogInfo(_log, _callStack, _type, Time.time + elapseTime, size));
    }
}
