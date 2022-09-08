using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace BProfiler
{
    public class FPSShow
    {
        public int m_frameCount;
        public float m_SumDeltaTime;
        public float m_SumGpuDeltaTime;
        public float m_AveTime;
        public float m_AveGpuTime;
        public float m_Fps ;

        private float _m_lastResetTime;
        private int _m_lastRestFrameCount;

        public void init()
        {
            reset();
        }

        public void reset()
        {
            m_SumDeltaTime = 0.0f;
            m_SumGpuDeltaTime = 0.0f;
            m_frameCount = 0;
            _m_lastResetTime = Time.realtimeSinceStartup;
            _m_lastRestFrameCount = Time.frameCount;
        }

        public void OnGUI()
        {
            GUILayout.Label($"FPS:{m_Fps},Time:{m_AveTime:f2}, {m_AveGpuTime:f2}");

            float aveTime = (Time.realtimeSinceStartup - _m_lastResetTime)/(Time.frameCount - _m_lastRestFrameCount) * 1000;
            float fps = (Time.frameCount - _m_lastRestFrameCount) / (Time.realtimeSinceStartup - _m_lastResetTime);

            GUILayout.Label($"FPS:{fps:f0}, Time:{aveTime:f2}");
            if (GUILayout.Button("Reset"))
                reset();

        }
        public void Update()
        {
            m_SumDeltaTime += Time.unscaledDeltaTime;
            m_frameCount++;
            m_AveTime = m_SumDeltaTime * 1000.0f / m_frameCount;
            m_AveGpuTime = m_SumGpuDeltaTime / m_frameCount;
            m_Fps = (int) (((float) m_frameCount) / m_SumDeltaTime);
            
        }
    }

    public class ToolSetFPS
    {
        private bool _m_is60Rate = false;
        private int _m_frameRate = 30;
        
        public void OnGUI()
        {
            GUILayout.BeginHorizontal();
            _m_frameRate = int.Parse(GUILayout.TextField(_m_frameRate.ToString()));
            if (GUILayout.Button("SetFPS"))
            {
                Application.targetFrameRate = _m_frameRate;
            }
            if (GUILayout.Button(_m_is60Rate ? "FPS:60" : "FPS:30"))
            {
                _m_is60Rate = !_m_is60Rate;
                _m_frameRate = _m_is60Rate ? 60 : 30;
                Application.targetFrameRate = _m_frameRate ;
            }
            GUILayout.EndHorizontal();
        }
    }

    public class ToolSetResolution
    {
        
        private int _m_screenWidth;
        private int _m_screenHeight;

        public void init()
        {
            _m_screenWidth = Screen.width;
            _m_screenHeight = Screen.height;
        }
        
        public void OnGUI()
        {
            GUILayout.BeginHorizontal();
            _m_screenWidth = int.Parse(GUILayout.TextField(_m_screenWidth.ToString()));
            _m_screenHeight = int.Parse(GUILayout.TextField(_m_screenHeight.ToString()));
            
            if (GUILayout.Button("分辨率"))
            {
                Screen.SetResolution(_m_screenWidth,_m_screenHeight,true);
            }
            GUILayout.EndHorizontal();
        }
    }

    [Serializable]
    public class ToolChangeScene 
    {
        public bool showChangeScene = false;
        public List<string> changesScene = new List<string>();
        public void init()
        {
            if(!showChangeScene)
                return;
            if (changesScene != null && changesScene.Count > 0) 
                SceneManager.LoadScene(changesScene[0], LoadSceneMode.Single);
        }
        
        public void OnGUI()
        {
            if(!showChangeScene)
                return;
            GUILayout.BeginHorizontal();
            GUILayout.Label(SceneManager.GetActiveScene().name);
            foreach (var scene in changesScene)
            {
                if (GUILayout.Button(scene))
                    SceneManager.LoadScene(scene, LoadSceneMode.Single);
            }
            GUILayout.EndHorizontal();
        }
    }

    public class ToolGraphicSetting
    {
        public void OnGUI()
        {
            if (GUILayout.Button($"Shadow:{QualitySettings.shadows}"))
            {
                int tmp = (int) QualitySettings.shadows;
                tmp++;
                tmp = tmp % 3;
                QualitySettings.shadows = (ShadowQuality) tmp;
            }
            if (GUILayout.Button($"ShadowResolution:{QualitySettings.shadowResolution}"))
            {
                int tmp = (int) QualitySettings.shadowResolution;
                tmp++;
                tmp = tmp % 4;
                QualitySettings.shadowResolution = (ShadowResolution) tmp;
            }
        }
    }
    
    public class ProfilerShower : MonoBehaviour
    {
        public int textSize = 36;
        public Color textColor = Color.white;
        
        public ToolChangeScene toolChangeScene = new ToolChangeScene();
        
        private GUISkin _m_guiSkin;
        
        private ProfilerRecoderInfos collector;
        private FPSShow fpsShow;
        private ToolSetFPS toolSetFPS;
        private ToolSetResolution toolSetResolution;
        private ToolGraphicSetting toolGraphicSetting;

        private bool _m_bIsShow = true;
        
        // Start is called before the first frame update
        void Start()
        {
            DontDestroyOnLoad(this.gameObject);
            

            collector = new ProfilerRecoderInfos();
            collector.init();
            
            fpsShow = new FPSShow();
            fpsShow.init();

            toolSetFPS = new ToolSetFPS();
            
            toolSetResolution = new ToolSetResolution();
            toolSetResolution.init();

            toolChangeScene = new ToolChangeScene();
            toolChangeScene.init();

            toolGraphicSetting = new ToolGraphicSetting();
        }

        private void Update()
        {
            fpsShow.Update();
        }

        void initGUIStyle()
        {
            _m_guiSkin = Instantiate(GUI.skin);
            _m_guiSkin.label.fontSize = textSize;
            _m_guiSkin.label.normal.textColor = textColor;
            _m_guiSkin.button.fontSize = textSize;
            _m_guiSkin.textField.fontSize = textSize;
            _m_guiSkin.toggle.fontSize = textSize;
        }

        public void OnGUI()
        {
            initGUIStyle();

            var tmpGUISkin = GUI.skin;
            GUI.skin = _m_guiSkin;

            if(GUILayout.Button(_m_bIsShow ?  "Hide" : "Show"))
                _m_bIsShow = !_m_bIsShow;
            if (_m_bIsShow)
            {
                fpsShow.OnGUI();
                collector.OnGUI();
                toolSetFPS.OnGUI();
                toolSetResolution.OnGUI();
                toolChangeScene.OnGUI();
                toolGraphicSetting.OnGUI();
            }
           
            GUI.skin = tmpGUISkin;
        }
    }
}