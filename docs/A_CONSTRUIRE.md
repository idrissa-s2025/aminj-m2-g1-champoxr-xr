# Ce que le groupe P1 conçoit

Le socle montre une chaîne technique : pointer un bâtiment, l'identifier, en parler. Il ne
fait rien apprendre. Les sept éléments ci-dessous sont ceux du dossier du module ; chacun
indique l'endroit du code où il se branche.

## 1. Objectif d'apprentissage

Qu'est-ce qui est appris à travers le dispositif, et comment le joueur prend-il conscience
de l'évolution de son apprentissage ? Formuler avec un verbe observable : « à la fin, le
nouvel étudiant sait situer… », « sait se rendre de… à… ».

## 2. Missions et progression

Transformer la sélection libre en tâches. Exemples à discuter, pas à recopier : retrouver le
bâtiment d'un cours à partir de son code de salle (`MM013`), relier un service à son étage,
reconstituer un trajet. Point d'entrée : `ControleurSocle.Selectionner()`.

## 3. Fonction didactique de l'agent

Guider, questionner, constater, démontrer, débriefer : choisir et justifier. Elle s'écrit dans
`relais/consignes_agent.md` (message système) et dans le moment où le code appelle
`AgentRelais.Demander()`. Décocher *demanderAgentALaSelection* : c'est au scénario de décider
quand l'agent intervient.

## 4. Incarnation et voix

Personnage, voix seule ou texte : un choix argumenté (effet persona, charge cognitive). Le
relais fournit `POST /api/session-vocale`, un jeton éphémère pour l'API Realtime ; la
chaîne vocale reste à construire.

## 5. Traces

Choisir les verbes AVANT de développer : `a_selectionne` est fourni, à compléter
(`a_demande_aide`, `a_reussi_mission`…). Elles sont écrites dans `relais/journal/`.

## 6. Évaluation

Ce qui prouve l'apprentissage : épreuve avant, après et différée, questionnaires (SUS, MPS,
SSQ, ASAQ), lecture des traces.

## 7. Débriefing

Ce que l'apprenant voit de son parcours à la fin, à partir des traces.

## Coordination avec P2

Même maquette, même `campus_semantique.json`. Toute correction de données se fait dans le
fichier commun et se signale à l'autre équipe.
