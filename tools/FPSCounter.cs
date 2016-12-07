using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    const float fpsMeasurePeriod = 0.5f;
    private int m_FpsAccumulator = 0;
    private float m_FpsNextPeriod = 0;
    private int m_CurrentFps;
    const string display = "{0} FPS";

    public bool Show { get; set; }

    private void Start()
    {
        m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
    }


    private void Update()
    {
        //measure average frames per second
        m_FpsAccumulator++;
        if (Time.realtimeSinceStartup > m_FpsNextPeriod)
        {
            m_CurrentFps = (int)(m_FpsAccumulator / fpsMeasurePeriod);
            m_FpsAccumulator = 0;
            m_FpsNextPeriod += fpsMeasurePeriod;
        }
    }

    void OnGUI()
    {
        GUI.skin.label.normal.textColor = new Color(0, 255.0f / 255, 0, 1.0f);
        GUI.skin.label.fontSize = 25;
        GUI.skin.label.alignment = TextAnchor.UpperLeft;
        GUI.Label(new Rect(Screen.width / 2, 10, 250, 50), string.Format(display, m_CurrentFps));
    }
}

