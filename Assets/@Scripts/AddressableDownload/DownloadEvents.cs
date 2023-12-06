using System;

/// 다운로드 진행 상황 정보 
public struct DownloadProgressStatus
{
    public long downloadedBytes;// 다운로드된 바이트 사이즈 
    public long totalBytes;     // 다운로드 받을 전체 사이즈 
    public long remainedBytes;  // 남은 바이트 사이즈 
    public float totalProgress; // 전체 진행률 0 ~ 1 

    public DownloadProgressStatus(long downloadedBytes, long totalBytes, long remainedBytes, float totalProgress)
    {
        this.downloadedBytes = downloadedBytes;
        this.totalBytes = totalBytes;
        this.remainedBytes = remainedBytes;
        this.totalProgress = totalProgress;
    }
}

public class DownloadEvents
{
    public event Action Initialized;
    public event Action CatalogUpdated;
    public event Action<long> SizeDownloaded;
    public event Action<DownloadProgressStatus> ProgressUpdated;
    public event Action<bool> Finished;
    
    public void NotifyInitialized() => Initialized?.Invoke();
    public void NotifyCatalogUpdated() => CatalogUpdated?.Invoke();
    public void NotifySizeDownloaded(long size) => SizeDownloaded?.Invoke(size);
    public void NotifyDownloadProgress(DownloadProgressStatus status) => ProgressUpdated?.Invoke(status);
    public void NotifyDownloadFinished(bool isSuccess) => Finished?.Invoke(isSuccess);
}