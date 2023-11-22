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
	
	public string DownloadLabel;
	string _downloadURL;

	public State CurrentState { get; set; } = State.Idle;
	private State _lastState = State.Idle;
	private Action<DownloadEvents> _onEventObtained;

	private void Awake()
	{
		_downloadURL = Application.platform == RuntimePlatform.IPhonePlayer  
			? "https://rookiss-rumble-addressables.s3.ap-northeast-2.amazonaws.com/iOS" 
			: "https://rookiss-rumble-addressables.s3.ap-northeast-2.amazonaws.com/Android";
	}

	public IEnumerator StartDownloadRoutine(Action<DownloadEvents> onEventObtained)
	{
		_downloader = new AddressableDownloader();
		_onEventObtained = onEventObtained;

		_lastState = CurrentState = State.Initialize;

		while (CurrentState != State.Finished)
		{
			OnExecute();
			yield return null;
		}
	}

	public void GoNext()
	{
		if (_lastState == State.Initialize)
		{
			CurrentState = State.UpdateCatalog;
		}
		else if (_lastState == State.UpdateCatalog)
		{
			CurrentState = State.DownloadSize;
		}
		else if (_lastState == State.DownloadSize)
		{
			CurrentState = State.DownloadDependencies;
		}
		else if (_lastState == State.Downloading || _lastState == State.DownloadDependencies)
		{
			CurrentState = State.Finished;
		}

		_lastState = CurrentState;
	}

	void OnExecute()
	{
		if (CurrentState == State.Idle)
		{
			return;
		}

		if (CurrentState == State.Initialize)
		{
			var events = _downloader.InitializedSystem(this.DownloadLabel, this._downloadURL);
			_onEventObtained?.Invoke(events);

			CurrentState = State.Idle;
		}
		else if (CurrentState == State.UpdateCatalog)
		{
			_downloader.UpdateCatalog();
			CurrentState = State.Idle;
		}
		else if (CurrentState == State.DownloadSize)
		{
			_downloader.DownloadSize();
			CurrentState = State.Idle;
		}
		else if (CurrentState == State.DownloadDependencies)
		{
			_downloader.StartDownload();
			CurrentState = State.Downloading;
		}
		else if (CurrentState == State.Downloading)
		{
			_downloader.Update();
		}
	}
}

