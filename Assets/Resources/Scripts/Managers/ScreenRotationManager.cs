using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Скрипт пределяет ориентацию экрана в приложении
/// </summary>
public class ScreenRotationScript : MonoBehaviour
{
    [SerializeField]
    public bool _allowPortraitOrientation;
    [SerializeField]
    public bool _allowPortraitUpsideDownOrientation;
    [SerializeField]
    public bool _allowLandscapeLeftOrientation;
    [SerializeField]
    public bool _allowPortraitLandscapeRightOrientation;

    private void Start()
    {
        SetScreenOrientation();
        Destroy(gameObject);
    }
    private void SetScreenOrientation()
    {
        if (!_allowPortraitOrientation
            && !_allowPortraitUpsideDownOrientation
            && !_allowLandscapeLeftOrientation
            && !_allowPortraitLandscapeRightOrientation)
        {
            Screen.orientation = ScreenOrientation.AutoRotation;
            return;
        }

        Screen.autorotateToPortrait = _allowPortraitOrientation;
        Screen.autorotateToPortraitUpsideDown = _allowPortraitUpsideDownOrientation;
        Screen.autorotateToLandscapeLeft = _allowLandscapeLeftOrientation;
        Screen.autorotateToLandscapeRight = _allowPortraitLandscapeRightOrientation;
    }    
}
