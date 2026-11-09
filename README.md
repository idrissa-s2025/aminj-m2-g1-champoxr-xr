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
| Fiches des bâtiments | `Assets/CampusXR/Resources/fiches_batiments.json` | fourni, non branché |
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
