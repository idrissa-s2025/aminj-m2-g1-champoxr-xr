// MonterSocle.cs : construit la scene du socle a partir du modele et des donnees.
//
// Menu AMINJ > 1. Monter la scene du socle : rien n'est pose a la main. Le script
//   - pose le rig XR (mains et manettes) de l'exemple XR Interaction Toolkit,
//   - active le passthrough (AR Session et AR Camera Manager, fond transparent),
//   - pose un socle a hauteur de table et la maquette du campus au 1/400e,
//   - rend chaque batiment connu du JSON pointable et selectionnable,
//   - ajoute le controleur, l'agent (relais) et les traces xAPI,
//   - enregistre Assets/Scenes/Socle_Campus.unity et l'ajoute au build.
// Menu AMINJ > 2. Verifier le socle : compte ce qui a ete monte.
using System.IO;
using System.Linq;
using Aminj.Campus;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

namespace Aminj.Campus.Editeur
{
    public static class MonterSocle
    {
        private const string Modele = "Assets/CampusXR/Modele/campus_holo.fbx";
        private const string Scene = "Assets/Scenes/Socle_Campus.unity";
        private const float Echelle = 1f / 400f;
        private const float HauteurTable = 0.8f;
        private const float DistanceTable = 0.7f;

        [MenuItem("AMINJ/1. Monter la scene du socle")]
        public static void Monter()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Lumiere
            var soleil = new GameObject("Soleil").AddComponent<Light>();
            soleil.type = LightType.Directional;
            soleil.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            // Rig XR : mains et manettes, issu de l'exemple Hands Interaction Demo.
            var rig = ChercherPrefab("XR Origin Hands (XR Rig)") ?? ChercherPrefab("XR Origin (XR Rig)");
            if (rig == null) { Debug.LogError("[AMINJ] Rig XR introuvable : importer les exemples XRI (voir README)."); return; }
            var origine = (GameObject)PrefabUtility.InstantiatePrefab(rig);

            // Passthrough : une AR Session et une camera au fond transparent.
            new GameObject("AR Session").AddComponent<ARSession>();
            var camera = origine.GetComponentInChildren<Camera>();
            if (camera)
            {
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = new Color(0f, 0f, 0f, 0f);
                if (!camera.GetComponent<ARCameraManager>()) camera.gameObject.AddComponent<ARCameraManager>();
            }

            // Socle a hauteur de table
            var socle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            socle.name = "Socle";
            socle.transform.position = new Vector3(0f, HauteurTable - 0.01f, DistanceTable);
            socle.transform.localScale = new Vector3(0.9f, 0.01f, 0.9f);

            // Maquette au 1/400e, centree sur le socle
            var source = AssetDatabase.LoadAssetAtPath<GameObject>(Modele);
            if (source == null) { Debug.LogError("[AMINJ] Modele introuvable : " + Modele); return; }
            var pivot = new GameObject("Maquette").transform;
            pivot.position = new Vector3(0f, HauteurTable, DistanceTable);
            var campus = (GameObject)PrefabUtility.InstantiatePrefab(source, pivot);
            campus.transform.localScale = Vector3.one * Echelle;
            var bornes = Bornes(campus);
            campus.transform.position += pivot.position - new Vector3(bornes.center.x, bornes.min.y, bornes.center.z);

            // Batiments pointables : un objet dont le nom est un semantic_id connu
            int batiments = 0;
            foreach (var t in campus.GetComponentsInChildren<Transform>(true))
            {
                if (!CampusDonnees.Batiments.ContainsKey(t.name) || t.GetComponentInParent<BatimentCible>()) continue;
                foreach (var mf in t.GetComponentsInChildren<MeshFilter>(true))
                    if (!mf.GetComponent<Collider>()) mf.gameObject.AddComponent<MeshCollider>();
                t.gameObject.AddComponent<BatimentCible>().semanticId = t.name;
                batiments++;
            }

            // Controleur, agent, traces, etiquette
            var controle = new GameObject("Controleur Socle");
            var c = controle.AddComponent<ControleurSocle>();
            c.agent = controle.AddComponent<AgentRelais>();
            c.trace = controle.AddComponent<TraceXapi>();
            var etiquette = new GameObject("Etiquette").AddComponent<TextMeshPro>();
            etiquette.transform.position = pivot.position + new Vector3(0f, 0.35f, 0f);
            etiquette.fontSize = 0.6f;
            etiquette.alignment = TextAlignmentOptions.Center;
            etiquette.rectTransform.sizeDelta = new Vector2(0.8f, 0.3f);
            etiquette.text = "Pointer un batiment";
            c.etiquette = etiquette;

            // Requetes HTTP vers le relais du reseau local
            PlayerSettings.insecureHttpOption = InsecureHttpOption.AlwaysAllowed;

            Directory.CreateDirectory(Path.GetDirectoryName(Scene));
            EditorSceneManager.SaveScene(scene, Scene);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(Scene, true) };
            Debug.Log($"[AMINJ] Scene montee : {batiments} batiments selectionnables, enregistree dans {Scene}");
        }

        [MenuItem("AMINJ/2. Verifier le socle")]
        public static bool Verifier()
        {
            var cibles = Object.FindObjectsByType<BatimentCible>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            bool ok = cibles.Length > 0 && Object.FindFirstObjectByType<ControleurSocle>() && Object.FindFirstObjectByType<ARSession>();
            Debug.Log($"[AMINJ] Verification : {cibles.Length} batiments, donnees {CampusDonnees.Batiments.Count} batiments, " +
                      (ok ? "socle complet." : "socle incomplet."));
            return ok;
        }

        // Pour la ligne de commande : Unity -batchmode -executeMethod Aminj.Campus.Editeur.MonterSocle.EnLot
        public static void EnLot()
        {
            Monter();
            EditorApplication.Exit(Verifier() ? 0 : 1);
        }

        private static GameObject ChercherPrefab(string nom)
        {
            return AssetDatabase.FindAssets("t:Prefab Origin")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(p => Path.GetFileNameWithoutExtension(p) == nom)
                .Select(AssetDatabase.LoadAssetAtPath<GameObject>)
                .FirstOrDefault();
        }

        private static Bounds Bornes(GameObject racine)
        {
            var rendus = racine.GetComponentsInChildren<Renderer>();
            if (rendus.Length == 0) return new Bounds(racine.transform.position, Vector3.zero);
            var b = rendus[0].bounds;
            foreach (var r in rendus.Skip(1)) b.Encapsulate(r.bounds);
            return b;
        }
    }
}
