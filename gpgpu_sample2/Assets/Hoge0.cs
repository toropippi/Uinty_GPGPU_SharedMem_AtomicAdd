using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Hoge0 : MonoBehaviour
{
    public ComputeShader shader;

    void Start()
    {
        int time0 = Gettime();
        int bp = 32;//ComputeShaderで指定している並列数
        int gp = 128;//Dispatchで指定している並列数
        int N = 65536 * 2048;
        float[] host_vec1 = new float[N];
        float[] host_vec2 = new float[N];
        float[] host_ans = new float[bp * gp];

        //初期値をセット
        for (int i = 0; i < N; i++)
        {
            host_vec1[i] = 1.0f / (i + 1.0f);// 1/1 , 1/2 , 1/3 , 1/4・・・・
            host_vec2[i] = 1.0f / (i + 1.0f);// 1/1 , 1/2 , 1/3 , 1/4・・・・
        }

        ComputeBuffer vec1 = new ComputeBuffer(host_vec1.Length, sizeof(float));
        ComputeBuffer vec2 = new ComputeBuffer(host_vec2.Length, sizeof(float));
        ComputeBuffer ans = new ComputeBuffer(host_ans.Length, sizeof(float));

        int k = shader.FindKernel("CSMain");

        // host to device
        int time1 = Gettime() - time0;
        vec1.SetData(host_vec1);
        vec2.SetData(host_vec2);
        int time2 = Gettime() - time0;

        //引数をセット
        shader.SetBuffer(k, "vec1", vec1);
        shader.SetBuffer(k, "vec2", vec2);
        shader.SetBuffer(k, "ans", ans);

        // GPUで計算
        int time3 = Gettime() - time0;
        shader.Dispatch(k, gp, 1, 1);
        int time4 = Gettime() - time0;

        // device to host
        ans.GetData(host_ans);
        int time5 = Gettime() - time0;

        //計測時間表示
        Debug.Log("CPU→GPU転送前\t" + time1);
        Debug.Log("CPU→GPU転送後\t" + time2);
        Debug.Log("Dispatch前　　\t" + time3);
        Debug.Log("Dispatch後　　\t" + time4);
        Debug.Log("GPU→CPU転送後\t" + time5);

        //結果表示
        float calc_pi = 0.0f;
        for (int i = 0; i < bp * gp; i++)//最後の最後の結果はCPUで加算
        {
            calc_pi += host_ans[i];
        }
        calc_pi = Mathf.Sqrt(calc_pi * 6.0f);
        Debug.Log("π =" + calc_pi.ToString("f10"));

        //解放
        vec1.Release();
        vec2.Release();
        ans.Release();
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