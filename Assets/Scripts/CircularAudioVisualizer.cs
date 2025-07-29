using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CircularAudioVisualizer : MonoBehaviour {
    public AudioSource audioSource;
    public GameObject[] visualizerObjects;
    public int sampleSize = 512;
    public float radius = 5f;
    public float heightMultiplier = 20f;
    public FFTWindow fftWindow = FFTWindow.BlackmanHarris;

    public Gradient colorGradient;

    private float[] spectrumData;

    // 内部用の自動生成帯域
    private struct BandRange { public float minHz, maxHz; }
    private BandRange[] instrumentBands;

    void Start() {
        spectrumData = new float[sampleSize];
        GenerateEqualBands(0f, 2000f); // 🎯 0Hz～2000Hz を均等分割
        ArrangeCircle();
        ApplyGradientColors();
    }

    void GenerateEqualBands(float minFreq, float maxFreq) {
        int count = visualizerObjects.Length;
        instrumentBands = new BandRange[count];
        float step = (maxFreq - minFreq) / count;
        for (int i = 0; i < count; i++) {
            instrumentBands[i].minHz = minFreq + i * step;
            instrumentBands[i].maxHz = minFreq + (i + 1) * step;
        }
    }

    void ApplyGradientColors() {
        int count = visualizerObjects.Length;
        for (int i = 0; i < count; i++) {
            float t = (count == 1) ? 0f : (float)i / (count - 1);
            Color c = colorGradient.Evaluate(t);
            Renderer r = visualizerObjects[i].GetComponent<MeshRenderer>();
            if (r != null && r.material != null) {
                r.material.color = c;
                // Emission も使用する場合
                if (r.material.HasProperty("_EmissionColor")) {
                    r.material.SetColor("_EmissionColor", c);
                }
            }
        }
    }

    void Update() {
        audioSource.GetSpectrumData(spectrumData, 0, fftWindow);
        UpdateVisualizer();
    }

    void UpdateVisualizer(){
        int seg = visualizerObjects.Length;
        float sampleRate = AudioSettings.outputSampleRate;

        for(int i = 0; i < seg; i++) {
            float minHz = instrumentBands[i].minHz;
            float maxHz = instrumentBands[i].maxHz;

            int minIndex = Mathf.Clamp((int)(minHz / sampleRate * sampleSize * 2), 0, spectrumData.Length - 1);
            int maxIndex = Mathf.Clamp((int)(maxHz / sampleRate * sampleSize * 2), 0, spectrumData.Length - 1);

            float sum = 0f;
            int count = maxIndex - minIndex + 1;
            if(count <= 0) continue;

            for(int j = minIndex; j <= maxIndex; j++) {
                sum += spectrumData[j];
            }

            float avg = sum / count;
            Vector3 sc = visualizerObjects[i].transform.localScale;
            sc.y = 1 + avg * heightMultiplier;
            visualizerObjects[i].transform.localScale = sc;
        }
    }

    void ArrangeCircle() {
        int count = visualizerObjects.Length;
        float step = 360f / count;
        for (int i = 0; i < count; i++) {
            float ang = i * step * Mathf.Deg2Rad;
            visualizerObjects[i].transform.localPosition =
              new Vector3(Mathf.Cos(ang), 0, Mathf.Sin(ang)) * radius;
            visualizerObjects[i].transform.localRotation =
              Quaternion.LookRotation(visualizerObjects[i].transform.localPosition);
        }
    }
}
