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
        Melon<PlaneMod>.Logger.Msg($"[UnityBundleManager] Initialized");
    }

    public void LoadAssetBundle(string assetBundleName, string assetBundlePath)
    {
        Melon<PlaneMod>.Logger.Msg($"[UnityBundleManager] LoadAssetBundle assetBundleName=\"{assetBundleName}\", assetBundlePath=\"{assetBundlePath}\"");
        
        AssetBundle assetBundle = AssetBundle.LoadFromFile(assetBundlePath);
        assetBundles.Add(assetBundleName, assetBundle);
    }

    public GameObject GetPrefabFromAssetBundle(string assetBundleName, string prefabName)
    {
        Melon<PlaneMod>.Logger.Msg($"[UnityBundleManager] GetPrefabFromAssetBundle assetBundleName=\"{assetBundleName}\", prefabName=\"{prefabName}\"");
        AssetBundle assetBundle = GetAssetBundle(assetBundleName);
        
        GameObject prefab = assetBundle.LoadAsset<GameObject>(prefabName);
        if (prefab == null)
        {
            Melon<PlaneMod>.Logger.Msg($"[UnityBundleManager] GetPrefabFromAssetBundle \"{prefabName}\" was not found!");
            return null;
        }

        return prefab;
    }

    public AssetBundle GetAssetBundle(string assetBundleName)
    {
        Melon<PlaneMod>.Logger.Msg($"[UnityBundleManager] GetAssetBundle assetBundleName=\"{assetBundleName}\"");

        try
        {
            AssetBundle assetBundle = assetBundles[assetBundleName];
            return assetBundle;
        }
        catch (KeyNotFoundException)
        {
            Melon<PlaneMod>.Logger.Warning($"[UnityBundleManager] GetAssetBundle \"{assetBundleName}\" was not found!");
        }

        return null;
    }
}