using UnityEngine;
using System.Collections;

using Yamaha.VOCALOID.Samples;

namespace LibitCall.Applications.VocalWall {
    public class GameController: MonoBehaviour {
        private const float minSpeed = 0.1f;
        private const float maxSpeed = 0.4f;
        private Vector3 velocity = new Vector3(0, minSpeed);
        private const float extForce = 0.1f;
        private const float attenuation = 0.001f;

        public VOCALOIDManager VManager = null;

        void Update() {
            if(Input.GetMouseButtonDown(0)) {
                accelerate();
            }
            Vector3 pos = transform.position;
            pos+=velocity;
            transform.position=pos;
            decelerate();
        }

        void OnCollisionEnter(Collision collision) {
            velocity.y*=-1;
            KeyWall keyWall = collision.gameObject.GetComponent<KeyWall>();
            if(keyWall!=null) {
                VManager.NoteOn(keyWall.NoteNumber);
                keyWall.UpdateKey();
            }
        }

        private void accelerate() {
            if(velocity.y>0) {
                velocity.y=Mathf.Min(velocity.y+extForce, maxSpeed);
            } else {
                velocity.y=Mathf.Max(velocity.y-extForce, -maxSpeed);
            }
        }
        private void decelerate() {
            if(velocity.y>0) {
                velocity.y=Mathf.Max(velocity.y-attenuation, minSpeed);
            } else {
                velocity.y=Mathf.Min(velocity.y+attenuation, -minSpeed);
            }
        }
    }
}