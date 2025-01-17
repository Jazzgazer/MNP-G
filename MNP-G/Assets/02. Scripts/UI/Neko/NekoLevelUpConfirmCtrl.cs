﻿using UnityEngine;
using System.Collections;

public class NekoLevelUpConfirmCtrl : MonoBehaviour {


    OwnCatCtrl _currentNeko;

    [SerializeField]
    NekoUpCtrl _nekoLevelUpWindow;

    [SerializeField]
    UISprite _neko;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_pNeko"></param>
    public void SetNekoLevelUp(OwnCatCtrl _pNeko) {
        this.gameObject.SetActive(true);

        _currentNeko = _pNeko;
        _neko.atlas = _pNeko.CharacterAtlas;
        _neko.spriteName = _pNeko.CharacterSpriteName;

    }

    public void OnClickLevel1() {
        this.SendMessage("CloseSelf");
        _nekoLevelUpWindow.SetNekoLevelUp(_currentNeko, 1);
    }

    public void OnClickLevel10() {
        
        this.SendMessage("CloseSelf");
        _nekoLevelUpWindow.SetNekoLevelUp(_currentNeko, 10);
    }


}
