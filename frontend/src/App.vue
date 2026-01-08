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
}

/* Header Fixe */
.navbar {
  position: sticky;
  top: 0;
  z-index: 1000;
  background-color: rgba(255, 255, 255, 0.8);
  backdrop-filter: blur(10px); /* Effet flou moderne */
  border-bottom: 1px solid #e5e7eb;
  padding: 0.75rem 1.5rem;
}

.navbar-content {
  display: flex;
  align-items: center;
  justify-content: space-between; /* Sépare le logo des liens */
  max-width: 1200px;
  margin: 0 auto;
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
  flex-grow: 1; /* Prend toute la place restante */
  padding-top: 2rem; /* Espace sous le header */
}
</style>
