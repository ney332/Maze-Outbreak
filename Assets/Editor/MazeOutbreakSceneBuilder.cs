using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class MazeOutbreakSceneBuilder
{
    private const float CellSize = 4f;
    private const float WallHeight = 3.2f;
    private const string ZombieModelPath = "Assets/Models/Zombie/ZOMBIEN_3D_MODEL_FREE_BY_Oscar_Creativo.fbx";
    private const string ZombieTextureFolder = "Assets/Models/Zombie/Textures/ZOMBIEN/";
    private const string TrophyModelPath = "Assets/Models/Trophy/Trofeu_Libertadores.fbx";
    private const string BackgroundMusicPath = "Assets/Audio/intense_horror_music_01.mp3";
    private const string ZombieFootstepAudioPath = "Assets/Audio/passos-terror.mp3";

    [MenuItem("Maze Outbreak/Build Complete Project")]
    public static void BuildAll()
    {
        EnsureFolders();
        CreateMaterials();
        CreatePrefabs();
        CreateMenuScene();
        CreateEndScene("GameOver", "GAME OVER", "Voce foi pego.");
        CreateEndScene("Victory", "VITORIA", "Voce escapou do Maze Outbreak.");
        CreateLevelScenes();
        ConfigureBuildSettings();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Maze Outbreak project generated.");
    }

    [MenuItem("Maze Outbreak/Build macOS Standalone")]
    public static void BuildMacOSStandalone()
    {
        BuildAll();
        string output = "Builds/MazeOutbreak_macOS/Maze Outbreak.app";
        Directory.CreateDirectory(Path.GetDirectoryName(output));
        BuildReport report = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, output, BuildTarget.StandaloneOSX, BuildOptions.None);
        Debug.Log("macOS build result: " + report.summary.result);
    }

    [MenuItem("Maze Outbreak/Build Windows 64-bit")]
    public static void BuildWindows64()
    {
        BuildAll();
        string output = "Builds/MazeOutbreak_Windows/MazeOutbreak.exe";
        Directory.CreateDirectory(Path.GetDirectoryName(output));
        BuildReport report = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, output, BuildTarget.StandaloneWindows64, BuildOptions.None);
        Debug.Log("Windows build result: " + report.summary.result);
    }

    private static void EnsureFolders()
    {
        foreach (string folder in new[] { "Scenes", "Scripts", "Prefabs", "Materials", "Audio", "UI", "Documentation", "Editor", "Animations" })
        {
            string path = "Assets/" + folder;
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder("Assets", folder);
            }
        }
    }

    private static void CreateMaterials()
    {
        SaveWallMaterial("Wall_Abandoned", new Color(0.78f, 0.78f, 0.45f));
        SaveWallMaterial("Wall_Subterranean", new Color(0.55f, 0.57f, 0.34f));
        SaveWallMaterial("Wall_Contaminated", new Color(0.48f, 0.62f, 0.32f));
        SaveWallMaterial("Wall_Final", new Color(0.34f, 0.36f, 0.22f));
        SaveFloorMaterial();
        SaveCeilingMaterial();
        SaveMaterial("Player", new Color(0.15f, 0.24f, 0.35f));
        SaveMaterial("Zombie", new Color(0.26f, 0.45f, 0.22f));
        SaveMaterial("Exit", new Color(0.1f, 0.8f, 0.45f));
        SaveMaterial("TrophyGold", new Color(1f, 0.74f, 0.22f));
        SaveMaterial("Contamination", new Color(0.05f, 0.7f, 0.15f));
    }

    private static Material SaveMaterial(string name, Color color)
    {
        string path = $"Assets/Materials/{name}.mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            material = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"));
            AssetDatabase.CreateAsset(material, path);
        }

        material.color = color;
        return material;
    }

    private static Material SaveWallMaterial(string name, Color tint)
    {
        TextureImporter importer = AssetImporter.GetAtPath("Assets/Textures/bwa_7.jpeg") as TextureImporter;
        if (importer != null)
        {
            importer.wrapMode = TextureWrapMode.Repeat;
            importer.mipmapEnabled = true;
            importer.SaveAndReimport();
        }

        Material material = SaveMaterial(name, tint);
        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/bwa_7.jpeg");
        if (texture != null)
        {
            material.mainTexture = texture;
            material.mainTextureScale = Vector2.one;
        }

        return material;
    }

    private static Material SaveFloorMaterial()
    {
        TextureImporter importer = AssetImporter.GetAtPath("Assets/Textures/floor_1.png") as TextureImporter;
        if (importer != null)
        {
            importer.wrapMode = TextureWrapMode.Repeat;
            importer.mipmapEnabled = true;
            importer.SaveAndReimport();
        }

        Material material = SaveMaterial("Floor", Color.white);
        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/floor_1.png");
        if (texture != null)
        {
            material.mainTexture = texture;
            material.mainTextureScale = Vector2.one;
        }

        return material;
    }

    private static Material SaveCeilingMaterial()
    {
        TextureImporter importer = AssetImporter.GetAtPath("Assets/Textures/backrooms_roof_4.png") as TextureImporter;
        if (importer != null)
        {
            importer.wrapMode = TextureWrapMode.Repeat;
            importer.mipmapEnabled = true;
            importer.SaveAndReimport();
        }

        Material material = SaveMaterial("Ceiling", Color.white);
        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Textures/backrooms_roof_4.png");
        if (texture != null)
        {
            material.mainTexture = texture;
            material.mainTextureScale = Vector2.one;
        }

        return material;
    }

    private static void CreatePrefabs()
    {
        ConfigureZombieModelImporter();
        ConfigureZombieTextureImporters();
        CreateZombieMaterials();
        ConfigureTrophyModelImporter();

        GameObject zombie = new GameObject("Zombie");
        zombie.name = "Zombie";
        CharacterController controller = zombie.AddComponent<CharacterController>();
        controller.height = 2.4f;
        controller.radius = 0.55f;
        controller.center = new Vector3(0f, 1.2f, 0f);
        zombie.AddComponent<ZombieAI>();
        zombie.AddComponent<PlayerDetection>();

        GameObject modelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ZombieModelPath);
        if (modelPrefab != null)
        {
            GameObject model = PrefabUtility.InstantiatePrefab(modelPrefab) as GameObject;
            model.name = "ZombieAnimatedModel";
            model.transform.SetParent(zombie.transform, false);
            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;
            model.transform.localScale = Vector3.one;
            FitVisualHeight(model, 2.25f);
            AlignVisualBottomToGround(model);
            ApplyZombieMaterials(model);

            Animator animator = model.GetComponent<Animator>();
            if (animator == null)
            {
                animator = model.AddComponent<Animator>();
            }
            animator.applyRootMotion = false;
            animator.runtimeAnimatorController = CreateZombieAnimatorController();
        }
        else
        {
            GameObject fallback = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            fallback.name = "ZombiePlaceholderVisual";
            fallback.transform.SetParent(zombie.transform, false);
            fallback.transform.localPosition = new Vector3(0f, 1.2f, 0f);
            fallback.transform.localScale = new Vector3(1.1f, 1.3f, 1.1f);
            fallback.GetComponent<Renderer>().sharedMaterial = LoadMat("Zombie");
            Object.DestroyImmediate(fallback.GetComponent<CapsuleCollider>());
        }

        PrefabUtility.SaveAsPrefabAsset(zombie, "Assets/Prefabs/Zombie.prefab");
        Object.DestroyImmediate(zombie);

        GameObject exit = GameObject.CreatePrimitive(PrimitiveType.Cube);
        exit.name = "ExitDoor";
        exit.transform.localScale = Vector3.one;
        Renderer exitRenderer = exit.GetComponent<Renderer>();
        if (exitRenderer != null)
        {
            exitRenderer.enabled = false;
        }
        Collider exitCollider = exit.GetComponent<Collider>();
        exitCollider.isTrigger = true;
        BoxCollider exitBox = exitCollider as BoxCollider;
        if (exitBox != null)
        {
            exitBox.size = new Vector3(2.4f, 2.4f, 2.4f);
            exitBox.center = new Vector3(0f, 1.2f, 0f);
        }
        CreateTrophyVisual(exit.transform);
        exit.AddComponent<ExitDoor>();
        PrefabUtility.SaveAsPrefabAsset(exit, "Assets/Prefabs/ExitDoor.prefab");
        Object.DestroyImmediate(exit);
    }

    private static void ConfigureTrophyModelImporter()
    {
        ModelImporter importer = AssetImporter.GetAtPath(TrophyModelPath) as ModelImporter;
        if (importer == null)
        {
            return;
        }

        importer.importAnimation = false;
        importer.materialImportMode = ModelImporterMaterialImportMode.ImportViaMaterialDescription;
        importer.SaveAndReimport();
    }

    private static void CreateTrophyVisual(Transform parent)
    {
        GameObject trophyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(TrophyModelPath);
        if (trophyPrefab == null)
        {
            GameObject fallback = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            fallback.name = "TrophyPlaceholder";
            fallback.transform.SetParent(parent, false);
            fallback.transform.localPosition = Vector3.zero;
            fallback.transform.localScale = new Vector3(0.7f, 1.4f, 0.7f);
            fallback.GetComponent<Renderer>().sharedMaterial = LoadMat("TrophyGold");
            Object.DestroyImmediate(fallback.GetComponent<Collider>());
            return;
        }

        GameObject trophy = PrefabUtility.InstantiatePrefab(trophyPrefab) as GameObject;
        trophy.name = "TrophyVisual";
        trophy.transform.SetParent(parent, false);
        trophy.transform.localPosition = Vector3.zero;
        trophy.transform.localRotation = Quaternion.identity;
        trophy.transform.localScale = Vector3.one;
        OrientLongestAxisUpright(trophy);
        FitVisualHeight(trophy, 1.7f);
        AlignVisualBottomToGround(trophy);
        ApplyMaterialToRenderers(trophy, LoadMat("TrophyGold"));
        RemoveColliders(trophy);
    }

    private static void ConfigureZombieModelImporter()
    {
        string modelPath = ZombieModelPath;
        ModelImporter importer = AssetImporter.GetAtPath(modelPath) as ModelImporter;
        if (importer == null)
        {
            return;
        }

        importer.importAnimation = true;
        importer.animationType = ModelImporterAnimationType.Human;
        importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
        importer.optimizeGameObjects = false;
        GameObject modelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
        importer.humanDescription = CreateZombieHumanDescription(modelPrefab);

        ModelImporterClipAnimation[] clips = importer.defaultClipAnimations;
        if (clips != null && clips.Length > 0)
        {
            clips[0].name = "Walk";
            clips[0].loopTime = true;
            clips[0].loopPose = true;
            clips[0].lockRootPositionXZ = true;
            clips[0].lockRootHeightY = true;
            clips[0].lockRootRotation = true;
            importer.clipAnimations = clips;
        }

        importer.SaveAndReimport();
    }

    private static HumanDescription CreateZombieHumanDescription(GameObject modelPrefab)
    {
        HumanDescription description = new HumanDescription();
        description.human = new[]
        {
            HumanBone("Hips", "CC_Base_Hip"),
            HumanBone("LeftUpperLeg", "CC_Base_L_Thigh"),
            HumanBone("LeftLowerLeg", "CC_Base_L_Calf"),
            HumanBone("LeftFoot", "CC_Base_L_Foot"),
            HumanBone("LeftToes", "CC_Base_L_ToeBase"),
            HumanBone("RightUpperLeg", "CC_Base_R_Thigh"),
            HumanBone("RightLowerLeg", "CC_Base_R_Calf"),
            HumanBone("RightFoot", "CC_Base_R_Foot"),
            HumanBone("RightToes", "CC_Base_R_ToeBase"),
            HumanBone("Spine", "CC_Base_Waist"),
            HumanBone("Chest", "CC_Base_Spine01"),
            HumanBone("UpperChest", "CC_Base_Spine02"),
            HumanBone("Neck", "CC_Base_NeckTwist02"),
            HumanBone("Head", "CC_Base_Head"),
            HumanBone("LeftShoulder", "CC_Base_L_Clavicle"),
            HumanBone("LeftUpperArm", "CC_Base_L_Upperarm"),
            HumanBone("LeftLowerArm", "CC_Base_L_Forearm"),
            HumanBone("LeftHand", "CC_Base_L_Hand"),
            HumanBone("RightShoulder", "CC_Base_R_Clavicle"),
            HumanBone("RightUpperArm", "CC_Base_R_Upperarm"),
            HumanBone("RightLowerArm", "CC_Base_R_Forearm"),
            HumanBone("RightHand", "CC_Base_R_Hand")
        };

        description.skeleton = BuildSkeletonBones(modelPrefab);
        description.upperArmTwist = 0.5f;
        description.lowerArmTwist = 0.5f;
        description.upperLegTwist = 0.5f;
        description.lowerLegTwist = 0.5f;
        description.armStretch = 0.05f;
        description.legStretch = 0.05f;
        description.feetSpacing = 0f;
        description.hasTranslationDoF = false;
        return description;
    }

    private static SkeletonBone[] BuildSkeletonBones(GameObject modelPrefab)
    {
        if (modelPrefab == null)
        {
            return new SkeletonBone[0];
        }

        List<SkeletonBone> bones = new List<SkeletonBone>();
        Transform[] transforms = modelPrefab.GetComponentsInChildren<Transform>(true);
        foreach (Transform transform in transforms)
        {
            SkeletonBone bone = new SkeletonBone();
            bone.name = transform.name;
            bone.position = transform.localPosition;
            bone.rotation = transform.localRotation;
            bone.scale = transform.localScale;
            bones.Add(bone);
        }

        return bones.ToArray();
    }

    private static HumanBone HumanBone(string humanName, string boneName)
    {
        HumanLimit limit = new HumanLimit();
        limit.useDefaultValues = true;

        HumanBone bone = new HumanBone();
        bone.humanName = humanName;
        bone.boneName = boneName;
        bone.limit = limit;
        return bone;
    }

    private static RuntimeAnimatorController CreateZombieAnimatorController()
    {
        AnimationClip walkClip = FindZombieWalkClip();
        if (walkClip == null)
        {
            return null;
        }

        string controllerPath = "Assets/Animations/ZombieAnimator.controller";
        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
        if (controller == null)
        {
            controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
        }

        AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;
        for (int i = stateMachine.states.Length - 1; i >= 0; i--)
        {
            stateMachine.RemoveState(stateMachine.states[i].state);
        }

        AnimatorState state = stateMachine.AddState("Walk");
        state.motion = walkClip;
        state.speed = 1f;
        stateMachine.defaultState = state;
        EditorUtility.SetDirty(controller);
        return controller;
    }

    private static AnimationClip FindZombieWalkClip()
    {
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(ZombieModelPath);
        foreach (Object asset in assets)
        {
            AnimationClip clip = asset as AnimationClip;
            if (clip != null && !clip.name.StartsWith("__preview"))
            {
                return clip;
            }
        }

        return null;
    }

    private static void FitVisualHeight(GameObject visual, float targetHeight)
    {
        Renderer[] renderers = visual.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            return;
        }

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        if (bounds.size.y > 0.01f)
        {
            float scale = targetHeight / bounds.size.y;
            visual.transform.localScale *= scale;
        }
    }

    private static void AlignVisualBottomToGround(GameObject visual)
    {
        Renderer[] renderers = visual.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            return;
        }

        float minY = float.PositiveInfinity;
        foreach (Renderer renderer in renderers)
        {
            minY = Mathf.Min(minY, renderer.bounds.min.y);
        }

        if (!float.IsInfinity(minY))
        {
            visual.transform.localPosition += Vector3.up * -minY;
        }
    }

    private static void OrientLongestAxisUpright(GameObject visual)
    {
        Renderer[] renderers = visual.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            return;
        }

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        Vector3 size = bounds.size;
        if (size.x > size.y && size.x >= size.z)
        {
            visual.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
        }
        else if (size.z > size.y && size.z > size.x)
        {
            visual.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
        }
    }

    private static void ApplyMaterialToRenderers(GameObject target, Material material)
    {
        if (material == null)
        {
            return;
        }

        foreach (Renderer renderer in target.GetComponentsInChildren<Renderer>(true))
        {
            Material[] materials = renderer.sharedMaterials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = material;
            }
            renderer.sharedMaterials = materials;
        }
    }

    private static void RemoveColliders(GameObject target)
    {
        foreach (Collider collider in target.GetComponentsInChildren<Collider>(true))
        {
            Object.DestroyImmediate(collider);
        }
    }

    private static void ConfigureZombieTextureImporters()
    {
        string[] guids = AssetDatabase.FindAssets("t:Texture", new[] { ZombieTextureFolder.TrimEnd('/') });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
            {
                continue;
            }

            importer.wrapMode = TextureWrapMode.Repeat;
            importer.mipmapEnabled = true;
            importer.textureType = path.Contains("_Normal") ? TextureImporterType.NormalMap : TextureImporterType.Default;
            importer.SaveAndReimport();
        }
    }

    private static void CreateZombieMaterials()
    {
        CreateZombieMaterial("Std_Skin_Head", "MODEL_BAJA_Std_Skin_Head_BaseColor.1001.jpeg", "MODEL_BAJA_Std_Skin_Head_Normal.1001.jpeg", "MODEL_BAJA_Std_Skin_Head_Metallic.1001.jpeg", "MODEL_BAJA_Std_Skin_Head_Roughness.1001.jpeg");
        CreateZombieMaterial("Std_Skin_Body", "MODEL_BAJA_Std_Skin_Head_BaseColor.1002.jpeg", "MODEL_BAJA_Std_Skin_Head_Normal.1002.jpeg", "MODEL_BAJA_Std_Skin_Head_Metallic.1002.jpeg", "MODEL_BAJA_Std_Skin_Head_Roughness.1002.jpeg");
        CreateZombieMaterial("Std_Skin_Arm", "MODEL_BAJA_Std_Skin_Head_BaseColor.1003.jpeg", "MODEL_BAJA_Std_Skin_Head_Normal.1003.jpeg", "MODEL_BAJA_Std_Skin_Head_Metallic.1003.jpeg", "MODEL_BAJA_Std_Skin_Head_Roughness.1003.jpeg");
        CreateZombieMaterial("Std_Skin_Leg", "MODEL_BAJA_Std_Skin_Head_BaseColor.1004.jpeg", "MODEL_BAJA_Std_Skin_Head_Normal.1004.jpeg", null, "MODEL_BAJA_Std_Skin_Head_Roughness.1004.jpeg");
        CreateZombieMaterial("Std_Nails", "MODEL_BAJA_Std_Skin_Head_BaseColor.1005.jpeg", "MODEL_BAJA_Std_Skin_Head_Normal.1005.jpeg", null, "MODEL_BAJA_Std_Skin_Head_Roughness.1005.jpeg");
        CreateZombieMaterial("Std_Eye_L", "eye.jpeg", null, null, null);
        CreateZombieMaterial("Std_Eye_R", "eye.jpeg", null, null, null);
        CreateZombieMaterial("Std_Cornea_L", "eye.jpeg", null, null, null);
        CreateZombieMaterial("Std_Cornea_R", "eye.jpeg", null, null, null);
        CreateZombieMaterial("Std_Tongue", "Std_Tongue_Diffuse.jpeg", "Std_Tongue_Normal.jpeg", "Std_Tongue_Metallic.jpeg", null);
        CreateZombieMaterial("Std_Upper_Teeth", "Std_Upper_Teeth_Diffuse.jpeg", "Std_Upper_Teeth_Normal.jpeg", null, null);
        CreateZombieMaterial("Std_Lower_Teeth", "Std_Lower_Teeth_Diffuse.jpeg", "Std_Lower_Teeth_Normal.jpeg", null, null);
        CreateZombieMaterial("Std_Eyelash", null, null, null, null);
        CreateZombieMaterial("Std_Eye_Occlusion_L", "Std_Cornea_L_ao.jpeg", null, null, null);
        CreateZombieMaterial("Std_Eye_Occlusion_R", "Std_Cornea_L_ao.jpeg", null, null, null);
    }

    private static Material CreateZombieMaterial(string materialName, string baseMap, string normalMap, string metallicMap, string roughnessMap)
    {
        string path = "Assets/Materials/Zombie_" + materialName + ".mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            material = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"));
            AssetDatabase.CreateAsset(material, path);
        }

        material.name = "Zombie_" + materialName;
        material.color = Color.white;
        SetMaterialTexture(material, "_BaseMap", baseMap);
        SetMaterialTexture(material, "_MainTex", baseMap);
        SetMaterialTexture(material, "_BumpMap", normalMap);
        SetMaterialTexture(material, "_MetallicGlossMap", metallicMap);
        SetMaterialTexture(material, "_OcclusionMap", roughnessMap);
        if (!string.IsNullOrEmpty(normalMap) && material.HasProperty("_BumpScale"))
        {
            material.SetFloat("_BumpScale", 0.65f);
            material.EnableKeyword("_NORMALMAP");
        }

        if (!string.IsNullOrEmpty(metallicMap) && material.HasProperty("_Metallic"))
        {
            material.SetFloat("_Metallic", 0.1f);
        }

        if (material.HasProperty("_Smoothness"))
        {
            material.SetFloat("_Smoothness", string.IsNullOrEmpty(roughnessMap) ? 0.25f : 0.18f);
        }

        EditorUtility.SetDirty(material);
        return material;
    }

    private static void SetMaterialTexture(Material material, string propertyName, string textureName)
    {
        if (string.IsNullOrEmpty(textureName) || !material.HasProperty(propertyName))
        {
            return;
        }

        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(ZombieTextureFolder + textureName);
        if (texture != null)
        {
            material.SetTexture(propertyName, texture);
        }
    }

    private static void ApplyZombieMaterials(GameObject model)
    {
        Renderer[] renderers = model.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.sharedMaterials;
            for (int i = 0; i < materials.Length; i++)
            {
                string sourceName = materials[i] != null ? materials[i].name.Replace(" (Instance)", string.Empty) : string.Empty;
                Material replacement = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Zombie_" + sourceName + ".mat");
                if (replacement != null)
                {
                    materials[i] = replacement;
                }
            }

            renderer.sharedMaterials = materials;
        }
    }

    private static void CreateMenuScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "MainMenu";
        CreateCamera(new Vector3(0f, 3f, -10f), Quaternion.Euler(12f, 0f, 0f), Color.black);
        CreateGameManager();

        Canvas canvas = CreateCanvas("MainMenuCanvas");
        MainMenu menu = canvas.gameObject.AddComponent<MainMenu>();
        GameObject main = CreatePanel(canvas.transform, "MainPanel", new Color(0.02f, 0.02f, 0.025f, 0.92f));
        GameObject levels = CreatePanel(canvas.transform, "LevelSelectPanel", new Color(0.02f, 0.02f, 0.025f, 0.92f));
        GameObject credits = CreatePanel(canvas.transform, "CreditsPanel", new Color(0.02f, 0.02f, 0.025f, 0.92f));
        menu.mainPanel = main;
        menu.levelSelectPanel = levels;
        menu.creditsPanel = credits;

        CreateTitle(main.transform, "MAZE OUTBREAK", 52, 140f);
        CreateButton(main.transform, "Iniciar Jogo", new Vector2(0f, 40f), menu.StartGame);
        CreateButton(main.transform, "Selecionar Fase", new Vector2(0f, -20f), menu.OpenLevelSelect);
        CreateButton(main.transform, "Creditos", new Vector2(0f, -80f), menu.OpenCredits);
        CreateButton(main.transform, "Sair", new Vector2(0f, -140f), menu.QuitGame);

        CreateTitle(levels.transform, "SELECIONAR FASE", 40, 140f);
        for (int i = 1; i <= 4; i++)
        {
            int level = i;
            CreateButton(levels.transform, "Fase " + i, new Vector2(0f, 90f - i * 55f), menu.LoadLevel, level);
        }
        CreateButton(levels.transform, "Voltar", new Vector2(0f, -170f), menu.BackToMain);

        CreateTitle(credits.transform, "CREDITOS", 40, 110f);
        CreateText(credits.transform, "Maze Outbreak\nProjeto Unity C#\nAssets placeholders nativos da Unity", 24, new Vector2(0f, 10f), new Vector2(680f, 160f));
        CreateButton(credits.transform, "Voltar", new Vector2(0f, -150f), menu.BackToMain);

        levels.SetActive(false);
        credits.SetActive(false);
        SaveScene(scene, "MainMenu");
    }

    private static void CreateEndScene(string sceneName, string title, string subtitle)
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = sceneName;
        CreateCamera(new Vector3(0f, 1f, -10f), Quaternion.identity, Color.black);
        CreateGameManager();
        Canvas canvas = CreateCanvas(sceneName + "Canvas");
        GameObject panel = CreatePanel(canvas.transform, "Panel", new Color(0.015f, 0.015f, 0.018f, 0.95f));
        CreateTitle(panel.transform, title, 56, 90f);
        CreateText(panel.transform, subtitle, 26, new Vector2(0f, 20f), new Vector2(700f, 80f));
        EndScreenMenu endMenu = panel.AddComponent<EndScreenMenu>();
        CreateButton(panel.transform, "Jogar novamente", new Vector2(0f, -55f), endMenu.PlayAgain);
        CreateButton(panel.transform, "Menu Principal", new Vector2(0f, -115f), endMenu.MainMenu);
        CreateButton(panel.transform, "Sair", new Vector2(0f, -175f), endMenu.Quit);
        SaveScene(scene, sceneName);
    }

    private static void CreateLevelScenes()
    {
        LevelSpec[] specs =
        {
            new LevelSpec(1, "Fase 1 - Labirinto Abandonado", "Wall_Abandoned", 0.16f, 2, Maze1()),
            new LevelSpec(2, "Fase 2 - Labirinto Subterraneo", "Wall_Subterranean", 0.09f, 3, Maze2()),
            new LevelSpec(3, "Fase 3 - Labirinto Contaminado", "Wall_Contaminated", 0.065f, 4, Maze3()),
            new LevelSpec(4, "Fase 4 - Labirinto Final", "Wall_Final", 0.035f, 6, Maze4())
        };

        foreach (LevelSpec spec in specs)
        {
            CreateLevelScene(spec);
        }
    }

    private static void CreateLevelScene(LevelSpec spec)
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "Level" + spec.Number;
        CreateGameManager();

        GameObject manager = new GameObject("LevelManager");
        LevelManager levelManager = manager.AddComponent<LevelManager>();
        levelManager.levelNumber = spec.Number;
        levelManager.levelName = spec.Name;

        RenderSettings.skybox = null;
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(spec.Light, spec.Light, spec.Light);
        RenderSettings.reflectionIntensity = 0.05f;
        CreateLevelGeometry(spec);
        CreatePlayer(FindCell(spec.Map, 'S'));
        CreateExit(FindCell(spec.Map, 'E'));
        CreateZombies(spec);
        CreateHud();
        CreateBackgroundMusic();
        CreateZombieFootstepsAmbience();
        CreateLevelLights(spec);
        SaveScene(scene, "Level" + spec.Number);
    }

    private static void CreateBackgroundMusic()
    {
        AssetDatabase.ImportAsset(BackgroundMusicPath, ImportAssetOptions.ForceSynchronousImport);
        AudioClip music = AssetDatabase.LoadAssetAtPath<AudioClip>(BackgroundMusicPath);
        if (music == null)
        {
            return;
        }

        GameObject musicObject = new GameObject("BackgroundMusic");
        AudioSource source = musicObject.AddComponent<AudioSource>();
        source.clip = music;
        source.playOnAwake = true;
        source.loop = true;
        source.volume = 0.18f;
        source.spatialBlend = 0f;
    }

    private static void CreateZombieFootstepsAmbience()
    {
        AssetDatabase.ImportAsset(ZombieFootstepAudioPath, ImportAssetOptions.ForceSynchronousImport);
        AudioClip footsteps = AssetDatabase.LoadAssetAtPath<AudioClip>(ZombieFootstepAudioPath);
        if (footsteps == null)
        {
            return;
        }

        GameObject footstepsObject = new GameObject("ZombieFootstepsAmbience");
        AudioSource source = footstepsObject.AddComponent<AudioSource>();
        source.clip = footsteps;
        source.playOnAwake = true;
        source.loop = true;
        source.volume = 0.075f;
        source.pitch = 0.88f;
        source.spatialBlend = 0f;
    }

    private static void CreateLevelGeometry(LevelSpec spec)
    {
        int rows = spec.Map.Length;
        int cols = spec.Map[0].Length;
        Vector3 center = GridToWorld(cols / 2, rows / 2, rows);
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor";
        floor.transform.position = new Vector3(center.x, -0.08f, center.z);
        floor.transform.localScale = new Vector3(cols * CellSize, 0.15f, rows * CellSize);
        Renderer floorRenderer = floor.GetComponent<Renderer>();
        Material floorMaterial = new Material(LoadMat("Floor"));
        floorMaterial.name = "Floor_" + spec.Number;
        floorMaterial.mainTextureScale = new Vector2(cols * 0.5f, rows * 0.5f);
        floorRenderer.sharedMaterial = floorMaterial;

        GameObject ceiling = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ceiling.name = "Ceiling";
        ceiling.transform.position = new Vector3(center.x, WallHeight + 0.04f, center.z);
        ceiling.transform.localScale = new Vector3(cols * CellSize, 0.08f, rows * CellSize);
        Renderer ceilingRenderer = ceiling.GetComponent<Renderer>();
        Material ceilingMaterial = new Material(LoadMat("Ceiling"));
        ceilingMaterial.name = "Ceiling_" + spec.Number;
        ceilingMaterial.mainTextureScale = new Vector2(cols * 0.35f, rows * 0.35f);
        ceilingRenderer.sharedMaterial = ceilingMaterial;

        Material wallMat = LoadMat(spec.WallMaterial);
        for (int z = 0; z < rows; z++)
        {
            for (int x = 0; x < cols; x++)
            {
                char cell = spec.Map[z][x];
                Vector3 pos = GridToWorld(x, z, rows);
                if (cell == '#')
                {
                    GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    wall.name = "Wall";
                    wall.transform.position = new Vector3(pos.x, WallHeight / 2f, pos.z);
                    wall.transform.localScale = new Vector3(CellSize, WallHeight, CellSize);
                    Renderer wallRenderer = wall.GetComponent<Renderer>();
                    Material wallMaterial = new Material(wallMat);
                    wallMaterial.name = spec.WallMaterial + "_Tile";
                    wallMaterial.mainTextureScale = new Vector2(1f, 1.2f);
                    wallRenderer.sharedMaterial = wallMaterial;
                }
                else if (cell == 'C')
                {
                    GameObject contamination = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    contamination.name = "Contamination";
                    contamination.transform.position = new Vector3(pos.x, 0.02f, pos.z);
                    contamination.transform.localScale = new Vector3(1.4f, 0.03f, 1.4f);
                    contamination.GetComponent<Renderer>().sharedMaterial = LoadMat("Contamination");
                }
            }
        }
    }

    private static void CreatePlayer(Vector2Int start)
    {
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player";
        player.tag = "Player";
        player.transform.position = new Vector3(start.x * CellSize, 1.1f, -start.y * CellSize);
        player.GetComponent<Renderer>().sharedMaterial = LoadMat("Player");
        Object.DestroyImmediate(player.GetComponent<CapsuleCollider>());
        CharacterController controller = player.AddComponent<CharacterController>();
        controller.height = 2f;
        controller.radius = 0.45f;
        player.AddComponent<FirstPersonController>();

        GameObject cameraObject = new GameObject("FirstPersonCamera");
        cameraObject.transform.SetParent(player.transform);
        cameraObject.transform.localPosition = new Vector3(0f, 0.72f, 0f);
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.fieldOfView = 72f;
        cameraObject.AddComponent<AudioListener>();
        MouseLook look = cameraObject.AddComponent<MouseLook>();
        look.playerBody = player.transform;

        GameObject lightObject = new GameObject("Flashlight");
        lightObject.transform.SetParent(cameraObject.transform);
        lightObject.transform.localPosition = new Vector3(0.25f, -0.1f, 0.25f);
        lightObject.transform.localRotation = Quaternion.identity;
        Light light = lightObject.AddComponent<Light>();
        light.type = LightType.Spot;
        light.range = 19f;
        light.spotAngle = 46f;
        light.intensity = 5.4f;
        light.shadows = LightShadows.Soft;
        FlashlightController flashlight = cameraObject.AddComponent<FlashlightController>();
        flashlight.flashlight = light;
    }

    private static void CreateExit(Vector2Int exitCell)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/ExitDoor.prefab");
        GameObject exit = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        exit.name = "ExitDoor";
        exit.transform.position = new Vector3(exitCell.x * CellSize, 0.05f, -exitCell.y * CellSize);
    }

    private static void CreateZombies(LevelSpec spec)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Zombie.prefab");
        List<Vector2Int> open = FindOpenCells(spec.Map);
        for (int i = 0; i < spec.ZombieCount; i++)
        {
            Vector2Int cell = open[Mathf.Clamp((i + 2) * open.Count / (spec.ZombieCount + 3), 0, open.Count - 1)];
            GameObject zombie = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            zombie.name = "Zombie_" + (i + 1);
            zombie.transform.position = new Vector3(cell.x * CellSize, 0.05f, -cell.y * CellSize);

            ZombieAI ai = zombie.GetComponent<ZombieAI>();
            ai.patrolSpeed = 1.1f + spec.Number * 0.25f;
            ai.chaseSpeed = 2.2f + spec.Number * 0.35f;
            ai.detectionDistance = 6f + spec.Number * 1.5f;
            ai.patrolPoints = CreatePatrolPoints(spec.Map, cell, i);
        }
    }

    private static Transform[] CreatePatrolPoints(string[] map, Vector2Int origin, int seed)
    {
        List<Vector2Int> open = FindOpenCells(map);
        Transform[] points = new Transform[3];
        for (int i = 0; i < points.Length; i++)
        {
            Vector2Int cell = open[Mathf.Abs((origin.x * 17 + origin.y * 31 + seed * 19 + i * 11) % open.Count)];
            GameObject point = new GameObject("PatrolPoint");
            point.transform.position = new Vector3(cell.x * CellSize, 0.05f, -cell.y * CellSize);
            points[i] = point.transform;
        }
        return points;
    }

    private static void CreateHud()
    {
        Canvas canvas = CreateCanvas("HUDCanvas");
        UIManager ui = canvas.gameObject.AddComponent<UIManager>();
        Text level = CreateText(canvas.transform, "", 24, new Vector2(18f, -18f), new Vector2(540f, 40f), TextAnchor.UpperLeft);
        Text objective = CreateText(canvas.transform, "", 18, new Vector2(18f, -52f), new Vector2(680f, 34f), TextAnchor.UpperLeft);
        ui.levelNameText = level;
        ui.objectiveText = objective;

        GameObject pause = CreatePanel(canvas.transform, "PausePanel", new Color(0f, 0f, 0f, 0.78f));
        CreateTitle(pause.transform, "PAUSA", 42, 80f);
        CreateButton(pause.transform, "Continuar", new Vector2(0f, 10f), ui.Resume);
        CreateButton(pause.transform, "Reiniciar Fase", new Vector2(0f, -50f), ui.RestartLevel);
        CreateButton(pause.transform, "Menu Principal", new Vector2(0f, -110f), ui.MainMenu);
        pause.SetActive(false);
        ui.pausePanel = pause;
    }

    private static void CreateLevelLights(LevelSpec spec)
    {
        Light sun = new GameObject("Dim Directional Light").AddComponent<Light>();
        sun.type = LightType.Directional;
        sun.intensity = spec.Light * 0.25f;
        sun.shadows = LightShadows.Soft;
        sun.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
    }

    private static Canvas CreateCanvas(string name)
    {
        GameObject canvasObject = new GameObject(name);
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280f, 720f);
        canvasObject.AddComponent<GraphicRaycaster>();
        if (Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));
        }
        return canvas;
    }

    private static GameObject CreatePanel(Transform parent, string name, Color color)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        Image image = panel.AddComponent<Image>();
        image.color = color;
        return panel;
    }

    private static void CreateTitle(Transform parent, string text, int size, float y)
    {
        CreateText(parent, text, size, new Vector2(0f, y), new Vector2(780f, 80f));
    }

    private static Text CreateText(Transform parent, string value, int size, Vector2 anchoredPosition, Vector2 rectSize, TextAnchor anchor = TextAnchor.MiddleCenter)
    {
        GameObject textObject = new GameObject("Text");
        textObject.transform.SetParent(parent, false);
        RectTransform rect = textObject.AddComponent<RectTransform>();
        rect.sizeDelta = rectSize;
        rect.anchoredPosition = anchoredPosition;
        if (anchor == TextAnchor.UpperLeft)
        {
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
        }
        Text text = textObject.AddComponent<Text>();
        text.text = value;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = size;
        text.alignment = anchor;
        text.color = Color.white;
        return text;
    }

    private static Button CreateButton(Transform parent, string label, Vector2 pos, UnityEngine.Events.UnityAction action)
    {
        GameObject buttonObject = new GameObject(label);
        buttonObject.transform.SetParent(parent, false);
        RectTransform rect = buttonObject.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(260f, 46f);
        rect.anchoredPosition = pos;
        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.13f, 0.17f, 0.18f, 0.95f);
        Button button = buttonObject.AddComponent<Button>();
        UnityEventTools.AddPersistentListener(button.onClick, action);
        CreateText(buttonObject.transform, label, 20, Vector2.zero, rect.sizeDelta);
        return button;
    }

    private static Button CreateButton(Transform parent, string label, Vector2 pos, UnityEngine.Events.UnityAction<int> action, int value)
    {
        GameObject buttonObject = new GameObject(label);
        buttonObject.transform.SetParent(parent, false);
        RectTransform rect = buttonObject.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(260f, 46f);
        rect.anchoredPosition = pos;
        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.13f, 0.17f, 0.18f, 0.95f);
        Button button = buttonObject.AddComponent<Button>();
        UnityEventTools.AddIntPersistentListener(button.onClick, action, value);
        CreateText(buttonObject.transform, label, 20, Vector2.zero, rect.sizeDelta);
        return button;
    }

    private static void CreateCamera(Vector3 pos, Quaternion rot, Color background)
    {
        GameObject cameraObject = new GameObject("Camera");
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = background;
        cameraObject.transform.SetPositionAndRotation(pos, rot);
        cameraObject.AddComponent<AudioListener>();
    }

    private static void CreateGameManager()
    {
        new GameObject("GameManager").AddComponent<GameManager>();
    }

    private static Vector3 GridToWorld(int x, int z, int rows)
    {
        return new Vector3(x * CellSize, 0f, -z * CellSize);
    }

    private static Vector2Int FindCell(string[] map, char marker)
    {
        for (int z = 0; z < map.Length; z++)
        {
            int x = map[z].IndexOf(marker);
            if (x >= 0) return new Vector2Int(x, z);
        }
        return Vector2Int.one;
    }

    private static List<Vector2Int> FindOpenCells(string[] map)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        for (int z = 0; z < map.Length; z++)
        {
            for (int x = 0; x < map[z].Length; x++)
            {
                if (map[z][x] != '#')
                {
                    result.Add(new Vector2Int(x, z));
                }
            }
        }
        return result;
    }

    private static Material LoadMat(string name)
    {
        return AssetDatabase.LoadAssetAtPath<Material>($"Assets/Materials/{name}.mat");
    }

    private static void SaveScene(Scene scene, string name)
    {
        EditorSceneManager.SaveScene(scene, $"Assets/Scenes/{name}.unity");
    }

    private static void ConfigureBuildSettings()
    {
        string[] names = { "MainMenu", "Level1", "Level2", "Level3", "Level4", "GameOver", "Victory" };
        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>();
        foreach (string name in names)
        {
            scenes.Add(new EditorBuildSettingsScene($"Assets/Scenes/{name}.unity", true));
        }
        EditorBuildSettings.scenes = scenes.ToArray();
    }

    private static string[] Maze1() => new[]
    {
        "#########",
        "#S..#...#",
        "#.#.#.#E#",
        "#.#...#.#",
        "#.#####.#",
        "#.......#",
        "#########"
    };

    private static string[] Maze2() => new[]
    {
        "#############",
        "#S....#.....#",
        "###.#.#.###.#",
        "#...#...#...#",
        "#.#####.#.#.#",
        "#.....#.#.#E#",
        "#.###.#.#.###",
        "#...#.......#",
        "#############"
    };

    private static string[] Maze3() => new[]
    {
        "###############",
        "#S#...#C......#",
        "#.#.#.#.#####.#",
        "#...#.#.....#.#",
        "###.#.#####.#.#",
        "#...#...C.#.#.#",
        "#.#####.#.#.#.#",
        "#.....#.#...#E#",
        "###############"
    };

    private static string[] Maze4() => new[]
    {
        "#################",
        "#S....#.........#",
        "###.#.#.#######.#",
        "#...#.#.....#...#",
        "#.###.#####.#.###",
        "#.#...#...#.#...#",
        "#.#.###.#.#.###.#",
        "#...#...#.#.....#",
        "#.###.###.#####.#",
        "#.....#.......#E#",
        "#################"
    };

    private readonly struct LevelSpec
    {
        public readonly int Number;
        public readonly string Name;
        public readonly string WallMaterial;
        public readonly float Light;
        public readonly int ZombieCount;
        public readonly string[] Map;

        public LevelSpec(int number, string name, string wallMaterial, float light, int zombieCount, string[] map)
        {
            Number = number;
            Name = name;
            WallMaterial = wallMaterial;
            Light = light;
            ZombieCount = zombieCount;
            Map = map;
        }
    }
}
