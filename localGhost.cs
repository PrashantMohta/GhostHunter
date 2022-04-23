
using UnityEngine;
namespace GhostHunter
{
    public class localGhost : MonoBehaviour{
        public string ghostId;
        void Update(){
            var pos = transform.position;
            GhostHunter.Instance.HkmpPipe.SendToAll(
                0,EVENT.UPDATE,
                $"{ghostId},{pos.x.ToString("0.0")},{pos.y.ToString("0.0")},{pos.z.ToString("0.0")},{transform.localScale.x.ToString("0.0")}",
                false,true);
        }
        void OnDestroy(){
            GhostHunter.Instance.HkmpPipe.SendToAll(0,EVENT.DESTROY,ghostId,false,true);
        }
    }
}