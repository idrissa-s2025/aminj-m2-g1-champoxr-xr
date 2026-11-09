# ChampoXR · équipe réalité mixte (G1)

Socle de projet du Master 2 AMINJ, INU Champollion, 2026-2027.
Module « Développer une ingénierie pédagogique pour les serious games ».

Ce dépôt n'est pas le projet ChampoXR complet. C'est une **base de départ ciblée** : la
maquette du campus, ses données, l'interaction de base et une brique d'agent OpenAI propre.
Tout ce qui fait d'une maquette une **formation** reste à concevoir par le groupe.

## Ce que contient le socle

| Élément | Où | État |
|---|---|---|
| Maquette du campus en hologramme, au 1/400e | `Assets/CampusXR/Modele/campus_holo.fbx` | fourni |
| Sources Blender (hologramme et maquette texturée, v05) | `blender/` | fourni |
| Sémantique du campus : bâtiments, étages, locaux | `Assets/CampusXR/Resources/campus_semantique.json` | fourni, partagé avec G2 |
| Fiches des bâtiments (rôle, histoire, pédagogie, services) | `Assets/CampusXR/Resources/fiches_batiments.json` | fourni, transmis à l'agent, partagé avec G2 |
| Rig XR mains et manettes, passthrough | exemples XR Interaction Toolkit, menu AMINJ | fourni |
| Pointer, survoler et sélectionner un bâtiment | `Assets/AMINJ/Scripts/Runtime/BatimentCible.cs` | fourni |
| Agent par le relais (texte) | `AgentRelais.cs` et `relais/` | fourni, fonction didactique à écrire |
| Traces xAPI simplifiées | `TraceXapi.cs`, écrites par le relais | fourni, verbes à choisir |
| Sons d'interface | `Assets/CampusXR/Audio/` | fourni, non branché |

## Ce qui n'y est pas, volontairement

Le guide incarné et sa voix, le guidage vers un lieu, les bulles d'information, les missions,
la progression, l'évaluation et le débriefing. C'est le travail du groupe : voir
[`docs/A_CONSTRUIRE.md`](docs/A_CONSTRUIRE.md).

## Démarrer

1. **Unity 6000.6.0f1** avec le module *Android Build Support*. Ouvrir le dossier du dépôt
   depuis Unity Hub. Le premier import prend une dizaine de minutes.
   Cloner le dépôt dans un chemin court (par exemple `C:\aminj\g1`) : Windows refuse les
   chemins trop longs dans `Library/`.
2. Ouvrir `Assets/Scenes/Socle_Campus.unity` : rig XR, passthrough, socle à hauteur de table,
   maquette, 23 bâtiments sélectionnables. Le menu **AMINJ > 1. Monter la scène du socle** la
   reconstruit à partir du modèle et des données ; **AMINJ > 2** la vérifie.
3. **Relais** (Node 18 ou plus) : copier `relais/env.exemple` en `relais/.env`, y coller la clé
   fournie par l'enseignant, puis lancer `node relais/serveur.mjs`. Vérifier
   `http://localhost:8787/sante`.
4. Dans la scène, sur l'objet *Controleur Socle* : renseigner l'adresse du relais dans
   *AgentRelais* et *TraceXapi*. Dans l'éditeur : `http://localhost:8787`. Dans le casque :
   l'adresse IP du PC, sur le même réseau Wi-Fi.
5. *File > Build Profiles* : Android, Meta Quest 3, puis *Build and Run*.

## L'agent IA : où tout se trouve

L'agent passe toujours par le relais : le client (casque ou navigateur) envoie une question et
un contexte, le relais ajoute la clé et les consignes, puis appelle OpenAI.

| Ce qui | Fichier | Ce que le groupe y fait |
|---|---|---|
| Appel à OpenAI, clé, modèle, plafonds | `relais/serveur.mjs` | rien en principe ; modèle et plafonds se règlent dans `relais/.env` |
| Clé et réglages (`OPENAI_API_KEY`, `OPENAI_MODEL`, `MAX_APPELS_JOUR`) | `relais/.env`, copié de `relais/env.exemple` | coller la clé ; jamais dans Git |
| **Instruction** de l'agent (message système) | `relais/consignes_agent.md` | **à réécrire** : rôle, ce qu'il dit, ce qu'il ne dit jamais, forme |
| Côté Unity : envoi de la question et réception de la réponse | `Assets/AMINJ/Scripts/Runtime/AgentRelais.cs` | brancher la voix, l'affichage |
| Quand l'agent parle, et avec quelle demande | `Assets/AMINJ/Scripts/Runtime/ControleurSocle.cs` (`Selectionner()`) | décider du moment et de la demande |
| **Connaissances** : bâtiment, étages, locaux | `Assets/CampusXR/Resources/campus_semantique.json` | enrichir ou filtrer |
| **Connaissances** : fiche du bâtiment et présentation du campus | `Assets/CampusXR/Resources/fiches_batiments.json` | vérifier, compléter |
| Assemblage du contexte envoyé à l'agent | `CampusDonnees.Resume()` dans `Assets/AMINJ/Scripts/Runtime/CampusDonnees.cs` | choisir ce que l'agent reçoit |
| Traces xAPI envoyées au relais | `Assets/AMINJ/Scripts/Runtime/TraceXapi.cs` | choisir les verbes |
| Journal des échanges et des traces | `relais/journal/*.jsonl` | lire pour l'évaluation et le débriefing |

Le relais relit `consignes_agent.md` à chaque question : une modification s'applique sans
redémarrer. Les connaissances ne sont pas « apprises » par le modèle : elles lui sont
transmises à chaque échange dans le contexte. Ce qui n'est pas dans le contexte, l'agent doit
dire qu'il ne le sait pas.

## Règles

- **La clé OpenAI ne quitte jamais le relais.** Jamais dans Unity, un APK, Git ou une capture.
  `relais/.env` est ignoré par Git.
- Aucune donnée personnelle réelle : l'acteur des traces est un identifiant anonyme.
- Un plafond d'appels par jour est réglé dans `relais/.env` (`MAX_APPELS_JOUR`).
- L'agent annonce qu'il est une IA (AI Act, article 50).

## Structure

```
Assets/AMINJ/Scripts/Runtime   CampusDonnees, BatimentCible, ControleurSocle, AgentRelais, TraceXapi
Assets/AMINJ/Scripts/Editor    MonterSocle (menu AMINJ)
Assets/CampusXR                modèle, données, sons
relais/                        serveur.mjs, consignes_agent.md, env.exemple
blender/                       sources de la maquette
docs/A_CONSTRUIRE.md           ce que le groupe conçoit
```

Le point d'entrée du travail est `ControleurSocle.Selectionner()` : c'est là que la
sélection d'un bâtiment devient, ou non, une étape de mission.
