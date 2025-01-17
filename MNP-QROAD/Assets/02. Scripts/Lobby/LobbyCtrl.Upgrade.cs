﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathologicalGames;

public partial class LobbyCtrl : MonoBehaviour {

    // Upgrade 창 제어 
    bool _isReadyCharacterList = false;

	// Selective 창 제어 
	[SerializeField] UIPanel pnlSelectiveNekoScrollView; // 네코 선택창 ScrollView Panel
	[SerializeField] UIPanel pnlRankScrollView;
    [SerializeField] CatInformationCtrl _CharacterInfo;
	

    [SerializeField] GameObject _newNekoAddedSign;

    List<OwnCatCtrl> _listCharacterList = new List<OwnCatCtrl>();
    List<SixNekoSetCtrl> _listSixNekoSetList = new List<SixNekoSetCtrl>();
    [SerializeField] private UIGrid _grdCharacterList;

    readonly string POOL_CHARACTER = "CharacterPool";
    readonly string PREFAB_CHARACTER = "OwnCatPrefab";


    public bool IsReadyCharacterList {
        get {
            return _isReadyCharacterList;
        }

        set {
            _isReadyCharacterList = value;
        }
    }

    public List<OwnCatCtrl> ListCharacterList {
        get {
            return _listCharacterList;
        }

        set {
            _listCharacterList = value;
        }
    }

    public List<SixNekoSetCtrl> ListSixNekoSetList {
        get {
            return _listSixNekoSetList;
        }

        set {
            _listSixNekoSetList = value;
        }
    }

    void DisableNewNekoAddedSign() {
        _newNekoAddedSign.SetActive(false);
    }

    public void EnableNewNekoAddedSign() {
        _newNekoAddedSign.SetActive(true);
    }


	
	public void CloseCharacterList() {
        if(CatInformationCtrl.Instance != null && CatInformationCtrl.Instance.gameObject.activeSelf)
            CatInformationCtrl.Instance.SendMessage("CloseSelf");
		
	}

    #region 고양이 선택창 제어

    /// <summary>
    /// 로비에서 열리는 캐릭터 선택창 
    /// </summary>
    public void OpenCharacterList() {
        IsReadyCharacterList = false;

        DisableNewNekoAddedSign();
        OnCharacterList();
    }


    /// <summary>
    /// 레디창에서 열리는 캐릭터 선택창 
    /// </summary>
    public void OpenReadyCharacterList() {
        IsReadyCharacterList = true;

        OnCharacterList();
    }

    void OnCharacterList() {
        // CatInformationCtrl.Instance.OpenCatInformation();
        _CharacterInfo.OpenCatInformation();


        // SpawnCharacterList(IsReadyCharacterList);
        SpawnCharacterListBySix(IsReadyCharacterList);
    }



    /// <summary>
    /// 6개씩 끊어서 생성되는 리스트 
    /// </summary>
    /// <param name="pIsReadyOpen"></param>
    public void SpawnCharacterListBySix(bool pIsReadyOpen) {


        int count = 0;
        int setIndex = 0;
        int nekoIndex = 0;

        ListCharacterList.Clear();

        // 패널 설정
        // pnlSelectiveNekoScrollView.gameObject.GetComponent<UIScrollView>().ResetPosition();
        // pnlSelectiveNekoScrollView.clipOffset = Vector2.zero;
        // pnlSelectiveNekoScrollView.transform.localPosition = _nekoSelectScrollViewPos;


        // 캐릭터 정렬 처리
        if (GameSystem.Instance.LoadGradeOrder()) {
            GameSystem.Instance.SortUserNekoByBead(); // 등급순 
        }
        else {
            GameSystem.Instance.SortUserNekoByGet(); // 획득순
        }

        // 몇개의 캐릭터 세트가 필요한가? 
        count = (GameSystem.Instance.ListSortUserNeko.Count / 6);

        if (GameSystem.Instance.ListSortUserNeko.Count % 6 > 0) // 나머지가 있으면 1을 추가한다. 
            count++;


        for (int i = 0; i < count; i++) {
            
            if(nekoIndex + 5 < GameSystem.Instance.ListSortUserNeko.Count) {
                ListSixNekoSetList[setIndex].InitList(nekoIndex, nekoIndex + 5);
                
            }
            else {
                ListSixNekoSetList[setIndex].InitList(nekoIndex, GameSystem.Instance.ListSortUserNeko.Count - 1);
            }

            ListSixNekoSetList[setIndex].SetPos(setIndex);

            setIndex++;
            nekoIndex += 6;
        }

        


        // 자동으로 선택 처리 
        if (pIsReadyOpen) {

            if(CatInformationCtrl.Instance != null) {
                CatInformationCtrl.Instance.OnCenterForce(ListSixNekoSetList[FindSixNekoIndexByNekoID(ReadyGroupCtrl.Instance.GetEquipedNekoID())].gameObject);
                CatInformationCtrl.Instance.SetNekoByNekoID(ReadyGroupCtrl.Instance.GetEquipedNekoID());
            } 

        }
        else {
            // 처음에는 이벤트를 강제로 발생
            if (CatInformationCtrl.Instance != null) 
                CatInformationCtrl.Instance.OnCenterForce(ListSixNekoSetList[0].gameObject);
        }
    }

    int FindSixNekoIndexByNekoID(int pNekoID) {
        for(int i=0; i<_listSixNekoSetList.Count; i++) {
            if (_listSixNekoSetList[i].CheckExistsNekoByID(pNekoID))
                return i;
        }

        return 0;
    }



    /// <summary>
    /// 캐릭터 리스트 비활성화
    /// </summary>
    public void ClearCharacterList() {

        for(int i=0; i<ListSixNekoSetList.Count; i++) {
            ListSixNekoSetList[i].SetDisable();
        }
    }


    /// <summary>
    /// 고양이 캐릭터 리스트를 준비한다.
    /// </summary>
    private void SetReadyCharacterList() {

        for(int i=0; i<20; i++) {
            ListSixNekoSetList.Add(PoolManager.Pools[POOL_CHARACTER].Spawn(PuzzleConstBox.prefabSixNekoSet).GetComponent<SixNekoSetCtrl>());
        }


        ClearCharacterList();
    }





    /// <summary>
    /// 첫번째 고양이 활성화하기 (튜토리얼)
    /// </summary>
    private void SetFirstNekoEnableInTutorial() {


        Debug.Log("SetFirstNekoEnableInTutorial");

        DisableAllButton();

        for (int i = 0; i < ListCharacterList.Count; i++) {
            ListCharacterList[i].GetComponent<UIButton>().enabled = false;
        }

        // 첫번째 고양이만 활성화
        ListCharacterList[0].GetComponent<UIButton>().enabled = true;
    }

    
    #endregion

    #region 과일 업그레이드 창 제어 

    /// <summary>
    /// 업그레이드 팝업창 호출 
    /// </summary>
    /// <param name="pAbilityIndex">P ability index.</param>
    public void OpenUpgradePopup(int pAbilityIndex, int pPrice) {

		// 골드가 부족한 경우 구매 팝업. 
		if (GameSystem.Instance.UserGold < pPrice) {
			//objPopupNeedCoin.gameObject.SetActive(true);

			OpenInfoPopUp(PopMessageType.GoldShortage);
			return;
		}
	}


	
	#endregion


}
