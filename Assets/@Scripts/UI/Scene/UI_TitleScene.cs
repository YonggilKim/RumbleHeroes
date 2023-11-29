using System.Collections;
using UnityEngine;
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

    public enum EState
    {
        None = 0,
        CalculatingSize,
        NothingToDownload,
        AskingDownload,
        Downloading,
        DownloadFinished
    }

    DownloadComponent DownloadComp;
    DownloadProgressStatus progressInfo;
    SizeUnits sizeUnit;
    long curDownloadedSizeInUnit;
    long totalSizeInUnit;

    private EState _currentState = EState.None;

    public EState CurrentState
    {
        get => _currentState;
        set
        {
            _currentState = value;
            UpdateUI();
        }
    }


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

        DownloadComp = gameObject.GetOrAddComponent<DownloadComponent>();
        DownloadComp.DownloadLabel = "Preload";
        return true;
    }

    IEnumerator Start()
    {
// #if UNITY_EDITOR
//         CurrentState = EState.DownloadFinished;
//         yield return null;
// #endif
        // Addressables.ResourceManager.
        yield return DownloadComp.StartDownloadRoutine((events) =>
        {
            events.SystemInitializedListener += OnInitialized;
            events.CatalogUpdatedListener += OnCatalogUpdated;
            events.SizeDownloadedListener += OnSizeDownloaded;
            events.DownloadProgressListener += OnDownloadProgress;
            events.DownloadFinished += OnDownloadFinished;
        });
    }

    void UpdateUI()
    {
        switch (CurrentState)
        {
            case EState.CalculatingSize:
                Debug.Log("다운로드 정보를 가져오고 있습니다. 잠시만 기다려주세요.");
                break;
            case EState.NothingToDownload:
                Debug.Log("다운로드 받을 데이터가 없습니다.");
                break;
            case EState.AskingDownload:
                Debug.Log(
                    $"다운로드를 받으시겠습니까 ? 데이터가 많이 사용될 수 있습니다. <color=green>({$"{this.totalSizeInUnit}{this.sizeUnit})</color>"}");
                break;
            case EState.Downloading:
                // Debug.Log( $"다운로드중입니다. 잠시만 기다려주세요. {(progressInfo.totalProgress * 100).ToString("0.00")}% 완료");
                break;
            case EState.DownloadFinished:
                Debug.Log("다운로드가 완료되었습니다. 게임을 진행하시겠습니까?");

                // Load 시작
                Managers.Resource.LoadAllAsync<Object>("Preload", (key, count, totalCount) =>
                {
                    Debug.Log($"{key} {count}/{totalCount}");

                    if (count == totalCount)
                    {
                        Managers.Data.Init();
                    }
                });
                break;
        }
    }

    private void OnInitialized()
    {
        DownloadComp.GoNext();
    }

    private void OnCatalogUpdated()
    {
        DownloadComp.GoNext();
    }

    private void OnSizeDownloaded(long size)
    {
        Debug.Log($"다운로드 사이즈 다운로드 완료 ! : {Util.GetConvertedByteString(size, SizeUnits.KB)} ({size}바이트)");

        if (size == 0)
        {
            CurrentState = EState.DownloadFinished;
        }
        else
        {
            sizeUnit = Util.GetProperByteUnit(size);
            totalSizeInUnit = Util.ConvertByteByUnit(size, sizeUnit);

            CurrentState = EState.AskingDownload;

            //TODO 일단 묻지않고 바로 다운로드
            CurrentState = EState.Downloading;
            DownloadComp.GoNext();
        }
    }

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

    private void OnDownloadFinished(bool isSuccess)
    {
        CurrentState = EState.DownloadFinished;
        DownloadComp.GoNext();
    }
}