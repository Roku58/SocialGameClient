using Outgame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Outgame
{
    // ユーザーデータをキャッシュするクラス
    public class UserDataCache
    {
        // ユーザーデータを保存するメソッド
        static public void Save<T>(string key, T data)
        {
            // LocalDataクラスを使用してデータを指定の場所に保存する
            LocalData.Save<T>(key, data, Application.dataPath, true);
        }

        // ユーザーデータを読み込むメソッド
        static public T Load<T>(string key)
        {
            // LocalDataクラスを使用してデータを指定の場所から読み込む
            return LocalData.Load<T>(key, Application.dataPath, true);
        }
    }
}