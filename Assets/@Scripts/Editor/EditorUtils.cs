using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

public static class EditorUtils
{
    public static void SetAddressableProfile(Define.EBuildType buildType)
    {
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        string profileID = settings.profileSettings.GetProfileId(buildType.ToString());
        settings.activeProfileId = profileID;
    }
}