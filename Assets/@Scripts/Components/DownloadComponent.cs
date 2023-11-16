using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownloadComponent: MonoBehaviour
{
	public enum State
	{
		Idle,
		Initialize,
		UpdateCatalog,
		DownloadSize,
		DownloadDependencies,
		Downloading,
		Finished
	}

	private AddressableDownloader _downloader;
	
	string _downloadLabel = "Preload";
	string _downloadURL;

	private State _currentState { get; set; } = State.Idle;
	private State _lastState = State.Idle;
	private Action<DownloadEvents> _onEventObtained;

	private void Awake()
	{
		_downloadLabel = "Preload";

		_downloadURL = Application.platform == RuntimePlatform.IPhonePlayer  
			? "https://rookiss-rumble-addressables.s3.ap-northeast-2.amazonaws.com/iOS" 
			: "https://rookiss-rumble-addressables.s3.ap-northeast-2.amazonaws.com/Android";
	}

	public IEnumerator StartDownloadRoutine(Action<DownloadEvents> onEventObtained)
	{
		_downloader = new AddressableDownloader();
		_onEventObtained = onEventObtained;

		_lastState = _currentState = State.Initialize;

		while (_currentState != State.Finished)
		{
			OnExecute();
			yield return null;
		}
	}

	public void GoNext()
	{
		if (_lastState == State.Initialize)
		{
			_currentState = State.UpdateCatalog;
		}
		else if (_lastState == State.UpdateCatalog)
		{
			_currentState = State.DownloadSize;
		}
		else if (_lastState == State.DownloadSize)
		{
			_currentState = State.DownloadDependencies;
		}
		else if (_lastState == State.Downloading || _lastState == State.DownloadDependencies)
		{
			_currentState = State.Finished;
		}

		_lastState = _currentState;
	}

	void OnExecute()
	{
		if (_currentState == State.Idle)
		{
			return;
		}

		if (_currentState == State.Initialize)
		{
			var events = _downloader.InitializedSystem(this._downloadLabel, this._downloadURL);
			_onEventObtained?.Invoke(events);

			_currentState = State.Idle;
		}
		else if (_currentState == State.UpdateCatalog)
		{
			_downloader.UpdateCatalog();
			_currentState = State.Idle;
		}
		else if (_currentState == State.DownloadSize)
		{
			_downloader.DownloadSize();
			_currentState = State.Idle;
		}
		else if (_currentState == State.DownloadDependencies)
		{
			_downloader.StartDownload();
			_currentState = State.Downloading;
		}
		else if (_currentState == State.Downloading)
		{
			_downloader.Update();
		}
	}
}

