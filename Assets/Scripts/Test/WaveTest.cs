using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveTest : MonoBehaviour
{
    [SerializeField] private Sprite[] waveSpriteArr;
    [SerializeField] private SpriteRenderer AnimSprite;
    [SerializeField] private float AnimSpeed;

    public void AnimStart()
    {
        StartCoroutine(WaveAnim());
    }

    
    private IEnumerator WaveAnim()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(AnimSpeed);

        for (int i = 0; i < waveSpriteArr.Length; i++)
        {
            AnimSprite.sprite = waveSpriteArr[i];
            yield return waitForSeconds;
        }

        yield break;
    }
}
