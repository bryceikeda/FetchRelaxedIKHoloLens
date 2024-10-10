using UnityEngine;

// https://gamedev.stackexchange.com/questions/180126/how-to-cache-the-main-camera-as-a-global-variable
public static class Utils
{
    static Camera _mainCamera;
    public static Camera MainCamera => _mainCamera != null ? _mainCamera : (_mainCamera = Camera.main);
}
