using System.Collections;
using UnityEngine;
using UnityEngine.Android;

namespace FunkySheep.Gps
{
    [AddComponentMenu("FunkySheep/Gps/GPS Manager")]
    public class Manager : MonoBehaviour
    {
        public FunkySheep.Types.Double latitude;
        public FunkySheep.Types.Double longitude;
        public FunkySheep.Events.Event<GameObject> onStartedEvent;

        //GameObject dialog = null;
        IEnumerator Start()
        {
            // Let some time for the editor to get the services location
#if UNITY_EDITOR
            yield return new WaitWhile(() => !UnityEditor.EditorApplication.isRemoteConnected);
#endif

            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                Permission.RequestUserPermission(Permission.FineLocation);
            }

            // First, check if user has location service enabled
            if (!Input.location.isEnabledByUser)
            {
                print("Location not enabled");
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

            reset();
        }

        public void reset()
        {
            GetData();
            onStartedEvent.Raise(gameObject);
        }

        public void GetData()
        {
            latitude.value = Input.location.lastData.latitude;
            longitude.value = Input.location.lastData.longitude;
        }
    }
}
