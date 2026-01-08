<template>
  <header>
    <img alt="Vue logo" class="logo" src="./assets/logo.svg" width="125" height="125" />
    <nav class="nav-links">
      <RouterLink to="/import" class="nav-item">Import CSV</RouterLink>
      <RouterLink to="/visualisation" class="nav-item">Visualisation</RouterLink>
    </nav>
  </header>

  <main>
    <RouterView />
  </main>
</template>

<style scoped>
header {
  position: sticky;
  line-height: 1.5;
  top: 0;
  left: 0;
  width: 100%;
  display: flex;
  align-items: center;
  gap: 2rem;
  max-width: 1200px;
  margin: 0 auto;
}

.logo {
  display: block;
  margin: 0 auto 2rem;
}

.nav-links {
  display: flex;
  gap: 1.5rem;
}

.nav-item {
  text-decoration: none;
  color: #4b5563;
  font-weight: 600;
  transition: color 0.2s;
}

/* Style auto pour le lien actif */
.router-link-active {
  color: #2563eb;
  border-bottom: 2px solid #2563eb;
}
</style>
