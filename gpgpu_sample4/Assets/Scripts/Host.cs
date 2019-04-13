using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Host : MonoBehaviour
{
    public ComputeShader shader;
    void Start()
    {
        uint N = 65536*256;
        uint[] host_A = new uint[N];
        for (uint i = 0; i < N; i++)
        {
            host_A[i] = i;
        }
        uint[] host_B = { 0 };//この数字はなんでもいい。

        ComputeBuffer A = new ComputeBuffer(host_A.Length, sizeof(uint));
        ComputeBuffer AtomicBUF = new ComputeBuffer(host_B.Length, sizeof(uint));
        int k = shader.FindKernel("sharedmem_samp1");

        // host to device
        A.SetData(host_A);

        //引数をセット
        shader.SetBuffer(k, "A", A);
        shader.SetBuffer(k, "atmicBUF", AtomicBUF);
        shader.SetInt("N", (int)N);
        // GPUで計算
        int time0 = Gettime();
        shader.Dispatch(k, (int)N/256, 1, 1);//ここでは1*1*1並列を指定。ComputeShader側で256並列を指定している

        // device to host
        AtomicBUF.GetData(host_B);
        int time1 = Gettime() - time0;

        //結果
        Debug.Log("addの結果   "+host_B[0]);
        Debug.Log("演算時間(ms) "+time1);

        //解放
        A.Release();
        AtomicBUF.Release();
    }

    // Update is called once per frame
    void Update()
    {
    }

    //現在の時刻をms単位で取得
    int Gettime()
    {
        return DateTime.Now.Millisecond + DateTime.Now.Second * 1000
         + DateTime.Now.Minute * 60 * 1000 + DateTime.Now.Hour * 60 * 60 * 1000;
    }
}