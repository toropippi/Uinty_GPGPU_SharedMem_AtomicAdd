using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Host : MonoBehaviour
{
    public ComputeShader shader;
    void Start()
    {
        float[] host_A = new float[256];
        for(int i = 0; i < 256; i++)
        {
            host_A[i] = 1f * i;
        }
        float[] host_B = { 0f };//この数字はなんでもいい。Bは結果がはいる側

        ComputeBuffer A = new ComputeBuffer(host_A.Length, sizeof(float));
        ComputeBuffer B = new ComputeBuffer(host_B.Length, sizeof(float));

        int k = shader.FindKernel("sharedmem_samp0");

        // host to device
        A.SetData(host_A);

        //引数をセット
        shader.SetBuffer(k, "A", A);
        shader.SetBuffer(k, "B", B);

        // GPUで計算
        shader.Dispatch(k, 1, 1, 1);//ここでは1*1*1並列を指定。ComputeShader側で256並列を指定している

        // device to host
        B.GetData(host_B);
        Debug.Log(host_B[0]);

        //解放
        A.Release();
        B.Release();
    }

    // Update is called once per frame
    void Update()
    {
    }
}