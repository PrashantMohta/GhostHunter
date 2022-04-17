using UnityEngine;
using Satchel;
using Satchel.HkmpPipe;
using static GhostHunter.Utils;
namespace GhostHunter
{
    public class remoteGhost : MonoBehaviour{
        public string playerId,ghostId;
        public Vector3 targetPosition;
        float cumulativeDeltaTime = 0f;
        void Start()
        {
            GhostHunter.Instance.HkmpPipe.OnRecieve += handlePacketRecieve;
        }
        void OnDestroy(){
            GhostHunter.Instance.HkmpPipe.OnRecieve -= handlePacketRecieve;
        }
        void handlePacketRecieve(object _,RecievedEventArgs R){
            var p = R.packet;
            if(p.fromPlayer.ToString() == playerId && p.eventName == $"update" && p.eventData.StartsWith($"{ghostId},")){
                var ghostData = p.eventData.Split(',');
                var ghostId = ghostData[0];
                targetPosition = new Vector3(
                    float.Parse(ghostData[1]),
                    float.Parse(ghostData[2]),
                    float.Parse(ghostData[3])
                );
                SetScale(float.Parse(ghostData[4]));
                cumulativeDeltaTime = 0;
            }
            if(p.fromPlayer.ToString() == playerId && p.eventName == $"destroy-{ghostId}"){
                GameObject.Destroy(gameObject);
            }
        }

        void SetScale(float scale){
            var ls = transform.localScale;
            ls.x = scale;
            transform.localScale = ls;
        }
        
        void Update(){
            if(targetPosition != null){
                cumulativeDeltaTime += Time.deltaTime;
                transform.position = Vector2.Lerp(transform.position, targetPosition, cumulativeDeltaTime / (1f/60f));
            }
        }
    }
    
}