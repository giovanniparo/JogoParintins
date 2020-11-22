using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StartText : MonoBehaviour
{
    private TextMeshProUGUI startText;

    int startTextCounter;

    private Vector3 initScale;

    private void Awake()
    {
        startText = GetComponent<TextMeshProUGUI>();
        initScale = transform.localScale;
        startTextCounter = 3;
        startText.text = startTextCounter.ToString();
    }

    public void UpdateStartText()
    {
        startTextCounter--;
        startText.text = startTextCounter.ToString();
        transform.localScale = initScale;
        if (startTextCounter == 0)
            Destroy(this.gameObject);
    }
}
