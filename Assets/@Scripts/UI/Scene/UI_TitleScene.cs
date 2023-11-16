using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using static Define;

public class UI_TitleScene : UI_Scene
{
    #region Enum

    private enum GameObjects
    {
        StartButton
    }

    private enum Buttons
    {
    }


    private enum Texts
    {
    }

    #endregion

    public enum State
    {
        None = 0,
        CalculatingSize,   
        NothingToDownload, 
        AskingDownload,  
        Downloading,    
        DownloadFinished 
    }

    DownloadComponent Downloader;
    DownloadProgressStatus progressInfo;
    SizeUnits sizeUnit;
    long curDownloadedSizeInUnit;
    long totalSizeInUnit;

    /// <summary> 현재 상태 </summary>
    public State CurrentState { get; private set; } = State.None;

    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObject(typeof(GameObjects));

        GetObject((int)GameObjects.StartButton).BindEvent(() =>
        {
            Debug.Log("OnClick");
            Managers.Scene.LoadScene(EScene.GameScene);
        });

        Downloader = gameObject.GetOrAddComponent<DownloadComponent>();

        return true;
    }

    IEnumerator Start()
    {
        // SetState(State.CalculatingSize, true);

        // yield return Downloader.StartDownloadRoutine((events) =>
        // {
        //     events.SystemInitializedListener += OnInitialized;
        //     events.CatalogUpdatedListener += OnCatalogUpdated;
        //     events.SizeDownloadedListener += OnSizeDownloaded;
        //     events.DownloadProgressListener += OnDownloadProgress;
        //     events.DownloadFinished += OnDownloadFinished;
        //     
        //
        // });
        yield return null;
        Managers.Resource.LoadAllAsync<Object>("Preload", (key, count, totalCount) =>
        {
            Debug.Log($"{key} {count}/{totalCount}");
            
            if (count == totalCount)
            {
                Managers.Data.Init();   
            }
        });
    }

    void UpdateUI()
    {
        if (CurrentState == State.CalculatingSize)
        {
            Debug.Log( "다운로드 정보를 가져오고 있습니다. 잠시만 기다려주세요.");
        }
        else if (CurrentState == State.NothingToDownload)
        {
            Debug.Log( "다운로드 받을 데이터가 없습니다.");
        }
        else if (CurrentState == State.AskingDownload)
        {
            Debug.Log($"다운로드를 받으시겠습니까 ? 데이터가 많이 사용될 수 있습니다. <color=green>({$"{this.totalSizeInUnit}{this.sizeUnit})</color>"}");
        }
        else if (CurrentState == State.Downloading)
        {
            Debug.Log( $"다운로드중입니다. 잠시만 기다려주세요. {(progressInfo.totalProgress * 100).ToString("0.00")}% 완료");
        }
        else if (CurrentState == State.DownloadFinished)
        {
            Debug.Log( "다운로드가 완료되었습니다. 게임을 진행하시겠습니까?");
        }
    }
    
    /// <summary> 초기화 완료시 호출 </summary>
    private void OnInitialized()
    {
        Downloader.GoNext();
    }

    /// <summary> 카탈로그 업데이트 완료시 호출 </summary>
    private void OnCatalogUpdated()
    {
        Downloader.GoNext();
    }

    /// <summary> 사이즈 다운로드 완료시 호출 </summary>
    private void OnSizeDownloaded(long size)
    {
        Debug.Log($"다운로드 사이즈 다운로드 완료 ! : {size} 바이트");

        if (size == 0)
        {
            CurrentState = State.NothingToDownload;
        }
        else
        {
            sizeUnit = Util.GetProperByteUnit(size);
            totalSizeInUnit = Util.ConvertByteByUnit(size, sizeUnit);

            CurrentState = State.AskingDownload;
        }
    }

    /// <summary> 다운로드 진행중 호출 </summary>
    private void OnDownloadProgress(DownloadProgressStatus newInfo)
    {
        bool changed = this.progressInfo.downloadedBytes != newInfo.downloadedBytes;

        progressInfo = newInfo;

        if (changed)
        {
            UpdateUI();

            curDownloadedSizeInUnit = Util.ConvertByteByUnit(newInfo.downloadedBytes, sizeUnit);
        }
    }

    /// <summary> 다운로드 마무리시 호출 </summary>
    private void OnDownloadFinished(bool isSuccess)
    {
        Debug.Log("다운로드 완료 ! 결과 : " + isSuccess);

        CurrentState = State.DownloadFinished;
        Downloader.GoNext();
    }
}