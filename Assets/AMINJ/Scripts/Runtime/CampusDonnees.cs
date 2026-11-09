// CampusDonnees.cs : la semantique du campus, lue depuis Resources/campus_semantique.json.
//
// Regle du projet : le modele 3D ne porte qu'un semantic_id par objet ; tout le reste
// (nom, plan, etages, locaux) vit dans ce JSON, partage avec l'equipe web.
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aminj.Campus
{
    [Serializable] public class Local { public string id; public string nom; public string type; public string code; public string plage; }
    [Serializable] public class Niveau { public string etage; public Local[] locaux; }
    [Serializable] public class Batiment { public string semantic_id; public string plan_num; public string nom; public string nom_usuel; public Niveau[] niveaux; }

    public static class CampusDonnees
    {
        [Serializable] private class Racine { public Batiment[] batiments; }

        private static Dictionary<string, Batiment> _batiments;

        public static IReadOnlyDictionary<string, Batiment> Batiments
        {
            get
            {
                if (_batiments == null)
                {
                    var texte = Resources.Load<TextAsset>("campus_semantique");
                    var racine = texte ? JsonUtility.FromJson<Racine>(texte.text) : null;
                    _batiments = (racine?.batiments ?? Array.Empty<Batiment>())
                        .Where(b => !string.IsNullOrEmpty(b.semantic_id))
                        .GroupBy(b => b.semantic_id).ToDictionary(g => g.Key, g => g.First());
                }
                return _batiments;
            }
        }

        /// <summary>Un resume court, transmis a l'agent comme contexte.</summary>
        public static string Resume(Batiment b)
        {
            if (b == null) return "";
            var niveaux = (b.niveaux ?? Array.Empty<Niveau>())
                .Select(n => $"{n.etage} : {string.Join(", ", (n.locaux ?? Array.Empty<Local>()).Select(l => l.nom))}");
            return $"Batiment : {b.nom} (n° {b.plan_num} sur le plan)\n" + string.Join("\n", niveaux);
        }
    }
}
