using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using static Network.WebRequest;
using System.Net.Http;


namespace Network
{
    // TaskRequestWorkerクラスの定義
    public class TaskRequestWorker
    {
        // 処理中かどうかを示すプロパティ
        public bool IsActive { get; private set; }

        // 通信管理オブジェクトの定義
        class Packet
        {
            // 通信先のURLを格納する変数
            public string Uri;

            // POSTの際に通信内容に含めるデータを格納する変数
            public byte[] Body = null;

            // HTTPメソッドを格納する変数
            public RequestMethod Method;

            // 通信完了後にデータを送るデリゲートを格納する変数
            public GetData Delegate;

            // その他ヘッダ等の付加情報を格納する変数
            public Options Opt = null;
        }

        // HttpClientオブジェクト
        HttpClient _client = null;

        // 初期化
        public TaskRequestWorker()
        {
            // IsActiveをfalseに設定
            IsActive = false;

            // HttpClientオブジェクトを生成
            _client = new HttpClient();
        }

        /// <summary>
        /// リクエスト実行
        /// </summary>
        /// <param name="method">HTTPメソッド。GETとPOSTのみ対応</param>
        /// <param name="uri">通信先のURL</param>
        /// <param name="dlg">通信完了後にデータを送るデリゲート</param>
        /// <param name="body">POSTの際に通信内容に含めるデータ</param>
        /// <param name="opt">その他ヘッダ等の付加情報</param>
        // PostRequestメソッドの定義
        public async Task<string> PostRequest(string uri, string body, Options opt = null)
        {
            // 非同期のネットワークメソッドをtry/catchブロックで例外処理
            try
            {
                // IsActiveをtrueに設定
                IsActive = true;

                // HTTPリクエストを非同期で送信し、レスポンスを受け取る
                using HttpResponseMessage response = await _client.PostAsync(uri, new StringContent(body, Encoding.UTF8, "application/json"));

                // レスポンスが成功していることを確認
                response.EnsureSuccessStatusCode();

                // レスポンスのボディを非同期で読み取る
                string responseBody = await response.Content.ReadAsStringAsync();

                // IsActiveをfalseに設定してレスポンスを返す
                IsActive = false;
                return responseBody;
            }
            // ネットワークエラーが発生した場合の例外処理
            catch (HttpRequestException e)
            {
                // エラーログを出力
                Debug.Log("\nException Caught!");
                Debug.Log("Message :" + e.Message);

                // IsActiveをfalseに設定
                IsActive = false;
            }

            return null;
        }


        // GetRequestメソッドの定義
        public async Task<string> GetRequest(string uri, Options opt = null)
        {
            // 非同期のネットワークメソッドをtry/catchブロックで例外処理
            try
            {
                // IsActiveをtrueに設定
                IsActive = true;

                // HTTPリクエストを非同期で送信し、レスポンスを受け取る
                using HttpResponseMessage response = await _client.GetAsync(uri);

                // レスポンスが成功していることを確認
                response.EnsureSuccessStatusCode();

                // レスポンスのボディを非同期で読み取る
                string responseBody = await response.Content.ReadAsStringAsync();

                // IsActiveをfalseに設定してレスポンスを返す
                IsActive = false;
                return responseBody;
            }
            // ネットワークエラーが発生した場合の例外処理
            catch (HttpRequestException e)
            {
                // エラーログを出力
                Debug.Log("\nException Caught!");
                Debug.Log("Message :" + e.Message);

                // IsActiveをfalseに設定
                IsActive = false;
            }

            return null;
        }
    }
}