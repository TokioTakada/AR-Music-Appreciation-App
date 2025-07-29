using UnityEngine;

public class RadialPlaneVisualizerWithGradient : MonoBehaviour
{
    public AudioSource audioSource;
    public GameObject[] visualizerPlanes;
    public int sampleSize = 512;
    public FFTWindow fftWindow = FFTWindow.BlackmanHarris;

    public float innerRadius = 2f;        // 棒の開始距離
    public float lengthMultiplier = 20f;
    public Gradient colorGradient;

    private float[] spectrumData;

    void Start()
    {
        spectrumData = new float[sampleSize];
        ArrangeCircle();
        ApplyGradientColors();
    }

    void ArrangeCircle()
    {
        int count = visualizerPlanes.Length;
        float step = 360f / count;
        for (int i = 0; i < count; i++)
        {
            float ang = i * step * Mathf.Deg2Rad;
            Vector3 dir = new Vector3(Mathf.Cos(ang), 0, Mathf.Sin(ang));
            var go = visualizerPlanes[i];

            go.transform.localRotation = Quaternion.LookRotation(dir);
        }
    }

    void ApplyGradientColors()
    {
        int count = visualizerPlanes.Length;
        for (int i = 0; i < count; i++)
        {
            float t = (count == 1) ? 0f : (float)i / (count - 1);
            Color c = colorGradient.Evaluate(t);
            var rend = visualizerPlanes[i].GetComponent<Renderer>();
            if (rend != null && rend.material != null)
            {
                rend.material.color = c;
                if (rend.material.HasProperty("_EmissionColor"))
                    rend.material.SetColor("_EmissionColor", c);
            }
        }
    }

    void Update()
    {
        audioSource.GetSpectrumData(spectrumData, 0, fftWindow);
        ExtendAndPositionPlanes();
    }

    void ExtendAndPositionPlanes()
    {
        int count = visualizerPlanes.Length;
        int segmentSize = sampleSize / count;
        float sampleRate = AudioSettings.outputSampleRate;

        for (int i = 0; i < count; i++)
        {
            float sum = 0f;
            for (int j = 0; j < segmentSize; j++)
                sum += spectrumData[i * segmentSize + j];
            float avg = sum / segmentSize;

            var plane = visualizerPlanes[i];
            Vector3 dir = plane.transform.forward;
            float length = 1f + avg * lengthMultiplier;

            // スケール更新
            Vector3 sc = plane.transform.localScale;
            sc.z = length;
            plane.transform.localScale = sc;

            // 始点が innerRadius に、端点が innerRadius + length に来るよう調整
            Vector3 startPos = dir * innerRadius;
            plane.transform.localPosition = startPos + dir * (length * 5f);
        }
    }
}
