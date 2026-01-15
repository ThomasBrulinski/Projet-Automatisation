<template>
  <div class="app-container">
    <header class="navbar">
      <div class="navbar-content">
        <div class="logo-section">
          <img alt="Vue logo" class="logo" src="./assets/logo.svg" width="35" height="35" />
          <span class="brand-name">DataFlow</span>
        </div>
        
        <nav class="nav-links">
          <RouterLink to="/import" class="nav-item">Import CSV</RouterLink>
          <RouterLink to="/visualisation" class="nav-item">Visualisation</RouterLink>
        </nav>
      </div>
    </header>

    <main class="main-content">
      <RouterView />
    </main>
  </div>
</template>

<style>
/* Reset global pour s'assurer que le header colle en haut */
body, html {
  margin: 0;
  padding: 0;
  height: 100%;
}

#app {
  width: 100%;
  margin: 0;
  padding: 0;
  max-width: none;
}
</style>

<style scoped>
/* Conteneur principal pour gérer le flux vertical */
.app-container {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
  width: 100%;       /* Prend toute la largeur de #app */
  margin: 0;
  padding: 0;
}

/* Header Fixe */
.navbar {
  position: sticky;
  top: 0;
  z-index: 1000;
  background-color: rgba(255, 255, 255, 0.9);
  backdrop-filter: blur(10px);
  border-bottom: 1px solid #e5e7eb;
  padding: 0.75rem 2rem; /* Plus d'espace sur les côtés */
}

.navbar-content {
  display: flex;
  align-items: center;
  justify-content: space-between;
  /* On enlève le max-width: 1200px pour que le menu suive la largeur du tableau */
  width: 100%; 
}

.logo-section {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.brand-name {
  font-weight: 800;
  font-size: 1.25rem;
  color: #1f2937;
  letter-spacing: -0.5px;
}

.logo {
  display: block;
}

.nav-links {
  display: flex;
  gap: 1rem;
}

.nav-item {
  text-decoration: none;
  color: #6b7280;
  font-weight: 500;
  padding: 0.5rem 1rem;
  border-radius: 8px;
  transition: all 0.2s ease;
}

.nav-item:hover {
  background-color: #f3f4f6;
  color: #1f2937;
}

/* Style lien actif */
.router-link-active {
  color: #2563eb !important;
  background-color: #eff6ff;
  font-weight: 700;
}

/* Zone de contenu */
.main-content {
  flex-grow: 1;
  width: 100%; /* Force le contenu à utiliser 100% de la largeur */
  display: flex;
  flex-direction: column;
}
</style>
