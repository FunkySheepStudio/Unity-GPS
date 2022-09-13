using System.Collections;
using UnityEngine;
using UnityEngine.Android;

namespace FunkySheep.Gps
{
    [AddComponentMenu("FunkySheep/Gps/GPS Manager")]
    public class Manager : FunkySheep.Types.Singleton<Manager>
    {
        public FunkySheep.Types.Double latitude;
        public FunkySheep.Types.Double longitude;
        public FunkySheep.Events.Event<GameObject> onStartedEvent;
        public FunkySheep.Events.Event<GameObject> onNoGpsEvent;

        //GameObject dialog = null;
        IEnumerator Start()
        {
#if !UNITY_ANDROID && !UNITY_IOS && !UNITY_EDITOR || UNITY_SERVER
            NoGps();
            yield break;
#endif
            // Let some time for the editor to get the services location
#if UNITY_EDITOR
            float timeStart = Time.realtimeSinceStartup;
            yield return new WaitUntil(() => UnityEditor.EditorApplication.isRemoteConnected || (Time.realtimeSinceStartup >= timeStart + 15));
#endif

            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                Permission.RequestUserPermission(Permission.FineLocation);
            }

            // First, check if user has location service enabled
            if (!Input.location.isEnabledByUser)
            {
                print("Location not enabled");
                NoGps();
                yield break;
            }

            // Start service before querying location
            //dialog = new GameObject();
            Input.location.Start();

            // Wait until service initializes
            while (Input.location.status != LocationServiceStatus.Running)
            {
                yield return new WaitForSeconds(1);
            }
            GetData();
            if (onStartedEvent)
                onStartedEvent.Raise(gameObject);
        }

        void NoGps()
        {
            if (onNoGpsEvent)
                onNoGpsEvent.Raise(gameObject);
        }

        public void GetData()
        {
            latitude.value = Input.location.lastData.latitude;
            longitude.value = Input.location.lastData.longitude;
        }
    }
}
