using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LibitCall.Applications.VocalWall {
    public class KeyWall: MonoBehaviour {
        public Material[] KeyWallMaterials = null;

        public enum Key {
            C, D, E, F, G, A, B, Length
        }

        private const int baseNoteNumber = 72;
        private int noteNumber = baseNoteNumber;
        public int NoteNumber {
            set {
                this.noteNumber=value;
            }
            get {
                return this.noteNumber;
            }
        }

        void Start() {
            UpdateKey();
        }

        public void UpdateKey() {
            int key = Random.Range(0, (int)KeyWall.Key.Length);
            GetComponent<Renderer>().material=KeyWallMaterials[key];

            switch((Key)key) {
                case Key.C:
                    noteNumber=baseNoteNumber;
                    break;
                case Key.D:
                    noteNumber=baseNoteNumber+2;
                    break;
                case Key.E:
                    noteNumber=baseNoteNumber+4;
                    break;
                case Key.F:
                    noteNumber=baseNoteNumber+5;
                    break;
                case Key.G:
                    noteNumber=baseNoteNumber+7;
                    break;
                case Key.A:
                    noteNumber=baseNoteNumber+9;
                    break;
                case Key.B:
                    noteNumber=baseNoteNumber+11;
                    break;
            }
        }
    }
}
