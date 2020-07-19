using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Hoge0 : MonoBehaviour
{
    public ComputeShader shader;

    void Start()
    {
        int[] host_A = { 2, 2, 2 };//これがグループサイズx,y,zに対応
        int[] host_B = new int[16];

        ComputeBuffer A = new ComputeBuffer(host_A.Length, sizeof(int), ComputeBufferType.IndirectArguments);
        ComputeBuffer B = new ComputeBuffer(host_B.Length, sizeof(int));

        int k = shader.FindKernel("CSMain");

        A.SetData(host_A);
        B.SetData(host_B);

        //引数をセット
        shader.SetBuffer(k, "B", B);

        // GPUで計算
        //shader.Dispatch(k, 2, 2, 2);と同じ
        shader.DispatchIndirect(k, A, 0);

        // device to host
        B.GetData(host_B);

        Debug.Log("GPU上で計算した結果");
        for (int i = 0; i < host_B.Length; i++)
        {
            Debug.Log(host_B[i]);
        }


        //解放
        A.Release();
        B.Release();
    }
}