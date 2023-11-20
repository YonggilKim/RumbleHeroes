using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary> Addressable 다운로드 수행 클래스 </summary>
public class AddressableDownloader
{
    public static string DownloadURL;
    DownloadEvents _events;
    string _labelToDownload;
    long _totalSize;
    AsyncOperationHandle _downloadHandle;

    /// <summary> Addressable 시스템 초기화하기 </summary>
    public DownloadEvents InitializedSystem(string label, string downloadURL)
    {
        _events = new DownloadEvents();

        Addressables.InitializeAsync().Completed += OnInitialized;

        DownloadURL = downloadURL;
        _labelToDownload = label;

        UnityEngine.ResourceManagement.ResourceManager.ExceptionHandler += OnException;

        return _events;
    }

    public void UpdateCatalog()
    {
        Addressables.CheckForCatalogUpdates().Completed += (result) =>
        {
            var catalogToUpdate = result.Result;
            if (catalogToUpdate.Count > 0)
            {
                Addressables.UpdateCatalogs(catalogToUpdate).Completed += OnCatalogUpdated;
            }
            else
            {
                _events.NotifyCatalogUpdated();
            }
        };
    }

    public void DownloadSize()
    {
        Addressables.GetDownloadSizeAsync(_labelToDownload).Completed += OnSizeDownloaded;
    }

    public void StartDownload()
    {
        _downloadHandle = Addressables.DownloadDependenciesAsync(_labelToDownload);
        _downloadHandle.Completed += OnDependenciesDownloaded;
    }

    public void Update()
    {
        if (_downloadHandle.IsValid()
            && _downloadHandle.IsDone == false
            && _downloadHandle.Status != AsyncOperationStatus.Failed)
        {
            var status = _downloadHandle.GetDownloadStatus();

            long curDownloadedSize = status.DownloadedBytes;
            long remainedSize = _totalSize - curDownloadedSize;

            _events.NotifyDownloadProgress(new DownloadProgressStatus(
                status.DownloadedBytes
                , _totalSize
                , remainedSize
                , status.Percent));
        }
    }

    //--------------------------------------------------------//

    /// <summary> 초기화 완료시 호출 </summary>
    private void OnInitialized(AsyncOperationHandle<IResourceLocator> result)
    {
        _events.NotifyInitialized();
    }

    /// <summary> 카탈로그 업데이트 완료시 호출 </summary>
    void OnCatalogUpdated(AsyncOperationHandle<List<IResourceLocator>> result)
    {
        _events.NotifyCatalogUpdated();
    }

    /// <summary> 사이즈 다운로드 완료시 호출 </summary>
    void OnSizeDownloaded(AsyncOperationHandle<long> result)
    {
        _totalSize = result.Result;
        _events.NotifySizeDownloaded(result.Result);
    }

    /// <summary> 번들 다운로드 완료시 호출 </summary>
    void OnDependenciesDownloaded(AsyncOperationHandle result)
    {
        _events.NotifyDownloadFinished(result.Status == AsyncOperationStatus.Succeeded);
    }

    /// <summary> 예외 발생시 호출 </summary>
    void OnException(AsyncOperationHandle handle, Exception exception)
    {
        Debug.LogError(exception.Message);
    }
}
