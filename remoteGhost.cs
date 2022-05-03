using UnityEngine;
using System;
using Satchel;
using HkmpPouch;
using static GhostHunter.Utils;
namespace GhostHunter
{
    public class remoteGhost : MonoBehaviour{
        public string playerId,ghostId;
        public Vector3 targetPosition;
        float cumulativeDeltaTime = 0f;
        private DateTime lastUpdate;
        private SpriteRenderer sr;

        void Start()
        {
            GhostHunter.Instance.HkmpPipe.OnRecieve += handlePacketRecieve;
            sr = GetComponent<SpriteRenderer>();
        }
        void OnDestroy(){
            GhostHunter.Instance.HkmpPipe.OnRecieve -= handlePacketRecieve;
        }
        void handlePacketRecieve(object _,RecievedEventArgs R){
            var p = R.packet;
            var playerIdString = p.fromPlayer.ToString();
            if(playerIdString != playerId) {return;}
            sr.enabled = true;
            if(p.eventName == EVENT.UPDATE && p.eventData.StartsWith($"{ghostId}|",StringComparison.Ordinal)){
                var ghostData = p.eventData.Split('|');
                var ghostId = ghostData[0];
                targetPosition = new Vector3(
                    float.Parse(ghostData[1]),
                    float.Parse(ghostData[2]),
                    float.Parse(ghostData[3])
                );
                SetScale(float.Parse(ghostData[4]));
                cumulativeDeltaTime = 0;
                lastUpdate = DateTime.Now;
            }
            if(p.eventName == EVENT.DESTROY  && p.eventData == ghostId){
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
            if(lastUpdate != null){
                if((DateTime.Now - lastUpdate).TotalMilliseconds > 10000){
                    sr.enabled = false;
                    //GameObject.Destroy(gameObject);
                }
            }
        }
    }
    
}