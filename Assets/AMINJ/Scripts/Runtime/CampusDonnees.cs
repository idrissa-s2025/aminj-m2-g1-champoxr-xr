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
    [Serializable] public class Fiche { public string semantic_id; public string titre; public string role; public string histoire;
        public string[] pedagogie; public string[] services; public string[] reperes; }
    [Serializable] public class Campus { public string nom; public string role; public string histoire; public string[] reperes; }

    public static class CampusDonnees
    {
        [Serializable] private class Racine { public Batiment[] batiments; }

        [Serializable] private class RacineFiches { public Campus campus; public Fiche[] fiches; }

        private static Dictionary<string, Batiment> _batiments;
        private static Dictionary<string, Fiche> _fiches;
        private static Campus _campus;

        /// <summary>Les connaissances de l'agent, lues depuis Resources/fiches_batiments.json.</summary>
        public static IReadOnlyDictionary<string, Fiche> Fiches
        {
            get
            {
                if (_fiches == null)
                {
                    var texte = Resources.Load<TextAsset>("fiches_batiments");
                    var racine = texte ? JsonUtility.FromJson<RacineFiches>(texte.text) : null;
                    _campus = racine?.campus;
                    _fiches = (racine?.fiches ?? Array.Empty<Fiche>())
                        .Where(f => !string.IsNullOrEmpty(f.semantic_id))
                        .GroupBy(f => f.semantic_id).ToDictionary(g => g.Key, g => g.First());
                }
                return _fiches;
            }
        }

        public static Campus LeCampus { get { _ = Fiches; return _campus; } }

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

        /// <summary>Le contexte transmis a l'agent : le campus, le batiment, ses locaux et sa fiche.</summary>
        public static string Resume(Batiment b)
        {
            var lignes = new List<string>();
            var c = LeCampus;
            if (c != null) lignes.Add($"Campus : {c.nom}. {c.role}");
            if (b == null) return string.Join("\n", lignes);
            lignes.Add($"Batiment : {b.nom} (numero {b.plan_num} sur le plan)");
            foreach (var n in b.niveaux ?? Array.Empty<Niveau>())
                lignes.Add($"{n.etage} : {string.Join(", ", (n.locaux ?? Array.Empty<Local>()).Select(l => l.nom))}");
            if (Fiches.TryGetValue(b.semantic_id, out var f))
            {
                lignes.Add($"Role : {f.role}");
                lignes.Add($"Histoire : {f.histoire}");
                if (f.pedagogie?.Length > 0) lignes.Add("Pedagogie : " + string.Join(" ; ", f.pedagogie));
                if (f.services?.Length > 0) lignes.Add("Services : " + string.Join(" ; ", f.services));
                if (f.reperes?.Length > 0) lignes.Add("Reperes : " + string.Join(" ; ", f.reperes));
            }
            return string.Join("\n", lignes);
        }
    }
}
