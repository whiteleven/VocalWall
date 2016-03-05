using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
using Yamaha.VOCALOID.Windows;
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX 
using Yamaha.VOCALOID.OSX;
#elif UNITY_IOS
using Yamaha.VOCALOID.iOS;
#endif

namespace Yamaha.VOCALOID.Samples {
    public class VOCALOIDAudio: MonoBehaviour {
        private AudioSource source = null;      //!< 合成波形を出音するためのAudioSource.
        private Int16[] renderData = new Int16[2048];
        private bool ready = false;

        /**
         * @brief	メロディと再生タイミングを取得して,合成を行うスレッドを走らせる.
         */
        public void CreateVocAudio() {
            // リアルタイム処理を起動.
            if(YVF.YVFRealtimeStart()!=YVF.YVFResult.Success) {
                return;
            }
            // AudioClip の生成.
            AudioClip clip = AudioClip.Create("VOCALOID"+gameObject.GetInstanceID(), YVF.YVFSamplingRate, 1, YVF.YVFSamplingRate, true);

            source=gameObject.GetComponent<AudioSource>();
            source.clip=clip;
            source.Play();
        }

        /**
         * @brief	歌詞を設定
         */
        public bool SetLyrics(string lyrics) {
            ready=false;
            if(YVF.YVFRealtimeSetLyrics(lyrics, YVF.YVFLang.Japanese)!=YVF.YVFResult.Success) {
                return false;
            }
            ready=true;
            return true;
        }

        public bool NoteOn(int value) {
            if(YVF.YVFRealtimeAddMidi(YVF.YVFMIDIEventType.NoteOn, value)!=YVF.YVFResult.Success) {
                return false;
            }
            return YVF.YVFRealtimeCommitMidi()==YVF.YVFResult.Success;
        }

        /**
         * @brief	リアルタイム合成の停止
         */
        public void DeleteVocAudio() {
            YVF.YVFRealtimeStop();
        }

        // OnAudioFilterRead() で VOCALOID 合成音を直接設定する場合は，Audio の Project Settings にて System Sample Rate に 44100 [Hz] を設定する必要あります
        void OnAudioFilterRead(Single[] data, Int32 channels) {
            if(!ready) {
                return;
            }
            UInt32 numBufferdSamples = YVF.YVFRealtimeGetAudioNumData();

            Int32 numOutSamples = data.Length/channels;
            if(numBufferdSamples<numOutSamples) {
                numOutSamples=(Int32)numBufferdSamples;
            }

            // リアルタイム合成の結果をYVFから受け取る.
            if(renderData.Length<numOutSamples) {
                renderData=new Int16[numOutSamples];
            }
            YVF.YVFRealtimePopAudio(renderData, numOutSamples);

            // 合成結果をAudioClipに書き込む.
            for(int i = 0; i<numOutSamples; ++i) {
                Single value = renderData[i]/32768.0f;  // convert uint16_t (-32768 ~ 32767) to float (-1.0 ~ 1.0)
                int index = i*channels;
                for(int j = index; j<index+channels; ++j) {
                    data[j]=value;
                }
            }

            // 不足した場合はゼロで埋める.
            for(int i = numOutSamples*channels; i<data.Length; ++i) {
                data[i]=0;
            }
        }

        /**
         * @brief	再生を停止し,ゲームオブジェクトを破棄する.
         */
        public void Delete() {
            if(source!=null) {
                source.Stop();
            }
            Destroy(gameObject);
        }
    }
}
