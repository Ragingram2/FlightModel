using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ThrottleUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text m_throttleValue;
    [SerializeField]
    private TMP_Text m_airspeedValue;
    [SerializeField]
    private TMP_Text m_sasValue;
    [SerializeField]
    private TMP_Text m_altitiudeValue;
    [SerializeField]
    private TMP_Text m_trimValue;
    [SerializeField]
    private MaikelsWings m_plane;


    void Update()
    {
        m_throttleValue.text = $"{m_plane.Throttle * 100f:00.0}%";
        m_airspeedValue.text = $"{m_plane.AirSpeed:00.0}m/s";
        m_sasValue.text = m_plane.SAS ? "Enabled" : "Disabled";
        if (m_trimValue)
            m_trimValue.text = $"{m_plane.Trim}";
        m_altitiudeValue.text = $"{m_plane.transform.position.y:00000.0}";
    }
}
