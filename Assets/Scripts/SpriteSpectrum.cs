using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteSpectrum : MonoBehaviour
{
    [SerializeField] AudioSpectrum spectrum;
    //オブジェクトの配列（
    [SerializeField] RectTransform[] cubes;
    //スペクトラムの高さ倍率
    [SerializeField] float scale;

    // Update is called once per frame
    private void Update()
    {
        int i = 0;

        foreach (var cube in cubes)
        {
            //オブジェクトのスケールを取得
            var localScale = cube.localScale;
            //スペクトラムのレベル＊スケールをYスケールに置き換える
            localScale.y = spectrum.Levels[i] * scale;
            cube.localScale = localScale;
            i++;
        }
    }
}
