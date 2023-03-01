using HkmpPouch;
using System;
using System.Globalization;
using UnityEngine;
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
        void handlePacketRecieve(object _,ReceivedEventArgs R){
            var p = R.Data;
            var playerIdString = p.FromPlayer.ToString();
            if(playerIdString != playerId) {return;}
            sr.enabled = true;
            if(p.EventName == EVENT.UPDATE && p.EventData.StartsWith($"{ghostId}|",StringComparison.Ordinal)){
                var ghostData = p.EventData.Split('|');
                //var ghostId = ghostData[0];
                targetPosition = new Vector3(
                    ParseFloat(ghostData[1], targetPosition.x),
                    ParseFloat(ghostData[2], targetPosition.y),
                    ParseFloat(ghostData[3], targetPosition.z)
                );
                SetScale(ParseFloat(ghostData[4], transform.localScale.x));
                cumulativeDeltaTime = 0;
                lastUpdate = DateTime.Now;
            }
            if(p.EventName == EVENT.DESTROY  && p.EventData == ghostId){
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