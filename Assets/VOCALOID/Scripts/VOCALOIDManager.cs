using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
using Yamaha.VOCALOID.Windows;
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX 
using Yamaha.VOCALOID.OSX;
#elif UNITY_IOS
using Yamaha.VOCALOID.iOS;
#endif

namespace Yamaha.VOCALOID.Samples {
    public class VOCALOIDManager: MonoBehaviour {
        public enum EnginState {
            Uninitialized, Initialized, Finalized, EnginStateLength
        };

        private EnginState eState = EnginState.Uninitialized;   // エンジンの状態.
        public EnginState EState {
            get {
                return this.eState;
            }
        }

        public VOCALOIDAudio VAudio = null;

        void Start() {
            Startup();
        }
        void OnApplicationQuit() {
            Shutdown();
        }

        void OnDestroy() {
        }

        /**
         * @brief	エンジンを起動
         */
        public bool Startup() {
            if(eState==EnginState.Uninitialized||eState==EnginState.Finalized) {
                YVF.YVFResult result = YVF.YVFStartup("personal", Application.streamingAssetsPath+"/VOCALOID/DB_ini");
                print("Startup: "+result);
                if(result!=YVF.YVFResult.Success) {
                    return false;
                }
                eState=EnginState.Initialized;

                // Realtimeモードに設定する.
                YVF.YVFRealtimeSetStaticSetting(YVF.YVFRealtimeMode.Mode3);

                VAudio.CreateVocAudio();
                VAudio.SetLyrics("a");

                return true;
            }
            return false;
        }

        /**
         * @brief	エンジンを停止
         */
        public void Shutdown() {
            if(EState==EnginState.Initialized) {
                VAudio.DeleteVocAudio();

                YVF.YVFShutdown();
                eState=EnginState.Finalized;

                print("Shutdown");
            }
        }

        /**
         * @brief	歌詞を設定
         */
        public bool SetLyrics(string lyrics) {
            if(EState==EnginState.Initialized && VAudio!=null) {
                return VAudio.SetLyrics(lyrics);
            }
            return false;
        }

        public bool NoteOn(int value) {
            if(EState==EnginState.Initialized&&VAudio!=null) {
                return VAudio.NoteOn(value);
            }
            return false;
        }
    }
}
