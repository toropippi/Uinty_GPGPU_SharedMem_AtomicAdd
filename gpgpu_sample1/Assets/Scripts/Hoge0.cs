using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Hoge0 : MonoBehaviour
{
    public ComputeShader shader;

    void Start()
    {
        float[] host_A = { 1f, 1f, 1f, 1f };
        float[] host_B = { 1f, 1f, 1f, 1f };
        float[] host_C = { 0f, 0f, 0f, 0f };

        ComputeBuffer A = new ComputeBuffer(host_A.Length, sizeof(float));
        ComputeBuffer B = new ComputeBuffer(host_B.Length, sizeof(float));
        ComputeBuffer C = new ComputeBuffer(host_C.Length, sizeof(float));

        int k = shader.FindKernel("CSMain");

        // host to device
        A.SetData(host_A);
        B.SetData(host_A);

        //引数をセット
        shader.SetBuffer(k, "A", A);
        shader.SetBuffer(k, "B", B);
        shader.SetBuffer(k, "C", C);

        // GPUで計算
        shader.Dispatch(k, 1, 1, 1);

        // device to host
        C.GetData(host_C);

        Debug.Log("GPU上で計算した結果");
        for (int i = 0; i < host_C.Length; i++)
        {
            Debug.Log(host_A[i] + ", " + host_B[i] + ", " + host_C[i]);
        }

        //解放
        A.Release();
        B.Release();
        C.Release();
    }


    // Update is called once per frame
    void Update()
    {

    }


}