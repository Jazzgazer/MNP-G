﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using BestHTTP;

public class MaintenanceCtrl : MonoBehaviour {

    public UIToggle _maintenanceChecker;
    public UIInput _inputMaintenanceMessage;

    public MessageBoxCtrl _messageBox;


    void Start() {
        WWWHelper.Instance.Post2WithJSON("request_checkundermaintenance", OnFinishedCheckMaintenance, null);
    }

    public void CheckMaintenance() {
        Debug.Log("★★ CheckMaintenance isChecked :: " + _maintenanceChecker.value);
        Debug.Log("★★ _inputMaintenanceMessage value :: " + _inputMaintenanceMessage.value);

        string msg = "확인해주세요! \n";

        if (_maintenanceChecker.value) {
            msg += "추가 점검 메세지는 다음과 같습니다.\n\n";
            msg += "[" + _inputMaintenanceMessage.value + "]";

            msg += "점검을 시작하시게습니까?";
        }
        else {
            msg += "점검을 종료하시겠습니까?";
        }





        _messageBox.OpenMessageBox(SendMaintenanceInfo, CancelSend, msg);

    }

    void SendMaintenanceInfo() {
        Debug.Log("Sending Maintenance Info");

        JSONNode node = JSON.Parse("{}");
        node["checked"].AsBool = _maintenanceChecker.value;
        node["message"] = _inputMaintenanceMessage.value;

        WWWHelper.Instance.Post2WithJSON("request_setmaintenancesimple", OnFinishedSetMaintenanceSimple, node);
    }

    void CancelSend() {
        WWWHelper.Instance.Post2WithJSON("request_checkundermaintenance", OnFinishedCheckMaintenance, null);
    }


    private void OnFinishedSetMaintenanceSimple(HTTPRequest request, HTTPResponse response) {

        JSONNode result = JSON.Parse(response.DataAsText);

        Debug.Log("★★ OnFinishedSetMaintenanceSimple :: " + result.ToString());


        result = result["data"];

        

    }


    private void OnFinishedCheckMaintenance(HTTPRequest request, HTTPResponse response) {

        JSONNode result = JSON.Parse(response.DataAsText);

        Debug.Log("★★ OnFinishedCheckMaintenance :: " + result.ToString());
        result = result["data"];

        if (result["ismaintenance"].AsBool) {
            _maintenanceChecker.value = true;
            _inputMaintenanceMessage.value = result["msg"];
        }
        else {
            _maintenanceChecker.value = false;
        }

    }


}
