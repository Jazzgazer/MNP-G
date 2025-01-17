﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Advertisements;
using IgaworksUnityAOS;


public class LogoCtrl : MonoBehaviour {

    [SerializeField]
    GameObject _logo;

    [SerializeField]
    GameObject _warning;

    float _screenRatio;

    bool _waiting = false;

    void Awake() {

        /* 운영체제별 해상도 처리 */
        _screenRatio = (float)Screen.width / (float)Screen.height;
        //Debug.Log("▶▶▶▶ Resolution Width / Heigh , Ratio :: " + Screen.width + " / " + Screen.height + ", " + _screenRatio);
        

#if UNITY_IOS

        //iPad 일때는 해상도 조정을 하지 않음. 
        ISN_Device device = ISN_Device.CurrentDevice;
        if (!device.Model.Contains("iPad")) {
            Screen.SetResolution(Screen.height / 16 * 9, Screen.height, true);
        }

#else

        //안드로이드 해상도 처리 


        // 3:4 비율은 SetResolution을 사용하지 않고 좌우 검은 Letterbox 처리 
        if (_screenRatio > 0.7f) {

            //Debug.Log("▶▶▶ Android 3:4 ");

            return;
        } else {

            //Debug.Log("▶▶▶ Android SetResolution ");

            Screen.SetResolution(Screen.height / 16 * 9, Screen.height, true);
        }

#endif


    }


    // Use this for initialization
    void Start () {

        _waiting = false;

        //ScissorCtrl.Instance.UpdateResolution();

        PuzzleConstBox.Initialize();

		//LoadLobby ();
		StartCoroutine (Waiting ());

	}



    IEnumerator Waiting() {
        Debug.Log(">>> Waiting Start");


#if UNITY_ANDROID



        /*
        GoogleCloudMessageService.Instance.Init();
        GoogleCloudMessageService.Instance.RgisterDevice();
        */

#elif UNITY_IOS
        ISN_RemoteNotificationsController.Instance.RegisterForRemoteNotifications((ISN_RemoteNotificationsRegistrationResult result) => {
            if (result.IsSucceeded) {
                Debug.Log("DeviceId: " + result.Token.DeviceId);
                WWWHelper.Instance.PushDeviceToken = result.Token.TokenString;
            }
            else {
                Debug.Log("Error: " + result.Error.Code + " / " + result.Error.Message);
            }
        });
        
        GameSystem.Instance.CancelAllLocalNotification();
        
#endif





        //Debug.Log(">>> Waiting Start #1");

        _logo.SetActive(true);
        

        yield return new WaitForSeconds(2);

        _logo.SetActive(false);
        _warning.gameObject.SetActive(true);

        yield return new WaitForSeconds(2);


        //Debug.Log(">>> Waiting Start #2");

        LoadTitleScene();
    }


	private void LoadLobby() {
		GameSystem.Instance.LoadLobbyScene ();
	}

	private void LoadTitleScene() {
		GameSystem.Instance.LoadTitleScene ();
        
	}


    public int GetSDKLevel() {

        int sdkLevel = 0;

        try {
            var clazz = AndroidJNI.FindClass("android.os.Build$VERSION");
            var fieldID = AndroidJNI.GetStaticFieldID(clazz, "SDK_INT", "I");
            sdkLevel = AndroidJNI.GetStaticIntField(clazz, fieldID);
        }
        catch(Exception e) {
            Debug.Log("★★★★ GetSDKLevel Exception ");

        }

        Debug.Log("★★★★ GetSDKLevel :: " + sdkLevel);

        return sdkLevel;
    }


    bool _needReadPhoneState = false;
    bool _needWriteExternal = false;
    bool _needReadExternal = false;







}
