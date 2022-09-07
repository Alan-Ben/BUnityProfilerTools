using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class ProfilerShower : MonoBehaviour
{
    public ProfilerCollector collector;
    
    private GUIStyle m_style;
    public List<string> changesScene = new List<string>();
    
    private bool _m_openGUI = true;
    private int _m_showCount = 10;
    private int _m_screenWidth;
    private int _m_screenHeight;


    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        _m_screenWidth = Screen.width;
        _m_screenHeight = Screen.height;
        
        m_style =new GUIStyle();
        m_style.fontSize = 30;
        m_style.normal.textColor = Color.black;

        if (changesScene != null && changesScene.Count > 0) 
            SceneManager.LoadScene(changesScene[0], LoadSceneMode.Single);
    }

    private bool _m_is60Rate = false;
    private int _m_frameRate = 30;
    public void OnGUI()
    {
        collector.GUIShow(m_style);

        GUILayout.BeginHorizontal();
        _m_frameRate = int.Parse(GUILayout.TextField(_m_frameRate.ToString(),GUILayout.Width(100),GUILayout.Height(50)));
        if (GUILayout.Button("SetFPS", GUILayout.Width(100), GUILayout.Height(50)))
        {
            Application.targetFrameRate = _m_frameRate;
        }
        if (GUILayout.Button(_m_is60Rate ? "FPS:60" : "FPS:30", GUILayout.Width(100), GUILayout.Height(50)))
        {
            _m_is60Rate = !_m_is60Rate;
            _m_frameRate = _m_is60Rate ? 60 : 30;
            Application.targetFrameRate = _m_frameRate ;
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        
        if(GUILayout.Button("Reset", GUILayout.Width(100), GUILayout.Height(50)))
            collector.Reset();
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label(SceneManager.GetActiveScene().name);
        foreach (var scene in changesScene)
        {
            if (GUILayout.Button(scene, GUILayout.Width(100), GUILayout.Height(50)))
                SceneManager.LoadScene(scene, LoadSceneMode.Single);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        _m_screenWidth = int.Parse(GUILayout.TextField(_m_screenWidth.ToString(),GUILayout.Width(100),GUILayout.Height(50)));
        _m_screenHeight = int.Parse(GUILayout.TextField(_m_screenHeight.ToString(),GUILayout.Width(100),GUILayout.Height(50)));

        
        if (GUILayout.Button("分辨率",GUILayout.Width(90),GUILayout.Height(90)))
        {
            Screen.SetResolution(_m_screenWidth,_m_screenHeight,true);
        }
        if (GUILayout.Button("Sce no full",GUILayout.Width(90),GUILayout.Height(90)))
        {
            Screen.SetResolution(_m_screenWidth,_m_screenHeight,false);
        }
     
        GUILayout.EndHorizontal();
      
    }
}
