using Cysharp.Threading.Tasks;
using MD;
using Outgame;
using System;
using UnityEngine;

namespace Outgame
{
    /// <summary>
    /// マスターデータを更新するクラス
    /// </summary>
    public class MasterDataUpdate : MonoBehaviour
    {
        // マスターデータを更新するためのメソッド
        public void MasterUpdate()
        {
            // MasterDataクラスのInstanceを取得し、Setupメソッドを実行する
            // Setupメソッドは非同期処理(UniTask)を返すため、Forgetメソッドで処理を非同期的に実行する
            MasterData.Instance.Setup(false).Forget();
        }
    }
}