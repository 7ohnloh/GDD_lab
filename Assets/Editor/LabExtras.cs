using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

// One-off helpers for level setup (not part of the lab handout).
// Run from the menu: Tools > Lab Extras > Add Goombas And Solid Pipes
public static class LabExtras
{
    // world X positions for the extra goombas, in the later part of the level
    static readonly float[] extraGoombaX = { 11.3f, 22.2f, 33.3f };

    [MenuItem("Tools/Lab Extras/Add Goombas And Solid Pipes")]
    static void AddGoombasAndSolidPipes()
    {
        int goombas = AddGoombas();
        int colliders = MakePipesSolid();
        EditorSceneManager_MarkDirty();
        Debug.Log($"Lab Extras: added {goombas} goombas and {colliders} pipe colliders. Save the scene (File > Save).");
    }

    static int AddGoombas()
    {
        GameObject original = GameObject.Find("Enemies/brown_goomba");
        if (original == null)
        {
            Debug.LogWarning("Lab Extras: couldn't find Enemies/brown_goomba");
            return 0;
        }

        int added = 0;
        for (int i = 0; i < extraGoombaX.Length; i++)
        {
            string name = "brown_goomba (extra " + (i + 1) + ")";
            if (original.transform.parent.Find(name) != null) continue; // already added

            // copy keeps the stomp score event and sound; EnemyManager resets it on restart
            GameObject copy = Object.Instantiate(original, original.transform.parent);
            copy.name = name;
            Vector3 p = original.transform.position;
            copy.transform.position = new Vector3(extraGoombaX[i], p.y, p.z);
            Undo.RegisterCreatedObjectUndo(copy, "Add goomba");
            added++;
        }
        return added;
    }

    static int MakePipesSolid()
    {
        GameObject pipes = GameObject.Find("Pipes");
        if (pipes == null)
        {
            Debug.LogWarning("Lab Extras: couldn't find Pipes");
            return 0;
        }

        int added = 0;
        // every pipe head and body sprite gets a box collider sized to its sprite
        foreach (SpriteRenderer sr in pipes.GetComponentsInChildren<SpriteRenderer>(true))
        {
            if (sr.GetComponent<Collider2D>() != null) continue; // already solid
            Undo.AddComponent<BoxCollider2D>(sr.gameObject);
            added++;
        }
        return added;
    }

    // Boxes whose Coin slot was pointed at a mushroom or flower still contain their
    // original Coin child. Nothing hides it, so it pops (and scores) at game start.
    [MenuItem("Tools/Lab Extras/Hide Unused Box Coins")]
    static void HideUnusedBoxCoins()
    {
        int hidden = 0;
        foreach (BounceOnHitFromBelow box in Object.FindObjectsByType<BounceOnHitFromBelow>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            Transform coin = box.transform.parent != null ? box.transform.parent.Find("Coin") : null;
            if (coin == null) coin = box.transform.Find("Coin");
            if (coin == null || box.coin == coin.gameObject || !coin.gameObject.activeSelf) continue;

            Undo.RecordObject(coin.gameObject, "Hide unused coin");
            coin.gameObject.SetActive(false);
            hidden++;
        }
        EditorSceneManager_MarkDirty();
        Debug.Log($"Lab Extras: hid {hidden} unused coins. Save the scene (File > Save).");
    }

    // ---------------- Step 15: star buff FSM ----------------
    const string BuffFolder = "Assets/ScriptableObjects/BuffFSM";
    const float StarmanSeconds = 10f;              // how long the star lasts
    const float StarBoxLocalX = 20f;               // inside Obstacles (world X is about 22.8)

    [MenuItem("Tools/Lab Extras/Setup Star Buff")]
    static void SetupStarBuff()
    {
        // 1. FSM assets (created once, reused if they already exist)
        if (!AssetDatabase.IsValidFolder(BuffFolder))
            AssetDatabase.CreateFolder("Assets/ScriptableObjects", "BuffFSM");

        var startStarman = GetOrCreate<StartStarmanAction>("StartStarman");
        var exitStarman = GetOrCreate<ExitStarmanAction>("ExitStarman");
        var clearBuff = GetOrCreate<ClearBuffAction>("ClearBuff");
        var shouldStart = GetOrCreate<BuffDecision>("ShouldStartStarman");
        shouldStart.buffType = PowerupType.StarMan;
        var countdown = GetOrCreate<CountdownDecision>("StarmanCountdown");
        countdown.buffDuration = StarmanSeconds;

        State remain = AssetDatabase.LoadAssetAtPath<State>("Assets/ScriptableObjects/MarioFSM/RemainInState.asset");
        var buffDefault = GetOrCreate<State>("BuffDefault");
        var buffInvincible = GetOrCreate<State>("BuffInvincible");

        buffDefault.setupActions = new Action[0];
        buffDefault.actions = new Action[0];
        buffDefault.eventTriggeredActions = new EventAction[0];
        buffDefault.exitActions = new Action[0];
        buffDefault.transitions = new[] { new Transition { decision = shouldStart, trueState = buffInvincible, falseState = remain } };
        buffDefault.sceneGizmoColor = Color.grey;

        buffInvincible.setupActions = new Action[] { startStarman, clearBuff };
        buffInvincible.actions = new Action[0];
        buffInvincible.eventTriggeredActions = new EventAction[0];
        buffInvincible.exitActions = new Action[] { exitStarman };
        buffInvincible.transitions = new[] { new Transition { decision = countdown, trueState = buffDefault, falseState = remain } };
        buffInvincible.sceneGizmoColor = Color.yellow;

        foreach (Object o in new Object[] { shouldStart, countdown, buffDefault, buffInvincible }) EditorUtility.SetDirty(o);
        AssetDatabase.SaveAssets();

        // 2. Star music, next to the background music
        GameObject manager = GameObject.Find("GameManager");
        AudioSource bgm = GameObject.Find("GameManager/BackgroundMusic")?.GetComponent<AudioSource>();
        Transform starMusicT = manager.transform.Find("StarmanMusic");
        if (starMusicT == null)
        {
            GameObject sm = new GameObject("StarmanMusic");
            Undo.RegisterCreatedObjectUndo(sm, "Add StarmanMusic");
            sm.transform.SetParent(manager.transform, false);
            AudioSource src = sm.AddComponent<AudioSource>();
            src.clip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/05-starman.mp3");
            src.loop = true;
            src.playOnAwake = false;
            if (bgm != null) src.outputAudioMixerGroup = bgm.outputAudioMixerGroup;
            starMusicT = sm.transform;
        }

        // 3. BuffStateController on Mario
        GameObject mario = GameObject.Find("Mario");
        BuffStateController buff = mario.GetComponent<BuffStateController>();
        if (buff == null) buff = Undo.AddComponent<BuffStateController>(mario);
        Undo.RecordObject(buff, "Setup buff controller");
        buff.startState = buffDefault;
        buff.remainState = remain;
        buff.backgroundMusic = bgm;
        buff.starmanMusic = starMusicT.GetComponent<AudioSource>();

        // 4. OnGameRestart also resets the buff FSM
        SimpleGameEvent onRestart = AssetDatabase.LoadAssetAtPath<SimpleGameEvent>("Assets/ScriptableObjects/Events/OnGameRestart.asset");
        foreach (SimpleGameEventListener l in mario.GetComponents<SimpleGameEventListener>())
        {
            if (l.Event != onRestart) continue;
            bool already = false;
            for (int i = 0; i < l.Response.GetPersistentEventCount(); i++)
                if (l.Response.GetPersistentTarget(i) == buff && l.Response.GetPersistentMethodName(i) == "GameRestart") already = true;
            if (!already)
            {
                Undo.RecordObject(l, "Add buff restart");
                UnityEditor.Events.UnityEventTools.AddVoidPersistentListener(l.Response, buff.GameRestart);
            }
        }

        // 5. Star powerup in its own question box
        GameObject star = FindInactive("Star");   // hidden until its box is hit
        if (star == null)
        {
            GameObject mushroom = FindInactive("MagicMushroom");
            star = Object.Instantiate(mushroom, mushroom.transform.parent);
            star.name = "Star";
            Undo.RegisterCreatedObjectUndo(star, "Add star");
            MagicMushroom pm = star.GetComponent<MagicMushroom>();
            pm.powerupType = PowerupType.StarMan;
            foreach (Object a in AssetDatabase.LoadAllAssetsAtPath("Assets/Sprites/misc-3.gif"))
                if (a is Sprite sp && sp.name == "misc-3_48") star.GetComponent<SpriteRenderer>().sprite = sp;
        }

        Transform obstacles = GameObject.Find("Obstacles").transform;
        Transform starBox = obstacles.Find("Question-Box-Star");
        if (starBox == null)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Question-Box-Spring.prefab");
            GameObject box = (GameObject)PrefabUtility.InstantiatePrefab(prefab, obstacles);
            box.name = "Question-Box-Star";
            box.transform.localPosition = new Vector3(StarBoxLocalX, -3.71f, 0f);
            Undo.RegisterCreatedObjectUndo(box, "Add star box");
            starBox = box.transform;
        }
        // the star sits where its box is (like the mushroom), and the box pops the star, not its coin
        star.transform.position = new Vector3(starBox.position.x, 1.64f, 0f);
        BounceOnHitFromBelow bounce = starBox.GetComponentInChildren<BounceOnHitFromBelow>(true);
        Undo.RecordObject(bounce, "Point box at star");
        bounce.coin = star;
        PrefabUtility.RecordPrefabInstancePropertyModifications(bounce);
        Transform coin = starBox.Find("Coin");
        if (coin != null && coin.gameObject.activeSelf)
        {
            Undo.RecordObject(coin.gameObject, "Hide star box coin");
            coin.gameObject.SetActive(false);
            PrefabUtility.RecordPrefabInstancePropertyModifications(coin.gameObject);
        }

        EditorSceneManager_MarkDirty();
        Debug.Log("Lab Extras: star buff set up. Save the scene (File > Save).");
    }

    // ---------------- Step 16: finish line and World 1-2 ----------------
    const string World11Path = "Assets/Scenes/SampleScene.unity";
    const string World12Path = "Assets/Scenes/World-1-2.unity";
    const float FlagX = 34.5f;   // a bit before EndLimit so the camera can show it

    [MenuItem("Tools/Lab Extras/Setup Finish Line And World 1-2")]
    static void SetupWorld12()
    {
        Scene active = SceneManager.GetActiveScene();
        if (active.path != World11Path)
        {
            EditorUtility.DisplayDialog("Lab Extras", "Open SampleScene first, then run this again.", "OK");
            return;
        }
        if (!UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        // 1. World 1-1 (SampleScene): flagpole -> World-1-2, score starts at 0
        AddFlag("World-1-2");
        SetResetScore(true);
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(SceneManager.GetActiveScene());

        // 2. World 1-2 = a copy of 1-1 (created once)
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(World12Path) == null)
            AssetDatabase.CopyAsset(World11Path, World12Path);
        AddToBuildSettings(World12Path);

        // 3. Make 1-2 different: underground colours and music, score carried over, flag -> MainMenu
        Scene w12 = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(World12Path);
        Camera cam = Camera.main;
        if (cam != null) cam.backgroundColor = Color.black;
        AudioSource bgm = GameObject.Find("GameManager/BackgroundMusic")?.GetComponent<AudioSource>();
        if (bgm != null) bgm.clip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/02-underworld.mp3");
        SetResetScore(false);
        FinishLine flag = Object.FindAnyObjectByType<FinishLine>();
        if (flag != null) flag.nextScene = "MainMenu";
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(w12);

        // 4. Back to World 1-1
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene(World11Path);
        Debug.Log("Lab Extras: flagpole added; World-1-2 created and added to the build. Both scenes are saved.");
    }

    static void AddFlag(string nextScene)
    {
        FinishLine existing = Object.FindAnyObjectByType<FinishLine>();
        if (existing != null) { existing.nextScene = nextScene; EditorUtility.SetDirty(existing); return; }

        GameObject flag = new GameObject("FinishLine");
        Undo.RegisterCreatedObjectUndo(flag, "Add finish line");
        SpriteRenderer sr = flag.AddComponent<SpriteRenderer>();
        foreach (Object a in AssetDatabase.LoadAllAssetsAtPath("Assets/Sprites/misc-3.gif"))
            if (a is Sprite sp && sp.name == "misc-3_9") sr.sprite = sp;   // green flagpole with its base block

        // stand the pole on the ground: find the ground top under it
        Physics2D.SyncTransforms();
        float groundTop = -4.3f;
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(FlagX, 20f), Vector2.down, 40f, 1 << 3); // Ground layer
        if (hit.collider != null) groundTop = hit.point.y;
        float halfHeight = sr.sprite != null ? sr.sprite.bounds.extents.y : 5.25f;
        flag.transform.position = new Vector3(FlagX, groundTop + halfHeight, 0f);

        BoxCollider2D box = flag.AddComponent<BoxCollider2D>();
        box.isTrigger = true;
        AudioSource src = flag.AddComponent<AudioSource>();
        src.clip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/smb_flagpole.wav");
        src.playOnAwake = false;
        AudioSource bgm = GameObject.Find("GameManager/BackgroundMusic")?.GetComponent<AudioSource>();
        if (bgm != null) src.outputAudioMixerGroup = bgm.outputAudioMixerGroup;
        FinishLine fl = flag.AddComponent<FinishLine>();
        fl.nextScene = nextScene;
    }

    static void SetResetScore(bool reset)
    {
        GameManager gm = Object.FindAnyObjectByType<GameManager>();
        if (gm == null) return;
        Undo.RecordObject(gm, "Set score reset");
        gm.resetScoreOnStart = reset;
        EditorUtility.SetDirty(gm);
    }

    static void AddToBuildSettings(string path)
    {
        var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        foreach (var s in scenes) if (s.path == path) return;
        scenes.Add(new EditorBuildSettingsScene(path, true));
        EditorBuildSettings.scenes = scenes.ToArray();
    }

    // ---------------- Step 17: sound when the blink (InvincibleSmallMario) ends ----------------
    [MenuItem("Tools/Lab Extras/Add State Change Sounds")]
    static void AddStateChangeSounds()
    {
        const string folder = "Assets/ScriptableObjects/MarioFSM";
        string path = folder + "/PlayBlinkEndSound.asset";
        PlaySoundAction play = AssetDatabase.LoadAssetAtPath<PlaySoundAction>(path);
        if (play == null)
        {
            play = ScriptableObject.CreateInstance<PlaySoundAction>();
            AssetDatabase.CreateAsset(play, path);
        }
        play.clip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Sounds/smb_bump.wav");
        EditorUtility.SetDirty(play);

        State blink = AssetDatabase.LoadAssetAtPath<State>(folder + "/InvincibleSmallMario.asset");
        if (System.Array.IndexOf(blink.exitActions, play) < 0)
        {
            var list = new System.Collections.Generic.List<Action>(blink.exitActions) { play };
            blink.exitActions = list.ToArray();
            EditorUtility.SetDirty(blink);
        }
        AssetDatabase.SaveAssets();
        Debug.Log("Lab Extras: InvincibleSmallMario now plays smb_bump when the blink ends.");
    }

    // ---------------- Flower without mushroom: small Mario in fire colours ----------------
    [MenuItem("Tools/Lab Extras/Setup Small Fire Mario")]
    static void SetupSmallFireMario()
    {
        const string fsm = "Assets/ScriptableObjects/MarioFSM/";
        const string spritePath = "Assets/Sprites/mario-small-fire.png";

        // 1. import the recoloured sprite like the other Mario sprites (16 px per unit, crisp pixels)
        TextureImporter ti = (TextureImporter)AssetImporter.GetAtPath(spritePath);
        ti.textureType = TextureImporterType.Sprite;
        ti.spriteImportMode = SpriteImportMode.Single;
        ti.spritePixelsPerUnit = 16;
        ti.filterMode = FilterMode.Point;
        ti.textureCompression = TextureImporterCompression.Uncompressed;
        ti.SaveAndReimport();
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);

        // 2. animation clip + animator controller, same simple pose setup as SmallMario
        string clipPath = "Assets/Animations/Mario/mario-small-fire.anim";
        AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
        if (clip == null)
        {
            clip = new AnimationClip { frameRate = 60 };
            var binding = EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite");
            AnimationUtility.SetObjectReferenceCurve(clip, binding, new[] { new ObjectReferenceKeyframe { time = 0, value = sprite } });
            var settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = true;
            AnimationUtility.SetAnimationClipSettings(clip, settings);
            AssetDatabase.CreateAsset(clip, clipPath);
        }
        string ctrlPath = "Assets/Animations/Mario/SmallFireMario.controller";
        RuntimeAnimatorController ctrl = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(ctrlPath);
        if (ctrl == null) ctrl = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPathWithClip(ctrlPath, clip);

        // 3. FSM pieces
        SetupAnimator setup = AssetDatabase.LoadAssetAtPath<SetupAnimator>(fsm + "SetupSmallFireMario.asset");
        if (setup == null) { setup = ScriptableObject.CreateInstance<SetupAnimator>(); AssetDatabase.CreateAsset(setup, fsm + "SetupSmallFireMario.asset"); }
        setup.animatorController = ctrl;

        TransformDecision toSmallFire = AssetDatabase.LoadAssetAtPath<TransformDecision>(fsm + "ShouldTransformToSmallFireMario.asset");
        if (toSmallFire == null) { toSmallFire = ScriptableObject.CreateInstance<TransformDecision>(); AssetDatabase.CreateAsset(toSmallFire, fsm + "ShouldTransformToSmallFireMario.asset"); }
        toSmallFire.map = new[] { new StateTransformMap { fromState = MarioState.SmallMario, powerupCollected = PowerupType.FireFlower } };

        // big Fire Mario now comes from Super + flower, or small-fire + mushroom
        TransformDecision toFire = AssetDatabase.LoadAssetAtPath<TransformDecision>(fsm + "ShouldTransformToFireMario.asset");
        toFire.map = new[] {
            new StateTransformMap { fromState = MarioState.SuperMario, powerupCollected = PowerupType.FireFlower },
            new StateTransformMap { fromState = MarioState.SmallFireMario, powerupCollected = PowerupType.MagicMushroom },
        };
        // small-fire Mario hit -> loses fire, blinks small (like Super/Fire)
        TransformDecision toInv = AssetDatabase.LoadAssetAtPath<TransformDecision>(fsm + "ShouldTransformToInvincibleMario.asset");
        bool hasSF = false;
        foreach (var m in toInv.map) if (m.fromState == MarioState.SmallFireMario) hasSF = true;
        if (!hasSF)
        {
            var l = new System.Collections.Generic.List<StateTransformMap>(toInv.map) { new StateTransformMap { fromState = MarioState.SmallFireMario, powerupCollected = PowerupType.Damage } };
            toInv.map = l.ToArray();
        }

        State remain = AssetDatabase.LoadAssetAtPath<State>(fsm + "RemainInState.asset");
        State fireState = AssetDatabase.LoadAssetAtPath<State>(fsm + "FireMario.asset");
        State invState = AssetDatabase.LoadAssetAtPath<State>(fsm + "InvincibleSmallMario.asset");
        Action clearPowerup = AssetDatabase.LoadAssetAtPath<Action>(fsm + "ClearPowerup.asset");
        Action fireAttack = AssetDatabase.LoadAssetAtPath<Action>(fsm + "FireAttack.asset");

        State smallFire = AssetDatabase.LoadAssetAtPath<State>(fsm + "SmallFireMario.asset");
        if (smallFire == null) { smallFire = ScriptableObject.CreateInstance<State>(); AssetDatabase.CreateAsset(smallFire, fsm + "SmallFireMario.asset"); }
        smallFire.setupActions = new Action[] { setup, clearPowerup };
        smallFire.actions = new Action[0];
        smallFire.eventTriggeredActions = new[] { new EventAction { action = fireAttack, type = ActionType.Attack } };
        smallFire.exitActions = new Action[0];
        smallFire.transitions = new[] {
            new Transition { decision = toFire, trueState = fireState, falseState = remain },
            new Transition { decision = toInv, trueState = invState, falseState = remain },
        };
        smallFire.sceneGizmoColor = new Color(1f, 0.5f, 0f);

        // SmallMario + flower now goes to SmallFireMario instead of big FireMario
        State small = AssetDatabase.LoadAssetAtPath<State>(fsm + "SmallMario.asset");
        for (int i = 0; i < small.transitions.Length; i++)
            if (small.transitions[i].decision == toFire)
            {
                small.transitions[i].decision = toSmallFire;
                small.transitions[i].trueState = smallFire;
            }

        foreach (Object o in new Object[] { setup, toSmallFire, toFire, toInv, smallFire, small }) EditorUtility.SetDirty(o);
        AssetDatabase.SaveAssets();
        Debug.Log("Lab Extras: SmallFireMario set up. Small + flower = small fire Mario; then mushroom = big Fire Mario.");
    }

    static T GetOrCreate<T>(string name) where T : ScriptableObject
    {
        string path = BuffFolder + "/" + name + ".asset";
        T asset = AssetDatabase.LoadAssetAtPath<T>(path);
        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
        }
        return asset;
    }

    static GameObject FindInactive(string name)
    {
        foreach (GameObject g in Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (g.name == name && g.scene.IsValid()) return g;
        return null;
    }

    static void EditorSceneManager_MarkDirty()
    {
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }
}
