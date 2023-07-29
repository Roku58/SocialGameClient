using Cysharp.Threading.Tasks;
using Outgame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Outgame
{
    // ログイン時のデータ格納パッケージクラス
    public class LoginPackage : SequencePackage
    {
        // ログインAPIからのレスポンスを格納するプロパティ
        public APIResponceLogin Login { get; set; }

        // カード取得APIからのレスポンスを格納するプロパティ
        public APIResponceGetCards Cards { get; set; }
    }

    // ガチャ実行時のデータ格納パッケージクラス
    public class GachaDrawPackage : SequencePackage
    {
        // ガチャ実行APIからのレスポンスを格納するプロパティ
        public APIResponceGachaDraw Gacha { get; set; }
    }

    // クエスト実行時のデータ格納パッケージクラス
    public class QuestPackage : SequencePackage
    {
        // クエスト結果APIからのレスポンスを格納するプロパティ
        public APIResponceQuestResult QuestResult { get; set; }
    }
}