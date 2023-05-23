using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ThrottleUI : MonoBehaviour
{
    [SerializeField] private TMP_Text m_throttleValue;
    [SerializeField] private TMP_Text m_airspeedValue;
    [SerializeField] private MaikelsWings m_plane;

    void Start()
    {
        
    }

    void Update()
    {
        m_throttleValue.text = $"{m_plane.Throttle*100f:00.0}%";
        m_airspeedValue.text = $"{m_plane.AirSpeed:00.0}m/s";
    }
}
