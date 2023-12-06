using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Downloader: MonoBehaviour
{
	public enum State
	{
		Idle,
		Initialization,
		UpdateCatalog,
		DetermineDownloadSize,
		DownloadDependencies,
		Downloading,
		Finished
	}
	
	public string DownloadLabel;

	public State CurrentState { get; set; } = State.Idle;
	private State _previousState = State.Idle;
	private Action<DownloadEvents> _onEventObtained;
	private DownloadEvents _downloadEvents;
	private AsyncOperationHandle _downloadHandle;
	private long _totalDownloadSize;


	
	public IEnumerator StartDownload(Action<DownloadEvents> onEventObtained)
	{
		_onEventObtained = onEventObtained;
		_previousState = CurrentState = State.Initialization;

		while (CurrentState != State.Finished)
		{
			OnExecute();
			yield return null;
		}
	}

	public void GoNext()
	{
		if (_previousState == State.Initialization)
		{
			CurrentState = State.UpdateCatalog;
		}
		else if (_previousState == State.UpdateCatalog)
		{
			CurrentState = State.DetermineDownloadSize;
		}
		else if (_previousState == State.DetermineDownloadSize)
		{
			CurrentState = State.DownloadDependencies;
		}
		else if (_previousState == State.Downloading || _previousState == State.DownloadDependencies)
		{
			CurrentState = State.Finished;
		}

		_previousState = CurrentState;
	}

	private void OnExecute()
	{
		switch (CurrentState)
		{
			case State.Idle:
				return;
			case State.Initialization:
				SetupDownloader();
				CurrentState = State.Idle;
				break;
			case State.UpdateCatalog:
				UpdateCatalog();
				CurrentState = State.Idle;
				break;
			case State.DetermineDownloadSize:
				DownloadSize();
				CurrentState = State.Idle;
				break;
			case State.DownloadDependencies:
				StartDownload();
				CurrentState = State.Downloading;
				break;
			case State.Downloading:
				Update();
				break;
		}
	}

	private void SetupDownloader()
	{
		_downloadEvents = new DownloadEvents();
		Addressables.InitializeAsync().Completed += OnInitialized;
		UnityEngine.ResourceManagement.ResourceManager.ExceptionHandler += OnException;
		_onEventObtained?.Invoke(_downloadEvents);
	}

	#region MyRegion
	
	private void UpdateCatalog()
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
				_downloadEvents.NotifyCatalogUpdated();
			}
		};
	}

	private void DownloadSize()
	{
		Addressables.GetDownloadSizeAsync(DownloadLabel).Completed += OnSizeDownloaded;
	}

	private void StartDownload()
	{
		_downloadHandle = Addressables.DownloadDependenciesAsync(DownloadLabel);
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
			long remainedSize = _totalDownloadSize - curDownloadedSize;

			_downloadEvents.NotifyDownloadProgress(new DownloadProgressStatus(
				status.DownloadedBytes
				, _totalDownloadSize
				, remainedSize
				, status.Percent));
		}
	}

	

	#endregion

    #region DownloadEvent

    private void OnInitialized(AsyncOperationHandle<IResourceLocator> result)
    {
        _downloadEvents.NotifyInitialized();
    }

    void OnCatalogUpdated(AsyncOperationHandle<List<IResourceLocator>> result)
    {
        _downloadEvents.NotifyCatalogUpdated();
    }

    void OnSizeDownloaded(AsyncOperationHandle<long> result)
    {
        _totalDownloadSize = result.Result;
        _downloadEvents.NotifySizeDownloaded(result.Result);
    }

    void OnDependenciesDownloaded(AsyncOperationHandle result)
    {
        _downloadEvents.NotifyDownloadFinished(result.Status == AsyncOperationStatus.Succeeded);
    }

    void OnException(AsyncOperationHandle handle, Exception exception)
    {
        Debug.LogError(exception.Message);
    }

    #endregion
}

