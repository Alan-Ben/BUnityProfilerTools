using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.XR;

public class ProfilerCollector : MonoBehaviour
{
    public class RecorderEntry
    {
        public string name;
        public string oldName;
        public Recorder recorder;
        public ProfilerRecorder recorderGpu;
        
        public int callCount;
        public float accTime;
        public float gpuTime;
        
        public void Reset()
        {
            callCount = 0;
            accTime = 0.0f;
            gpuTime = 0f;
        }

    };
    
    public RecorderEntry[] recordersList =
    {
        // Warning: Keep that list in the exact same order than SRPBMarkers enum
        new RecorderEntry() { name="PlayerLoop"},
        new RecorderEntry() { name="Camera.Render"},
        new RecorderEntry() { name="Render.OpaqueGeometry", oldName="Render.OpaqueGeometry"},
        new RecorderEntry() { name="Animators.Update"},
        new RecorderEntry() { name="PreLateUpdate.DirectorUpdateAnimationEnd"},
        new RecorderEntry() { name="PostLateUpdate.UpdateAllSkinnedMeshes"},
        new RecorderEntry() { name="PostLateUpdate.UpdateAllRenderers"},
        new RecorderEntry() { name="PreLateUpdate.DirectorUpdateAnimationBegin"},


    };
    
    public int m_frameCount;
    public float m_SumDeltaTime;
    public float m_SumGpuDeltaTime;
    public float m_AveTime;
    public float m_AveGpuTime;
    // public float m_Time;
    // public float m_GpuTime;
    public float m_Fps ;
    
    ProfilerRecorder gpuMemoryRecorder;

    void Awake()
    {           
        gpuMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Camera.Render");
        for (int i = 0; i < recordersList.Length; i++)
        {
            var sampler = Sampler.Get(recordersList[i].name);
            if (sampler.isValid)
                recordersList[i].recorder = sampler.GetRecorder();
            else if ( recordersList[i].oldName != null )
            {
                sampler = Sampler.Get(recordersList[i].oldName);
                if (sampler.isValid)
                    recordersList[i].recorder = sampler.GetRecorder();
            }
            recordersList[i].recorderGpu = ProfilerRecorder.StartNew(ProfilerCategory.Render, recordersList[i].name);
        }
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
       
        m_SumDeltaTime += Time.unscaledDeltaTime;
        m_SumGpuDeltaTime += gpuMemoryRecorder.LastValue/1000000.0f;

        m_frameCount++;

        // get timing & update average accumulators
        for (int i = 0; i < recordersList.Length; i++)
        {
            if ( recordersList[i].recorder != null )
            {
                recordersList[i].accTime += recordersList[i].recorder.elapsedNanoseconds / 1000000.0f;      // acc time in ms
                recordersList[i].callCount += recordersList[i].recorder.sampleBlockCount;
                recordersList[i].gpuTime = recordersList[i].recorderGpu.LastValue/1000000.0f;
            }
        }

        m_AveTime = m_SumDeltaTime * 1000.0f / m_frameCount;
        m_AveGpuTime = m_SumGpuDeltaTime / m_frameCount;
        m_Fps = (int) (((float) m_frameCount) / m_SumDeltaTime);
    }

    public void GUIShow(GUIStyle _style)
    {
        int count = recordersList.Length;
        count = 4;
        for (int i = 0; i < count; i++)
        {
            GUILayout.Label(
                            $"{recordersList[i].accTime/m_frameCount:f4}," +
                            $"{recordersList[i].name}:" +
                            // $"GPU:{recordersList[i].gpuTime:f4}," +
                            $"{recordersList[i].callCount}",_style);
        }

        float sum = 0;
        for (int i = 4; i < recordersList.Length; i++)
        {
            sum += recordersList[i].accTime/m_frameCount;
        }
        GUILayout.Label(
            $"{sum:f4}," +
            $"Animator Sum:" ,_style);
        GUILayout.Label($"FPS:{m_Fps},Time:{m_AveTime:f4}, {m_AveGpuTime:f4}",_style);
    }
    
    public void Reset()
    {
        m_SumDeltaTime = 0.0f;
        m_SumGpuDeltaTime = 0.0f;
        m_frameCount = 0;
        for (int i = 0; i < recordersList.Length; i++)
        {
            recordersList[i].Reset();
        }
    }
}
