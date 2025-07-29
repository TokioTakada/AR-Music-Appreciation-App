using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSpectrumMaterialSetter : MonoBehaviour
{
    [SerializeField] private GetAudioData audioData;
    [SerializeField] private Material mat;
    [Tooltip("バー16本にそれぞれサンプル番号(周波数)を割り当てる、数字は0以上GetSpectrum.FFT_res以下。サンプルごとの実際の周波数はFk = outputSampleRate * 0.5 * sampleNum / FFT_res")]
    [SerializeField] private int[] indices;
    [SerializeField] private float amp = 10;

    private int res;
    public Renderer mesh;
    private void OnEnable()
    {
        res = (int)audioData.FFT_res;
        if(mesh != null) mat.SetFloat("_Width", Mathf.Abs(mesh.bounds.size.x/mesh.transform.lossyScale.x));
    }

    public void FixedUpdate()
    {
        RefreshMaterial();
    }

    private void RefreshMaterial()
    {
        for (var i = 0; i < indices.Length; i++)
        {
            float dat = audioData.spectrumData[Mathf.Clamp(indices[i], 0, res - 1)];
            mat.SetFloat("_Param" + i.ToString(), Mathf.Clamp01(dat * amp));            
        }
    }

    private void Reset()
    {
        for (int i = 0; i < 16; i++) mat.SetFloat("_Param" + i.ToString(), 0);
    }
}