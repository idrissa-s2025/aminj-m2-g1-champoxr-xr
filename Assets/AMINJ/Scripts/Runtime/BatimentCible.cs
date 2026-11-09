// BatimentCible.cs : un batiment de la maquette que l'on peut pointer et selectionner.
//
// Pose par le menu AMINJ > 1, sur chaque objet dont le nom est un semantic_id connu.
// Il ne fait que signaler : c'est le ControleurSocle qui decide de la suite.
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Aminj.Campus
{
    [RequireComponent(typeof(XRSimpleInteractable))]
    public class BatimentCible : MonoBehaviour
    {
        public string semanticId;

        private XRSimpleInteractable _interactable;
        private Vector3 _position;

        private void Awake()
        {
            _interactable = GetComponent<XRSimpleInteractable>();
            _position = transform.localPosition;
        }

        private void OnEnable()
        {
            _interactable.hoverEntered.AddListener(Survol);
            _interactable.hoverExited.AddListener(FinSurvol);
            _interactable.selectEntered.AddListener(Selection);
        }

        private void OnDisable()
        {
            _interactable.hoverEntered.RemoveListener(Survol);
            _interactable.hoverExited.RemoveListener(FinSurvol);
            _interactable.selectEntered.RemoveListener(Selection);
        }

        // Le batiment se souleve un peu au survol : un retour visible, sans texte.
        private void Survol(HoverEnterEventArgs _) => transform.localPosition = _position + Vector3.up * 2f;
        private void FinSurvol(HoverExitEventArgs _) => transform.localPosition = _position;

        private void Selection(SelectEnterEventArgs _)
        {
            if (ControleurSocle.Instance) ControleurSocle.Instance.Selectionner(this);
        }
    }
}
