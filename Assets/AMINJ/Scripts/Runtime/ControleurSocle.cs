// ControleurSocle.cs : le point unique ou se branche la suite.
//
// Aujourd'hui : selectionner un batiment affiche son nom, ecrit une trace, et (si coche)
// demande a l'agent une phrase de presentation. C'est une DEMONSTRATION de la chaine,
// pas une situation d'apprentissage.
//
// A construire par le groupe (docs/A_CONSTRUIRE.md) : les missions et la progression,
// la fonction didactique de l'agent, son incarnation et sa voix, le debriefing.
using TMPro;
using UnityEngine;

namespace Aminj.Campus
{
    public class ControleurSocle : MonoBehaviour
    {
        public static ControleurSocle Instance { get; private set; }

        [Header("Briques")]
        public AgentRelais agent;
        public TraceXapi trace;
        [Tooltip("Etiquette au-dessus de la maquette.")]
        public TextMeshPro etiquette;

        [Header("Demonstration")]
        [Tooltip("Decocher pour que l'agent se taise : c'est au scenario de decider quand il intervient.")]
        public bool demanderAgentALaSelection = true;

        private string _selection;

        private void Awake() => Instance = this;

        public void Selectionner(BatimentCible cible)
        {
            _selection = cible.semanticId;
            CampusDonnees.Batiments.TryGetValue(_selection, out var b);
            var nom = b != null ? b.nom : _selection;
            if (etiquette) etiquette.text = nom;
            if (trace) trace.Tracer("a_selectionne", _selection);

            // TODO groupe : remplacer cette demonstration par la logique de mission.
            if (demanderAgentALaSelection && agent)
            {
                var id = _selection;
                agent.Demander("Presente ce batiment en une phrase a un nouvel etudiant.", CampusDonnees.Resume(b), reponse =>
                {
                    if (etiquette && id == _selection) etiquette.text = nom + "\n<size=60%>" + reponse + "</size>";
                });
            }
        }
    }
}
