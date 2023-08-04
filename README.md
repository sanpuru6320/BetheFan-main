# Poke-RPG
# ゲーム概要
ポケモン風のターン制バトルなどが楽しめるゲームです。
Assts/Scenes/Gameplay.unityからゲームを開始できます。

# 作った経緯
RPGゲームが作りたいと思い、Unity C#の学習も兼ねて作りました。

# できること(マップ中)
* マップ移動
* NPCとの会話
* キャラクターが覚えている特定の技で木を切る、水上を移動する
* アイテムの使用
* 技マシンの使用
* キャラクターの回復
# できること(バトル中)
* 技を選んで攻撃
* キャラクターの入れ替え
* アイテムの使用
* 進化
* 逃走(ランダムエンカウントのみ)
* キャラクターの捕獲(ランダムエンカウントのみ)

## マップ上のプレイ動画
https://youtu.be/vyp7juroOTM
## 戦闘のプレイ動画
https://youtu.be/y2MGMcFj_u4

# 操作方法
* 移動
十字キー
* メニュー表示
スペースキー
* 選択項目変更
十字キー
* 決定
Zキー
* 戻る
Xキー

# 補足
* 画面左上にゲーム中のステートを表示させています。
* Unityのバージョンは2021.3.16f1ですが、おそらく最新のバージョンでも動くと思います。
* 海外のチュートリアルを参考に作ったので言語は英語です。

# ゲーム内容以外のポイント
ゲーム中の各ステートをスタック形式で管理し、ステートの開始、実行中、終了で分けているため、機能の実装がとてもやりやすくなっています。また、Updateの処理もステートごとに独立しているので、パフォーマンス負荷も抑えられます。
マップ上では描画の必要のない箇所は非表示にできるため、広大なマップでも負荷を抑えられます。動作は下の動画のようになります。

https://github.com/sanpuru6320/BetheFan-main/assets/98676288/23824fc3-ac6f-42eb-a5e2-8fd35c99cd5d

マップは一つのシーンでまとめて作ったり、別のシーンに移行するのではなく、それぞれ独立してシーンを作成し、それぞれをまとめて読み込む形を取っています。


# 参照元
https://www.youtube.com/playlist?list=PLLf84Zj7U26kfPQ00JVI2nIoozuPkykDX
