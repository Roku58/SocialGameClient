using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Network
{
    /// <summary>
    /// リクエスト処理クラス
    /// </summary>
    public class WebRequest
    {
        // データ処理デリゲート
        public delegate void GetData(string result);

        // リクエストヘッダーの定義
        public class Header
        {
            // ヘッダーの名前を格納する変数
            public string Name;

            // ヘッダーの値を格納する変数
            public string Value;
        }

        // リクエスト処理につけるオプションの定義
        public class Options
        {
            // リクエストヘッダーのリストを格納する変数
            public List<Header> Header = new List<Header>(); //リクエストヘッダー
        }

        // HTTPメソッドの定義
        public enum RequestMethod
        {
            GET,
            POST
        }

        // このクラスはstatic的に機能する
        // WebRequestクラスの唯一のインスタンスを生成
        static WebRequest Instance = new WebRequest();

        // 複数のTaskRequestWorkerを管理するリスト
        List<TaskRequestWorker> _worker = new List<TaskRequestWorker>();

        // CancellationTokenの定義
        CancellationToken ct;

        /// <summary>
        /// ワーカー設定
        /// </summary>
        // ワーカーのインスタンスを生成するメソッド
        static void CheckWorkerInstance()
        {
            // ワーカーリストが空の場合は5つのワーカーを生成して追加
            if (Instance._worker.Count == 0)
            {
                for (int i = 0; i < 5; ++i)
                {
                    Instance._worker.Add(new TaskRequestWorker());
                }
            }

            // アクティブなワーカーが1つもない場合は新しいワーカーを生成して追加
            var active = Instance._worker.Where(r => r.IsActive == false);
            if (active.Count() == 0)
            {
                Instance._worker.Add(new TaskRequestWorker());
            }
        }

        /// <summary>
        /// ワーカー設定
        /// </summary>
        // アクティブでないワーカーを取得するメソッド
        static TaskRequestWorker GetWorker()
        {
            TaskRequestWorker ret = null;
            // アクティブでないワーカーを取得
            var active = Instance._worker.Where(r => r.IsActive == false);
            // アクティブでないワーカーがない場合は新しいワーカーを生成して追加
            if (active.Count() == 0)
            {
                ret = new TaskRequestWorker();
                Instance._worker.Add(ret);
            }
            else
            {
                // アクティブでないワーカーを取得
                ret = active.First();
            }
            return ret;
        }

        /// <summary>
        /// GET通信をする(asyncラッパー)
        /// </summary>
        /// <param name="uri">通信先のURL</param>
        /// <param name="dlg">データ受信コールバック</param>
        /// <param name="opt">ヘッダなど追加で含む情報</param>
        // HTTP GET通信を非同期で行うメソッド
        static public async UniTask<string> GetRequest(string uri, Options opt = null)
        {
            // ワーカーの生成と設定
            CheckWorkerInstance();
            var worker = GetWorker();
            // ワーカーを使用してGET通信を行う
            return await worker.GetRequest(uri, opt);
        }

        /// <summary>
        /// POST通信をする(asyncラッパー)
        /// </summary>
        /// <param name="uri">通信先のURL</param>
        /// <param name="body">サーバに送信する内容</param>
        /// <param name="dlg">データ受信コールバック</param>
        /// <param name="opt">ヘッダなど追加で含む情報</param>
        // HTTP POST通信を非同期で行うメソッド
        static public async UniTask<string> PostRequest<T>(string uri, T body, Options opt = null)
        {
            // ワーカーの生成と設定
            CheckWorkerInstance();
            var worker = GetWorker();
            string json = JsonUtility.ToJson(body);
            // ワーカーを使用してPOST通信を行う
            return await worker.PostRequest(uri, json, opt);
        }

        /// <summary>
        /// POST通信をする(asyncラッパー)
        /// </summary>
        /// <param name="uri">通信先のURL</param>
        /// <param name="body">サーバに送信する内容</param>
        /// <param name="opt">ヘッダなど追加で含む情報</param>
        // HTTP POST通信を非同期で行うメソッド
        static public async UniTask<string> PostRequest<T>(string uri, string body, Options opt = null)
        {
            // ワーカーの生成と設定
            CheckWorkerInstance();
            var worker = GetWorker();
            // ワーカーを使用してPOST通信を行う
            return await worker.PostRequest(uri, body, opt);
        }

        /// <summary>
        /// GET通信をする(コールバック運用)
        /// </summary>
        /// <param name="uri">通信先のURL</param>
        /// <param name="dlg">データ受信コールバック</param>
        /// <param name="opt">ヘッダなど追加で含む情報</param>
        // HTTP GET通信をコールバック運用で行うメソッド
        static public void GetRequest(string uri, GetData dlg, Options opt = null)
        {
            // ワーカーの生成と設定
            CheckWorkerInstance();
            UniTask.RunOnThreadPool(async () =>
            {
                var worker = GetWorker();
                Debug.Log(uri);
                // ワーカーを使用してGET通信を行い、結果をデリゲートでコールバックする
                string result = await worker.GetRequest(uri, opt);
                UniTask.Post(() =>
                {
                    dlg?.Invoke(result);
                });
            }).Forget();
        }

        /// <summary>
        /// POST通信をする(コールバック運用)
        /// </summary>
        /// <param name="uri">通信先のURL</param>
        /// <param name="body">サーバに送信する内容</param>
        /// <param name="dlg">データ受信コールバック</param>
        /// <param name="opt">ヘッダなど追加で含む情報</param>
        // HTTP POST通信をコールバック運用で行うメソッド
        static public void PostRequest<T>(string uri, T body, GetData dlg, Options opt = null)
        {
            // ワーカーの生成と設定
            CheckWorkerInstance();
            UniTask.RunOnThreadPool(async () =>
            {
                var worker = GetWorker();
                string json = JsonUtility.ToJson(body);
                // ワーカーを使用してPOST通信を行い、結果をデリゲートでコールバックする
                string result = await worker.PostRequest(uri, json, opt);
                UniTask.Post(() =>
                {
                    dlg?.Invoke(result);
                });
            }).Forget();
        }

        /// <summary>
        /// POST通信をする(コールバック運用)
        /// </summary>
        /// <param name="uri">通信先のURL</param>
        /// <param name="body">サーバに送信する内容</param>
        /// <param name="dlg">データ受信コールバック</param>
        /// <param name="opt">ヘッダなど追加で含む情報</param>
        // HTTP POST通信をコールバック運用で行うメソッド
        static public void PostRequest<T>(string uri, string body, GetData dlg, Options opt = null)
        {
            // ワーカーの生成と設定
            CheckWorkerInstance();
            UniTask.RunOnThreadPool(async () =>
            {
                var worker = GetWorker();
                // ワーカーを使用してPOST通信を行い、結果をデリゲートでコールバックする
                string result = await worker.PostRequest(uri, body, opt);
                UniTask.Post(() =>
                {
                    dlg?.Invoke(result);
                });
            }).Forget();
        }
    }
}