using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace TLD_PlaneMod
{
    public static class PlaneModSettings
    {
        public static string BASE_DATAPATH = "Mods\\planemod_data.json";
        
        public static string ASSETBUNDLE_PATH = "Mods\\assetsforplanemod_assets_all.bundle";
        public static string PLANEMOD_BUNDLENAME = "PLANEMOD";
        
        public static string ASSET_DEFINITION_PATH = "Mods\\planemod_assetdefinitions.json";
    }
    public class PlaneMod : MelonMod
    {
        public bool firstFrame;
        public GameObject plane;
        
        public override void OnInitializeMelon()
        {
            Melon<PlaneMod>.Logger.Msg($"OnInitializeMelon");
            new UnityBundleManager();
            new PlaneModManager();
            
            UnityBundleManager.Singleton.LoadAssetBundle(
                PlaneModSettings.PLANEMOD_BUNDLENAME, 
                PlaneModSettings.ASSETBUNDLE_PATH
            );
            
            UnityBundleManager.Singleton.DumpAssetNames(PlaneModSettings.PLANEMOD_BUNDLENAME);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (!SceneManager.GetActiveScene().isLoaded || !GameManager.m_MainCamera) return;

            if (!firstFrame)
            {
                firstFrame = true;
                PlaneModDataManager.Singleton.LoadData();
                plane = TryCreateFromBundle();
            }

            if (plane)
            {
                plane.transform.Rotate(0, 25 * Time.deltaTime, 0);
            }

            AircraftManager.Singleton.Update(Time.deltaTime);

        }

        public GameObject TryCreateFromBundle()
        {
            Vector3 position = GameManager.m_MainCamera.transform.position +
                               GameManager.m_MainCamera.transform.forward * 25;

            position.y += 6.75f;

            GameObject lGameObject = PlaneModAssetManager.Singleton.SpawnPlane("CropDuster01", position);
            
            return lGameObject;
        }
    }   
}
