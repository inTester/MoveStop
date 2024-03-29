﻿// このファイルで必要なライブラリのnamespaceを指定
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

/// <summary>
/// プロジェクト名がnamespaceとなります
/// </summary>
namespace OneButton
{
    /// <summary>
    /// ゲームの基盤となるメインのクラス
    /// 親クラスはXNA.FrameworkのGameクラス
    /// </summary>
    public class Game1 : Game
    {
        // フィールド（このクラスの情報を記述）
        private GraphicsDeviceManager graphicsDeviceManager;//グラフィックスデバイスを管理するオブジェクト
        private SpriteBatch spriteBatch;//画像をスクリーン上に描画するためのオブジェクト

        Size size = new Size();
        Player player;
        Key key;
        Map map;
        Collition coll;
        PositionBar positionBar;
        Enemy enemy;

        UI_Button button;
        Anime anime;
        UI ui;
        Scene_Count sceneCount;
        Bar bar;

        enum Scene { title, tutlial, play, end, retry }
        enum Bgm { TITLE, STAGE }
        enum SE { DIE, CLEAR, MOVE, ITEM, HIGH, BOTAN, HIT }
        Time time;
        Ranking ranking;
        Music music;

        Scene scene;

        /// <summary>
        /// コンストラクタ
        /// （new で実体生成された際、一番最初に一回呼び出される）
        /// </summary>
        public Game1()
        {
            //グラフィックスデバイス管理者の実体生成
            graphicsDeviceManager = new GraphicsDeviceManager(this);
            graphicsDeviceManager.PreferredBackBufferHeight = size.Height;
            graphicsDeviceManager.PreferredBackBufferWidth = size.Win_Width;
            //コンテンツデータ（リソースデータ）のルートフォルダは"Contentに設定
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// 初期化処理（起動時、コンストラクタの後に1度だけ呼ばれる）
        /// </summary>
        protected override void Initialize()
        {
            map = new Map();
            key = new Key();
            player = new Player();
            coll = new Collition();
            positionBar = new PositionBar();
            enemy = new Enemy();
            button = new UI_Button();
            anime = new Anime();
            ui = new UI();
            sceneCount = new Scene_Count();
            bar = new Bar();
            time = new Time();
            ranking = new Ranking();
            music = new Music();

            base.Initialize();// 親クラスの初期化処理呼び出し。絶対に消すな！！
        }
        public void Init()
        {
            map.Init();
            bar.Init();
            player.Ini();
            enemy.Init();
            anime.Ini();
            ui.Ini();
            time.Init();
            key.Ini();
            button.Ini();
            positionBar.Init();
            ranking.Init();
            music.Init();
        }
        /// <summary>
        /// コンテンツデータ（リソースデータ）の読み込み処理
        /// （起動時、１度だけ呼ばれる）
        /// </summary>
        protected override void LoadContent()
        {
            // 画像を描画するために、スプライトバッチオブジェクトの実体生成
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // この下にロジックを記述
            anime.Load(Content);
            map.Load(Content);
            positionBar.Load(Content);
            enemy.Load(Content);

            button.Load(Content);
            ui.Load(Content, GraphicsDevice);

            bar.Load(Content);
            time.Load(Content);
            ranking.Load(Content);
            music.Load(Content);
            // この上にロジックを記述
        }

        /// <summary>
        /// コンテンツの解放処理
        /// （コンテンツ管理者以外で読み込んだコンテンツデータを解放）
        /// </summary>
        protected override void UnloadContent()
        {
            // この下にロジックを記述


            // この上にロジックを記述
        }

        /// <summary>
        /// 更新処理
        /// （1/60秒の１フレーム分の更新内容を記述。音再生はここで行う）
        /// </summary>
        /// <param name="gameTime">現在のゲーム時間を提供するオブジェクト</param>
        protected override void Update(GameTime gameTime)
        {
            // ゲーム終了処理（ゲームパッドのBackボタンかキーボードのエスケープボタンが押されたら終了）
            if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) ||
                 (Keyboard.GetState().IsKeyDown(Keys.Escape)))
            {
                Exit();
            }

            // この下に更新ロジックを記述
            key.Push_Triger();
            ui.Anime();

            switch (scene)
            {
                case Scene.title:
                    button.Button();
                    music.SongPlayer((int)Bgm.TITLE);
                    if (ui.Scene_Change(ui.Title(key.IsPushKey, music.Se[(int)SE.BOTAN])))
                    {
                        scene = Scene.tutlial;
                    }

                    break;
                case Scene.tutlial:
                    button.Button();
                    if (ui.Scene_Change(ui.Tutlial(key.IsPushKey, music.Se[(int)SE.BOTAN])))
                    {
                        music.SongStopper();
                        scene = Scene.play;
                    }
                    break;
                case Scene.play:
                    if (player.St != 4)
                    {
                        music.SongPlayer((int)Bgm.STAGE);
                        ui.Scroll(player.SC);
                        key.Update();
                        enemy.Update();
                        time.Updae(player.Pos);
                        positionBar.Update(player.Pos, enemy.Pos, (int)enemy.Size.Y);
                        map.FloorMove(size.Width);
                        map.FlagChange(player.Pos);
                        player.Update(key, bar.Accele, music.Se[(int)SE.MOVE], music.Se[(int)SE.HIGH]);

                        int fi = coll.FloorColl(player.Pos, player.R, map.FloorPos, map.Fsize);
                        int pi = coll.PrColl(player.Pos + player.Hit, player.R - player.Coll, map.PrPos, map.PrSize, map.PrSize_S, map.PrMoveF);
                        int ii = coll.ItemColl(player.Pos, player.R, map.ItemPos, map.ISize, map.InowGet, music.Se[(int)SE.ITEM]);
                        
                        if (player.Ac) //加速中は無敵
                        {
                            Debug.WriteLine("通過中");
                            player.Drop();
                        }
                        else　//普通は当たり判定
                        {
                            if (fi != -1 && player.DropF) //床
                            {
                                player.FloorMove(map.MovePos[fi], map.FloorPos[fi].Y);
                                Debug.WriteLine("乗る");
                            }
                            if (pi != -1 || coll.EnemyColl(player.Pos, player.R, enemy.Pos, enemy.Size)) //ダメージ
                            {
                                //if() //前フレームが無敵状態で当たっていれば 無効***
                                music.SePlay((int)SE.HIT);
                                player.DeadFlag();
                                anime.DD();
                                Debug.WriteLine("死ぬ");
                            }
                            else if (fi == -1 && pi == -1 && player.DropF) //何も当たってない&&ドロップ状態で落ちる
                                player.Drop();
                        }

                        if (ii != -1) //アイテム
                        {
                            map.ItemGet(ii);
                            bar.GetItem();
                        }
                    }
                    anime.Update(player.St, player.StPre, player.Pos, player.SC, player.Ac, player.AcPre);

                    if (ui.Scene_Change(sceneCount.Change(anime.Dead || player.GoalFlag())))
                    {
                        key.Ini();
                        music.SongStopper();
                        if (!anime.Dead) scene = Scene.end;
                        else scene = Scene.retry;
                    }
                    break;
                case Scene.retry:
                    music.OneSePlay((int)SE.DIE);
                    button.Button();
                    key.Update();
                    key.Re();
                    if (key.IsPushKey) music.SePlay((int)SE.BOTAN);
                    if (ui.Scene_Change(key.IsPushKey))
                    {
                        if (key.Re() == 1) scene = Scene.play;
                        else scene = Scene.title;
                        Init();
                    }
                    break;
                case Scene.end:
                    music.OneSePlay((int)SE.CLEAR);
                    ranking.GiveRanking(player.Pos, time.StopTime);
                    ui.End();
                    button.Button();
                    if (key.IsPushKey) music.SePlay((int)SE.BOTAN);
                    if (ui.Scene_Change(key.IsPushKey))
                    {
                        scene = Scene.title;
                        Init();
                    }
                    break;
            }
            // この上にロジックを記述
            base.Update(gameTime); // 親クラスの更新処理呼び出し。絶対に消すな！！
        }

        /// <summary>
        /// 描画処理
        /// </summary>
        /// <param name="gameTime">現在のゲーム時間を提供するオブジェクト</param>
        protected override void Draw(GameTime gameTime)
        {
            // 画面クリア時の色を設定
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            switch (scene)
            {
                case Scene.title:
                    button.Draw(spriteBatch);
                    ui.Draw_Title(spriteBatch);
                    break;
                case Scene.tutlial:
                    button.Draw(spriteBatch);
                    ui.Draw_Tutlial(spriteBatch);
                    break;
                case Scene.play:
                    map.Draw(spriteBatch, player.SC);
                    enemy.Draw(spriteBatch, player.SC);
                    anime.Draw(spriteBatch, player.Pos, player.SC, player.St); //プレイヤーは床より前か後か

                    ui.Draw(spriteBatch, player.SC);
                    positionBar.Draw(spriteBatch);
                    bar.Draw(spriteBatch);
                    time.Draw(spriteBatch);
                    ui.Draw_Back(spriteBatch);
                    break;
                case Scene.retry:
                    button.Draw(spriteBatch);
                    ui.Draw_Lose(spriteBatch);
                    break;
                case Scene.end:
                    button.Draw(spriteBatch);
                    ui.Draw_End(spriteBatch);
                    ranking.Draw(spriteBatch);
                    ui.Draw_Back(spriteBatch);//数字が隠れないので
                    break;
            }
            spriteBatch.End();

            base.Draw(gameTime); // 親クラスの更新処理呼び出し。絶対に消すな！！
        }
    }
}
