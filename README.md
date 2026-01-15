📊 DataFlow - Moniteur de Migration de Données
DataFlow est une solution full-stack conteneurisée permettant d'importer, de traiter et de visualiser des flux de migration de données à partir de fichiers CSV. L'application utilise une architecture en microservices pour garantir une séparation nette entre le traitement des fichiers, la logique métier et l'interface utilisateur.

🏗️ Architecture Technique
L'application est décomposée en trois services principaux communiquant via des APIs REST :

Frontend (Vue.js 3) : Interface utilisateur réactive avec gestion du streaming d'importation et dashboard de visualisation paginé.

Backend Traitement (Python/Flask) : Orchestrateur chargé du parsing des fichiers CSV et du streaming des données vers l'interface de persistance.

Interface BDD (C# .NET 9) : Service robuste gérant les règles métiers, la validation des données et la communication avec la base de données SQL.

🚀 Installation et Lancement
Le projet est entièrement "Dockerisé". Une seule commande suffit pour monter l'infrastructure complète (3 containers + Base de données).

Prérequis
Docker et Docker Compose installés sur votre machine.

Démarrage
Clonez le dépôt.

À la racine du projet, lancez :

Bash

docker compose up -d --build
Accédez à l'application via votre navigateur à l'adresse : http://localhost:8080 (ou le port configuré dans votre Nginx).

🛠️ Fonctionnalités Principales
1. Importation Intelligente (Stream)
L'importation ne charge pas le fichier entier en mémoire. Les données sont lues par morceaux (chunks), envoyées au backend Python qui les transforme en batchs pour l'API C#.

Feedback en temps réel : Une barre de progression affiche l'état d'avancement du traitement SQL.

2. Visualisation et Analyse
Un dashboard permet de consulter les migrations stockées avec :

Recherche multicritère (Titre, SubJob ID, Statut).

Pagination performante côté serveur (LIMIT/OFFSET) pour supporter de gros volumes de données.

📈 Flux de Données (Niveau 1)
Voici comment circulent les informations lors d'une recherche :

Plaintext

User -> Python API : GET /api/files?page=1&query=abc
Python API -> C# Service : Forward Request
C# Service -> SQL DB : SELECT with LIMIT/OFFSET
SQL DB -> C# Service : Data Result
C# Service -> Python API : JSON Response
Python API -> User : Display in Table
🛠️ Stack Technologique
Frontend : Vue.js 3 (Composition API), CSS Moderno.

Processing : Python 3.11+, Flask/Gunicorn.

Business Logic : .NET 9 Core, Entity Framework.

Infrastructure : Docker, Docker Compose, Nginx.
