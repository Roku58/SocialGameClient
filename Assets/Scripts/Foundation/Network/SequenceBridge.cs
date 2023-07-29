using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// シーン間で処理する何かを橋渡しするクラス
/// </summary>
public class SequenceBridge
{
    // このクラスはstatic的に機能する
    // SequenceBridgeクラスの唯一のインスタンスを生成
    static SequenceBridge _instance = new SequenceBridge();

    // 唯一のインスタンスにアクセスするためのプロパティ
    static public SequenceBridge Instance => _instance;

    // コンストラクタをprivateにすることで、インスタンス生成を制限
    private SequenceBridge() { }

    // シーケンスデータを格納する辞書
    Dictionary<string, SequencePackage> _sequenceDeck = new Dictionary<string, SequencePackage>();

    /// <summary>
    /// シーケンス登録
    /// </summary>
    // シーケンスを登録するメソッド
    static public bool RegisterSequence(string key, SequencePackage task)
    {
        // 既に同じキーが登録されている場合は重複しているとログを出力
        if (_instance._sequenceDeck.ContainsKey(key))
        {
            Debug.Log("重複して登録している");
            return false;
        }

        // シーケンスを辞書に追加
        _instance._sequenceDeck.Add(key, task);
        return true;
    }

    /// <summary>
    /// シーケンスを貰う
    /// </summary>
    // 指定したキーに対応するシーケンスを取得するメソッド
    static public T GetSequencePackage<T>(string key) where T : SequencePackage
    {
        // キーが登録されていない場合はログを出力してdefault(T)を返す
        if (!_instance._sequenceDeck.ContainsKey(key))
        {
            Debug.Log("登録されていない");
            return default;
        }

        // キャストしてシーケンスを返す
        return _instance._sequenceDeck[key] as T;
    }

    /// <summary>
    /// 準備完了状態のシーケンスを貰う非同期処理
    /// </summary>
    // 準備完了状態のシーケンスを取得する非同期メソッド
    static public async UniTask<T> GetSequencePackageWaitForReady<T>(string key) where T : SequencePackage
    {
        // キーが登録されていない場合はログを出力してdefault(T)を返す
        if (!_instance._sequenceDeck.ContainsKey(key))
        {
            Debug.Log("登録されていない");
            return default;
        }

        // シーケンスを取得
        T ret = _instance._sequenceDeck[key] as T;

        // シーケンスのIsReadyがtrueになるまで待機
        await UniTask.WaitUntil(() => ret.IsReady);

        // 準備完了後のシーケンスを返す
        return ret;
    }

    /// <summary>
    /// シーケンスの削除
    /// </summary>
    // シーケンスを削除するメソッド
    static public bool DeleteSequence(string key)
    {
        // キーが登録されていない場合はログを出力してfalseを返す
        if (!_instance._sequenceDeck.ContainsKey(key))
        {
            Debug.Log("登録されていない");
            return false;
        }

        // シーケンスのDisposeメソッドを呼び出して解放
        _instance._sequenceDeck[key].Dispose();

        // 辞書から削除
        _instance._sequenceDeck.Remove(key);

        // 成功を示すtrueを返す
        return true;
    }
}

/// <summary>
/// 派生前提のデータ格納パッケージ
/// </summary>
public class SequencePackage
{
    // キャンセル用のトークン
    CancellationToken _token;

    // シーケンスを格納するUniTask
    UniTask _task;

    // シーケンスの準備完了フラグ
    public bool IsReady { get; set; }

    // 派生クラスが必要に応じてオーバーライドするAwakeメソッド
    protected virtual void Awake() { }

    // シーケンスをキャンセルするメソッド
    public void Dispose()
    {
        _token.ThrowIfCancellationRequested();
    }

    // SequencePackageを生成する静的メソッド
    static public SequencePackage Create<T>(UniTask task) where T : SequencePackage, new()
    {
        // T型のSequencePackageを生成
        T package = new T();

        // キャンセル用のトークンを生成
        package._token = new CancellationToken();

        // 派生クラスで必要な初期化処理をAwakeメソッドで行う
        package.Awake();

        // UniTaskを格納
        package._task = task;

        // 外部からのキャンセルを適用
        task.AttachExternalCancellation(package._token);

        // UniTaskのForgetメソッドを呼び出して例外を握りつぶす
        task.Forget();

        // 準備完了フラグをfalseに設定
        package.IsReady = false;

        // 生成したパッケージを返す
        return package;
    }
}