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
            
            uConsole.RegisterCommand("spawn_plane", new Action(SpawnPlane));
            uConsole.RegisterCommand("save_planemod", new Action(SavePlaneMod));
        }

        private void SpawnPlane()
        {
            RaycastHit hit;
            bool hasHit = Physics.Raycast(
                GameManager.m_MainCamera.transform.position,
                GameManager.m_MainCamera.transform.forward,
                hitInfo: out hit,
                1000,
                layerMask: Utils.m_WeaponProjectileCollisionLayerMask
            );
            Vector3 position = hasHit ? hit.point : GameManager.m_MainCamera.transform.position + GameManager.m_MainCamera.transform.forward * 1000;
            position -= GameManager.m_MainCamera.transform.forward;

            GameObject plane = PlaneModAssetManager.Singleton.SpawnPlane("CropDuster01", position);
            plane.transform.Translate(0, 3.56f, 0);
        }

        private void SavePlaneMod()
        {
            PlaneModDataManager.Singleton.SaveData();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (!SceneManager.GetActiveScene().isLoaded || !GameManager.m_MainCamera) return;

            AircraftManager.Singleton.Update(Time.deltaTime);
            PlaneModDataManager.Singleton.UpdateModelStreaming(Time.deltaTime);
        }
    }   
}
