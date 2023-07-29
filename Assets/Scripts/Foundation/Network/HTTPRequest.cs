using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

using static Network.WebRequest;
using System.Text;

/// <summary>
/// リクエストを処理するワーカースクリプト
/// NOTE: MonoBehaviourとして機能し、並列に実行できる
/// NOTE: UniTaskで書き直せたらやってみよう
/// </summary>
// HTTPRequestクラスの定義
/// <summary>
/// リクエストを処理するワーカースクリプト
/// NOTE: MonoBehaviourとして機能し、並列に実行できる
/// NOTE: UniTaskで書き直せたらやってみよう
/// </summary>
public class HTTPRequest : MonoBehaviour
{
    // リトライ回数を格納する変数
    int RetryCount = 0;

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

    // 初期化
    void Awake()
    {
        // IsActiveをfalseに設定
        IsActive = false;
    }

    /// <summary>
    /// リクエスト実行
    /// </summary>
    /// <param name="method">HTTPメソッド。GETとPOSTのみ対応</param>
    /// <param name="uri">通信先のURL</param>
    /// <param name="dlg">通信完了後にデータを送るデリゲート</param>
    /// <param name="body">POSTの際に通信内容に含めるデータ</param>
    /// <param name="opt">その他ヘッダ等の付加情報</param>
    // Requestメソッドの定義
    public void Request(RequestMethod method, string uri, GetData dlg, byte[] body = null, Options opt = null)
    {
        // IsActiveをtrueに設定
        IsActive = true;

        // 通信情報を格納するPacketクラスのインスタンスを生成
        Packet p = new Packet();

        // 通信先のURLを設定
        p.Uri = uri;

        // 通信完了後にデータを送るデリゲートを設定
        p.Delegate = dlg;

        // HTTPメソッドを設定
        p.Method = method;

        // POSTの際に通信内容に含めるデータを設定
        p.Body = body;

        // その他ヘッダ等の付加情報を設定
        p.Opt = opt;

        // Sendメソッドの実行を開始
        StartCoroutine(Send(p));
    }

    /// <summary>
    /// リクエスト処理コルーチン
    /// </summary>
    /// <param name="p">通信情報</param>
    /// <returns></returns>
    // Sendメソッドの定義
    IEnumerator Send(Packet p)
    {
        // UnityWebRequestを格納する変数の初期化
        UnityWebRequest req = null;

        // HTTPメソッドがGETの場合
        if (p.Method == RequestMethod.GET)
        {
            // UnityWebRequestをGETメソッドで初期化
            req = UnityWebRequest.Get(p.Uri);
        }

        // HTTPメソッドがPOSTの場合
        if (p.Method == RequestMethod.POST)
        {
            // UnityWebRequestをPOSTメソッドで初期化し、通信内容に含めるデータを設定
            req = new UnityWebRequest(p.Uri, UnityWebRequest.kHttpVerbPOST)
            {
                uploadHandler = new UploadHandlerRaw(p.Body),
                downloadHandler = new DownloadHandlerBuffer()
            };

            // Content-Typeヘッダを設定
            req.SetRequestHeader("Content-Type", "application/json");
        }

        // その他ヘッダ等の付加情報がある場合
        if (p.Opt != null)
        {
            // ヘッダ情報を設定
            p.Opt.Header.ForEach(h => req.SetRequestHeader(h.Name, h.Value));
        }

        // UnityWebRequestを送信し、レスポンスを待つ
        yield return req.SendWebRequest();

        // エラーが発生した場合
        if (req.error != null)
        {
            // リトライ回数を増やす
            RetryCount++;

            // リトライ回数が5回を超えた場合
            if (RetryCount > 5)
            {
                // IsActiveをfalseに設定して終了
                IsActive = false;
                yield break;
            }

            // エラーログを出力
            Debug.LogError(req.uri + ":" + req.error);

            // 1秒待ってリクエストを再試行
            yield return new WaitForSeconds(1);
            Request(p.Method, p.Uri, p.Delegate, p.Body, p.Opt);
        }
        // エラーがない場合
        else
        {
            // DataParseメソッドを呼び出し、データを解析
            DataParse(p, req);

            // IsActiveをfalseに設定
            IsActive = false;
        }
    }

    /// <summary>
    /// データ処理コールバック
    /// </summary>
    /// <param name="p"></param>
    /// <param name="req"></param>
    // DataParseメソッドの定義
    void DataParse(Packet p, UnityWebRequest req)
    {
        // 受信データを文字列に変換してデリゲートに渡す
        p.Delegate(Encoding.UTF8.GetString(req.downloadHandler.data));

        // IsActiveをfalseに設定
        IsActive = false;
    }
}