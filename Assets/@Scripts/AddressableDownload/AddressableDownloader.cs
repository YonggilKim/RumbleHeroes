using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableDownloader
{
    public static string DownloadURL;
    DownloadEvents _events;
    string _labelToDownload;
    long _totalSize;
    AsyncOperationHandle _downloadHandle;

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

    #region DownloadEvent

    private void OnInitialized(AsyncOperationHandle<IResourceLocator> result)
    {
        _events.NotifyInitialized();
    }

    void OnCatalogUpdated(AsyncOperationHandle<List<IResourceLocator>> result)
    {
        _events.NotifyCatalogUpdated();
    }

    void OnSizeDownloaded(AsyncOperationHandle<long> result)
    {
        _totalSize = result.Result;
        _events.NotifySizeDownloaded(result.Result);
    }

    void OnDependenciesDownloaded(AsyncOperationHandle result)
    {
        _events.NotifyDownloadFinished(result.Status == AsyncOperationStatus.Succeeded);
    }

    void OnException(AsyncOperationHandle handle, Exception exception)
    {
        Debug.LogError(exception.Message);
    }

    #endregion

}
