using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ThrottleUI : MonoBehaviour
{
    [SerializeField] private TMP_Text m_value;
    [SerializeField] private MaikelsWings m_plane;

    void Start()
    {
        
    }

    void Update()
    {
        m_value.text = $"{m_plane.Throttle*100f:00.0}%";
    }
}
