using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Host : MonoBehaviour
{
    public ComputeShader shader;
    ComputeBuffer A;
    int cnt;
    int time0;
    int time1;
    uint[] host_A;
    int[] k;
    int N;
    int[] timelist;
    int knum;
    int THREADNUM;
    void Start()
    {
        N = 9;
        THREADNUM = 256 * 1024;
        timelist = new int[N];
        host_A = new uint[THREADNUM];
        A = new ComputeBuffer(host_A.Length, sizeof(uint));
        k = new int[9];
        k[0] = shader.FindKernel("sharedmem_samp1");
        k[1] = shader.FindKernel("sharedmem_samp2");
        k[2] = shader.FindKernel("sharedmem_samp4");
        k[3] = shader.FindKernel("sharedmem_samp8");
        k[4] = shader.FindKernel("sharedmem_samp16");
        k[5] = shader.FindKernel("sharedmem_samp32");
        k[6] = shader.FindKernel("sharedmem_samp64");
        k[7] = shader.FindKernel("sharedmem_samp128");
        k[8] = shader.FindKernel("sharedmem_samp256");
        //引数をセット
        for (int i = 0; i < 9; i++)
        {
            shader.SetBuffer(k[i], "A", A);
        }
        cnt = 0;
        knum = 0;
    }


    void Update()
    {
        if (cnt < N)
        {
            gpuvoid();
        }

        if (cnt == N)
        {
            Array.Sort(timelist);//中央値を選択したい
            Debug.Log("グループスレッド数=" + (1 << knum) + "\n計算時間        " + timelist[N / 2] + "ms");

            knum++;
            if (knum == 9)
            {
                A.Release();
            }
            else
            {
                cnt = -1;
            }
        }

        cnt++;

    }

    void gpuvoid()
    {
        time0 = Gettime();
        // GPUで計算
        shader.Dispatch(k[knum], THREADNUM >> knum, 1, 1);
        // device to host
        A.GetData(host_A);
        time1 = Gettime() - time0;
        timelist[cnt] = time1;
    }

    //現在の時刻をms単位で取得
    int Gettime()
    {
        return DateTime.Now.Millisecond + DateTime.Now.Second * 1000
         + DateTime.Now.Minute * 60 * 1000 + DateTime.Now.Hour * 60 * 60 * 1000;
    }
}