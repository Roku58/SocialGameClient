using Cysharp.Threading.Tasks; // Cysharp.Threading.Tasks ネームスペースを使用して非同期処理をサポート
using MD; // MD ネームスペースを使用
using System; // System ネームスペースを使用
using System.Collections.Generic; // ジェネリックコレクションを使用
using System.Linq; // LINQ (Language Integrated Query) を使用
using System.Text; // 文字列操作をサポート
using System.Threading.Tasks; // タスクベースの非同期処理をサポート
using UnityEngine; // Unity エンジンに関連するクラスを使用
using static Network.WebRequest; // Network.WebRequest クラスを静的にインポート

public class StatusCheck
{
    // シングルトン運用
    static StatusCheck _instance = new StatusCheck(); // StatusCheck クラスのシングルトンインスタンスを生成
    static public StatusCheck Instance => _instance; // シングルトンインスタンスを外部に公開するプロパティ

    [Serializable]
    public class GameStatus
    {
        public int status; // ゲームのステータスを示す整数値
        public bool isMaintenance; // メンテナンス中かどうかを示すブール値
        public string contentCatalog; // コンテンツカタログに関する情報を示す文字列
        public MasterVersion[] masterVersion; // マスターデータのバージョン情報を配列で格納
    }

    GameStatus _status = null; // ゲームのステータス情報を保持する変数

    static public async UniTask<GameStatus> Check()
    {
        // チェックAPI読み込み
        Debug.Log("StatusCheck Start."); // デバッグログに "StatusCheck Start." を出力

        string json = await GetRequest(GameSetting.StatusCheckAPIURI); // GameSetting.StatusCheckAPIURI で指定されたAPIから情報を非同期に取得
        _instance._status = JsonUtility.FromJson<GameStatus>(json); // 取得した JSON データを GameStatus クラスにデシリアライズ
        Debug.Log(_instance._status.status); // デバッグログにステータスの整数値を出力
        return _instance._status; // 取得したゲームステータス情報を返す
    }
}

//シングルトンパターンを使って StatusCheck クラスの唯一のインスタンスを取得し、非同期でゲームステータス情報を取得するためのクラス
//GameStatus クラスはゲームの状態を示す情報を保持し、Check メソッドは指定されたAPIからJSONデータを取得し、それを GameStatus オブジェクトにデシリアライズして返す
//Debug.Log メソッドを使用してデバッグ情報を出力する