namespace TLD_PlaneMod;

public class UnityBundleManager
{
    public static UnityBundleManager Singleton;
    public Dictionary<string, AssetBundle> assetBundles;
    
    public UnityBundleManager()
    {
        if (Singleton != null) return;
        Singleton = this;
        
        assetBundles = new Dictionary<string, AssetBundle>();
        PlaneModLogger.Msg($"[UnityBundleManager] Initialized");
    }

    public void LoadAssetBundle(string assetBundleName, string assetBundlePath)
    {
        PlaneModLogger.MsgVerbose($"[UnityBundleManager] LoadAssetBundle assetBundleName=\"{assetBundleName}\", assetBundlePath=\"{assetBundlePath}\"");
        
        AssetBundle assetBundle = AssetBundle.LoadFromFile(assetBundlePath);
        assetBundles.Add(assetBundleName, assetBundle);
    }

    public GameObject GetPrefabFromAssetBundle(string assetBundleName, string prefabName)
    {
        PlaneModLogger.MsgVerbose($"[UnityBundleManager] GetPrefabFromAssetBundle assetBundleName=\"{assetBundleName}\", prefabName=\"{prefabName}\"");
        AssetBundle assetBundle = GetAssetBundle(assetBundleName);
        
        GameObject prefab = assetBundle.LoadAsset<GameObject>(prefabName);
        if (prefab == null)
        {
            PlaneModLogger.MsgVerbose($"[UnityBundleManager] GetPrefabFromAssetBundle \"{prefabName}\" was not found!");
            return null;
        }

        return prefab;
    }

    public AssetBundle GetAssetBundle(string assetBundleName)
    {
        PlaneModLogger.MsgVerbose($"[UnityBundleManager] GetAssetBundle assetBundleName=\"{assetBundleName}\"");

        try
        {
            AssetBundle assetBundle = assetBundles[assetBundleName];
            return assetBundle;
        }
        catch (KeyNotFoundException)
        {
            PlaneModLogger.Warn($"[UnityBundleManager] GetAssetBundle \"{assetBundleName}\" was not found!");
        }

        return null;
    }

    public void DumpAssetNames(string assetBundleName)
    {
        PlaneModLogger.MsgVerbose($"[UnityBundleManager] DumpAssetNames assetBundleName=\"{assetBundleName}\"");
        AssetBundle assetBundle = GetAssetBundle(assetBundleName);

        foreach (var name in assetBundle.AllAssetNames())
        {
            PlaneModLogger.MsgVerbose($"[UnityBundleManager] Asset = \"{name}\"");
        }
        foreach (var name in assetBundle.GetAllScenePaths())
        {
            PlaneModLogger.MsgVerbose($"[UnityBundleManager] ScenePath = \"{name}\"");
        }
    }
}