using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SC_CircularLoading : MonoBehaviour
{
    private Image loadingImage;
    //[SerializeField] Text loadingText;
    [Range(0, 1)]
    public float loadingProgress = 0;

    private void Start()
    {
        loadingImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        loadingImage.fillAmount = loadingProgress;
        if (loadingProgress < 1)
        {
            //loadingText.text = Mathf.RoundToInt(loadingProgress * 100) + "%\nLoading...";
        }
        else
        {
            //loadingText.text = "Done.";
        }
    }
}
