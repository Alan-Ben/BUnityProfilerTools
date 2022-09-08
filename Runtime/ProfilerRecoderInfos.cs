using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.XR;

public class ProfilerRecoderInfos 
{
#if UNITY_2020_1_OR_NEWER
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
    ProfilerRecorder gpuMemoryRecorder;
    
    void initProfilerRecorderInfo()
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
    }

#endif
    

    public void init()
    {
#if UNITY_2020_1_OR_NEWER
        initProfilerRecorderInfo();
#endif
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
       
       
#if UNITY_2020_1_OR_NEWER

        m_SumGpuDeltaTime += gpuMemoryRecorder.LastValue/1000000.0f;

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
#endif

    }

    public void OnGUI()
    {
#if UNITY_2020_1_OR_NEWER

        int count = recordersList.Length;
        count = 4;
        for (int i = 0; i < count; i++)
        {
            GUILayout.Label(
                            $"{recordersList[i].accTime/m_frameCount:f4}," +
                            $"{recordersList[i].name}:" +
                            // $"GPU:{recordersList[i].gpuTime:f4}," +
                            $"{recordersList[i].callCount}");
        }


        float sum = 0;
        for (int i = 4; i < recordersList.Length; i++)
        {
            sum += recordersList[i].accTime/m_frameCount;
        }
        
        GUILayout.Label(
            $"{sum:f4}," +
            $"Animator Sum:" ,_style);
#endif
    }
    
    public void Reset()
    {
#if UNITY_2020_1_OR_NEWER
        for (int i = 0; i < recordersList.Length; i++)
        {
            recordersList[i].Reset();
        }
#endif

    }
}
