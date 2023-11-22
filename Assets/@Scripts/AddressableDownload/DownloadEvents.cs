using System;

/// <summary> 다운로드 진행 상황 정보 </summary>
public struct DownloadProgressStatus
{
    public long downloadedBytes;/* 다운로드된 바이트 사이즈 */
    public long totalBytes;     /* 다운로드 받을 전체 사이즈 */
    public long remainedBytes;  /* 남은 바이트 사이즈 */
    public float totalProgress; /* 전체 진행률 0 ~ 1 */

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
    // 시스템 초기화
    public event Action SystemInitializedListener;
    public void NotifyInitialized() => SystemInitializedListener?.Invoke();

    // Catalog 업데이트 완료 
    public event Action CatalogUpdatedListener;
    public void NotifyCatalogUpdated() => CatalogUpdatedListener?.Invoke();

    // Size 다운로드 완료 
    public event Action<long> SizeDownloadedListener;
    public void NotifySizeDownloaded(long size) => SizeDownloadedListener?.Invoke(size);

    // 다운로드 진행
    public event Action<DownloadProgressStatus> DownloadProgressListener;
    public void NotifyDownloadProgress(DownloadProgressStatus status) => DownloadProgressListener?.Invoke(status);

    // Bundle 다운로드 완료
    public event Action<bool> DownloadFinished;
    public void NotifyDownloadFinished(bool isSuccess) => DownloadFinished?.Invoke(isSuccess);
}