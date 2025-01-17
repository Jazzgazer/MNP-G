﻿using UnityEngine;

public class AspectUtility : MonoBehaviour {

    //public float _wantedAspectRatio = 1.7777778f;
    public float _wantedAspectRatio = 0.5625f;
    static float wantedAspectRatio;
    static Camera cam;

    static Camera backgroundCam;
    static Camera topCam;

    void Awake() {
        topCam = null;

#if !UNITY_EDITOR && !UNITY_IOS
        return;
#endif

        cam = Camera.main;
        backgroundCam = GameObject.FindGameObjectWithTag("BackgroundCamera").GetComponent<Camera>();
        wantedAspectRatio = _wantedAspectRatio;

        if(InGameCtrl.Instance != null) {
            topCam = GameObject.FindGameObjectWithTag("TopCamera").GetComponent<Camera>();
        }


        SetCamera();
    }

    public static void SetCamera() {
        float currentAspectRatio = (float)Screen.width / Screen.height;
        
        //Debug.Log("▶ wantedAspectRatio currentAspectRatio:: " + Mathf.Round;
        Debug.Log("▶ wantedAspectRatio currentAspectRatio:: " + currentAspectRatio);

        // 근사치내에서는 아무것도 하지 않음 
        if(currentAspectRatio - wantedAspectRatio <= 0.01f && currentAspectRatio - wantedAspectRatio >= -0.01f) {
            return;
        }

        // If the current aspect ratio is already approximately equal to the desired aspect ratio,
        // use a full-screen Rect (in case it was set to something else previously)
        /*
        if ((int)(currentAspectRatio * 100) / 100.0f == (int)(wantedAspectRatio * 100) / 100.0f) {
            cam.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
            if (backgroundCam) {
                Destroy(backgroundCam.gameObject);
            }
            return;
        }
        */
        Debug.Log("▶ wantedAspectRatio Adjust Screen");

        // Pillarbox
        if (currentAspectRatio < wantedAspectRatio) {
            float inset = 1.0f - wantedAspectRatio / currentAspectRatio;
            cam.rect = new Rect(inset / 2, 0.0f, 1.0f - inset, 1.0f);

            if(topCam != null)
                topCam.rect = new Rect(inset / 2, 0.0f, 1.0f - inset, 1.0f);

        }
        // Letterbox
        else {
            float inset = 1.0f - currentAspectRatio / wantedAspectRatio;
            cam.rect = new Rect(0.0f, inset / 2, 1.0f, 1.0f - inset);

            if (topCam != null)
                topCam.rect = new Rect(0.0f, inset / 2, 1.0f, 1.0f - inset);

        }

        if (!backgroundCam) {
            // Make a new camera behind the normal camera which displays black; otherwise the unused space is undefined
            backgroundCam = new GameObject("LetterBoxCam").AddComponent<Camera>();
            //backgroundCam = new Camera();
            backgroundCam.depth = cam.depth - 1;
            backgroundCam.clearFlags = CameraClearFlags.SolidColor;
            backgroundCam.backgroundColor = Color.black;
            backgroundCam.cullingMask = 0;

        }
    }

    public static int screenHeight {
        get {
            return (int)(Screen.height * cam.rect.height);
        }
    }

    public static int screenWidth {
        get {
            return (int)(Screen.width * cam.rect.width);
        }
    }

    public static int xOffset {
        get {
            return (int)(Screen.width * cam.rect.x);
        }
    }

    public static int yOffset {
        get {
            return (int)(Screen.height * cam.rect.y);
        }
    }

    public static Rect screenRect {
        get {
            return new Rect(cam.rect.x * Screen.width, cam.rect.y * Screen.height, cam.rect.width * Screen.width, cam.rect.height * Screen.height);
        }
    }

    public static Vector3 mousePosition {
        get {
            Vector3 mousePos = Input.mousePosition;
            mousePos.y -= (int)(cam.rect.y * Screen.height);
            mousePos.x -= (int)(cam.rect.x * Screen.width);
            return mousePos;
        }
    }

    public static Vector2 guiMousePosition {
        get {
            Vector2 mousePos = Event.current.mousePosition;
            mousePos.y = Mathf.Clamp(mousePos.y, cam.rect.y * Screen.height, cam.rect.y * Screen.height + cam.rect.height * Screen.height);
            mousePos.x = Mathf.Clamp(mousePos.x, cam.rect.x * Screen.width, cam.rect.x * Screen.width + cam.rect.width * Screen.width);
            return mousePos;
        }
    }
}
