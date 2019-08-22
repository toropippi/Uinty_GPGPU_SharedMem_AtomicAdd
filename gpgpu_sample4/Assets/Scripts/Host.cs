using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Host : MonoBehaviour
{
    public ComputeShader shader;
    void Start()
    {
        uint N = 1024;//256の倍数で
        uint[] host_B = { 0 };//初期値
        ComputeBuffer AtomicBUF = new ComputeBuffer(host_B.Length, sizeof(uint));
        int k = shader.FindKernel("sharedmem_samp1");
        AtomicBUF.SetData(host_B);

        //時間測定開始
        int time = Gettime();

        //引数をセット
        shader.SetBuffer(k, "atmicBUF", AtomicBUF);

        //初回カーネル起動
        Debug.Log("初回カーネル起動前" + (Gettime() - time));
        shader.Dispatch(k, (int)N/256, 1, 1);
        Debug.Log("初回カーネルDispatch後  " + (Gettime() - time));
        AtomicBUF.GetData(host_B);
        Debug.Log("初回カーネルGetData直後  " + (Gettime() - time));

        // こっちが本命。GPUで計算
        host_B[0] = 0;
        AtomicBUF.SetData(host_B);
        shader.Dispatch(k, (int)N/256, 1, 1);
        Debug.Log("本命カーネルDispatch直後 " + (Gettime() - time));

        // device to host
        AtomicBUF.GetData(host_B);

        //結果
        Debug.Log("本命カーネル確定終了    " + (Gettime() - time));
        Debug.Log("\nAtomic_Addの結果   "+host_B[0]);

        //解放
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