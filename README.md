# Poke-RPG
# ゲーム概要
ポケモン風のターン制バトルなどが楽しめるゲームです。
Assts/Scenes/Gameplay.unityからゲームを開始できます。

## 作った経緯
RPGゲームが作りたいと思い、Unity C#の学習も兼ねて作りました。

# できること(マップ中)
* マップ移動
* NPCとの会話
* キャラクターが覚えている特定の技で木を切る、水上を移動する
* アイテムの使用
* 技マシンの使用
* キャラクターの回復
* 
## マップ画面のプレイ動画
https://youtu.be/vyp7juroOTM

# できること(バトル中)
* 技を選んで攻撃
* キャラクターの入れ替え
* アイテムの使用
* 進化
* 逃走(ランダムエンカウントのみ)
* キャラクターの捕獲(ランダムエンカウントのみ)

## 戦闘画面のプレイ動画
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

# ゲーム内容以外のポイント
### ゲームのステート管理
画面左上にゲーム中のステートを表示させています。ゲーム中の各ステートをスタック形式で管理し、ステートの開始、実行中、終了のそれぞれで処理を追加できるため、機能の実装がとてもやりやすくなっています。また、Updateの処理もステートごとに独立しているので、パフォーマンス負荷も抑えられます。

### 描画の必要ない部分の非表示
マップ上では描画の必要のない箇所は非表示にできるため、仮に広大なマップでも負荷を抑えられます。動作は下の動画のようになります。

https://github.com/sanpuru6320/BetheFan-main/assets/98676288/23824fc3-ac6f-42eb-a5e2-8fd35c99cd5d

### カットシーンをインスペクターで設定
インスペクター上でカットシーンを簡単に設定することができます。Dialogueボタンをクリックすれば下のリストに追加され、上から順に実行されます。順番を入れ替えればそのまま反映されます。
実装したカットシーンはマップ画面の動画1:10～1:20で見られます。
![スクリーンショット 2023-08-04 192229](https://github.com/sanpuru6320/BetheFan-main/assets/98676288/675e4dff-c74d-4c2b-af27-3fe78aed8358)

# 補足
* Unityのバージョンは2021.3.16f1ですが、おそらく最新のバージョンでも動くと思います。
* 海外のチュートリアルを参考に作ったので言語は英語です。

# 参照元
https://www.youtube.com/playlist?list=PLLf84Zj7U26kfPQ00JVI2nIoozuPkykDX
