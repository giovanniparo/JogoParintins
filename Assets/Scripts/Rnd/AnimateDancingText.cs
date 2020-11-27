using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AnimateDancingText : MonoBehaviour
{
    private TextMeshProUGUI textMesh;
    private List<Animator> animators;
    public GameObject prefab;
    private Canvas canvas;

    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        textMesh.ForceMeshUpdate();
        canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();

        for(int n = 0; n < textMesh.textInfo.characterCount; n++)
        {
            TMP_CharacterInfo characterInformation = textMesh.textInfo.characterInfo[n];
            Vector3 instPos = GetCharCenter(characterInformation);
            GameObject currentChar = Instantiate(prefab);
            currentChar.transform.SetParent(canvas.transform);
            /*currentChar.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1.0f);
            currentChar.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1.0f);
            currentChar.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1.0f);*/
            currentChar.GetComponent<RectTransform>().localPosition = instPos;
        }
    }

    public Vector3 GetCharCenter(TMP_CharacterInfo characterInfo)
    {
        Vector3 center = Vector3.zero;

        float characterWidth = (characterInfo.topRight - characterInfo.topLeft).magnitude;
        float characterHeight = (characterInfo.topRight - characterInfo.bottomRight).magnitude;

        center = new Vector3(characterInfo.bottomLeft.x + characterWidth / 2.0f,
                             characterInfo.bottomLeft.y + characterHeight / 2.0f,
                             0.0f);

        Debug.Log(center);
        return center;
    }
}
