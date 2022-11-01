using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class Camera2D : MonoBehaviour
{
    public Camera[] CameraArray;
    public int Width;
    public int Height;
    public ScreenOrientation[] OrientationArray;

    private Camera _mainCamera;

    // Start is called before the first frame update
    private void Start()
    {
        ChangeOrto();
    }

    // just for debug in editor
#if UNITY_EDITOR
    private void Update()
    {
        ChangeOrto();
    }
#endif

    private void ChangeOrto()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }

        float baseOrtoSize = Height * 1.0f / 200;
        float ortoSize = baseOrtoSize;
        if (OrientationArray.Contains(Screen.orientation))
        {
            ortoSize = baseOrtoSize * (1 + (Width * 1.0f / Height - Screen.width * 1.0f / Screen.height));
        }

        for (int i = 0; i < CameraArray.Length; i++)
        {
            CameraArray[i].orthographicSize = ortoSize;
        }
    }
}
