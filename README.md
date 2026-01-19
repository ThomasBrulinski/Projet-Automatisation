**IGOUFE Valentin**
**BRULINSKI Thomas**
**Groupe 1**

**📊 DataFlow - Moniteur de Migration de Données**

DataFlow est une solution full-stack conteneurisée permettant d'importer, de traiter et de visualiser des flux de migration de données à partir de fichiers CSV. L'application utilise une architecture en microservices pour garantir une séparation nette entre le traitement des fichiers, la logique métier et l'interface utilisateur.

**🛠️ Fonctionnalités Principales**

*1. Importation Intelligente (Stream)*

L'importation ne charge pas le fichier entier en mémoire. Les données sont lues par morceaux (chunks), envoyées au backend Python qui les transforme en batchs pour l'API C#.

Feedback en temps réel : Une barre de progression affiche l'état d'avancement du traitement SQL.

*2. Visualisation et Analyse*

Un dashboard permet de consulter les migrations stockées avec :

Recherche sur la Source.

Pagination performante côté serveur (LIMIT/OFFSET) pour supporter de gros volumes de données.

**🛠️ Stack Technologique**

Proxy/Web Server : Nginx (Reverse Proxy & Static Hosting).

Frontend : Vue.js 3.

Processing : Python 3.11+ (Flask).

Business Logic : .NET 9 Core (Entity Framework).

Infrastructure : Docker Compose (4 services).

**🏗️ Architecture Technique (4 Conteneurs)**

L'application est orchestrée par Docker Compose et se divise en 4 services :

Reverse Proxy (Nginx) : Le point d'entrée unique. Il redirige les requêtes selon le chemin :

localhost:8080/ => sert le Frontend (Vue.js => http://frontend:8081).

localhost:8080/api/ => Redirige vers le Backend (Python => http://backend-traitement:8000).

**🏗️ Stratégie d'Abstraction : Le Pattern DTO**

Le service C# agit comme un filtre de sécurité et de clarté.

MigrationModel (Interne) : Contient les clés primaires (UUID/ID), les hashs de vérification d'intégrité et les métadonnées techniques de la base SQL.

MigrationDto (Externe) : Contient uniquement ce qui est nécessaire au dashboard Vue.js (Dates, Titre, Source, Statut).

**🚀 Installation et Lancement**

Le projet est entièrement Dockerisé. Une seule commande suffit pour monter l'infrastructure complète (4 containers + Base de données).

Prérequis
Docker et Docker Compose installés sur votre machine.

Démarrage
Clonez le dépôt.

À la racine du projet, lancez :

docker compose up -d --build
Accédez à l'application via votre navigateur à l'adresse : http://localhost:8080 
