<script setup>
import { ref, computed } from 'vue';

const migrations = ref([]);
const pagination = ref({ total: 0, debut: 0, fin: 0 });
const isLoadingData = ref(false);
const currentPage = ref(0); 
const searchQuery = ref("");
const message = ref("");

const fetchMigrations = async (pageOffset = 0) => {
  const newPage = currentPage.value + pageOffset;
  const apiUrl = "/api/files/";
  
  if (newPage < 0) return;

  isLoadingData.value = true;
  try {
    const response = await fetch(`${apiUrl}?page=${newPage}&query=${encodeURIComponent(searchQuery.value)}`);
    if (!response.ok) throw new Error("Erreur serveur");
    
    const result = await response.json();
    
    // Accès aux données via data.data conformément à ton DTO C#
    const payload = result.data; 

    if (payload.migrations && payload.migrations.length > 0) {
      migrations.value = payload.migrations;
      // Mise à jour des métadonnées pour l'affichage "X - Y sur Z"
      pagination.value = {
        total: payload.totalCount,
        debut: payload.debut,
        fin: payload.fin
      };
      currentPage.value = newPage;
    } else if (pageOffset === 0) {
      migrations.value = [];
      pagination.value = { total: 0, debut: 0, fin: 0 };
    }
  } catch (error) {
    message.value = "Erreur de chargement des données.";
    console.error(error);
  } finally {
    isLoadingData.value = false;
  }
};

// Texte dynamique pour le footer
const paginationLabel = computed(() => {
  if (pagination.value.total === 0) return "0 - 0 sur 0";
  return `${pagination.value.debut} - ${pagination.value.fin} sur ${pagination.value.total}`;
});
</script>

<template>
  <div class="dashboard-container">
    
    <header class="dashboard-header">
      <div class="header-content">
        <h1>📊 Moniteur de Migration</h1>
        <p class="subtitle">Suivi des transferts de fichiers en temps réel</p>
      </div>
      
      <div class="actions-bar">
        <div class="search-wrapper">
          <span class="search-icon">🔍</span>
          <input 
            v-model="searchQuery" 
            type="text" 
            placeholder="Rechercher (Source)" 
            @keyup.enter="() => fetchMigrations(0)"
            class="search-input"
          />
        </div>
        
        <button 
          @click="() => fetchMigrations(0)"
          :disabled="isLoadingData"
          class="btn-primary"
        >
          <span v-if="isLoadingData" class="loader"></span>
          <span v-else>Actualiser</span>
        </button>
      </div>
    </header>

    <main class="main-content">
      
      <div v-if="migrations.length === 0 && !isLoadingData" class="empty-state">
        <div class="empty-icon">📁</div>
        <h3>Aucune donnée à afficher</h3>
        <p>Lancez une recherche ou cliquez sur Actualiser pour voir les migrations.</p>
        <button @click="() => fetchMigrations(0)" class="btn-secondary">Charger les données</button>
      </div>

      <div v-else class="table-card">
        <div class="table-responsive">
          <table class="modern-table">
            <thead>
              <tr>
                <th>Date</th>
                <th>SubJob ID</th>
                <th>Titre</th>
                <th>Type</th>
                <th>Source</th>
                <th>Destination</th>
                <th>Statut</th>
                <th>Taille</th>
                <th>Code</th>
                <th>Info</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="m in migrations" :key="m.id">
                <td class="whitespace-nowrap">{{ new Date(m.migrationStartTime).toLocaleString() }}</td>
                <td class="font-mono text-sm">{{ m.subJobId }}</td>
                <td class="font-bold">{{ m.title }}</td>
                <td><span class="tag-type">{{ m.type }}</span></td>
                
                <td>
                  <div class="path-cell">
                    <span class="path-id">ID: {{ m.sourceId }}</span>
                    <span class="path-text" :title="m.source">{{ m.source }}</span>
                  </div>
                </td>
                
                <td>
                  <div class="path-cell">
                    <span class="path-id">ID: {{ m.destinationId }}</span>
                    <span class="path-text" :title="m.destination">{{ m.destination }}</span>
                  </div>
                </td>
                
                <td>
                  <span :class="['status-badge', m.status === 'Successful' ? 'status-success' : 'status-error']">
                    {{ m.status === 'Successful' ? 'Succès' : 'Erreur' }}
                  </span>
                </td>
                
                <td class="font-mono">{{ m.size }}</td>
                <td class="text-red-500 font-bold">{{ m.errorCode || '-' }}</td>
                <td class="comment-cell" :title="m.comment">{{ m.comment || '-' }}</td>
              </tr>
            </tbody>
          </table>
        </div>

        <div class="pagination-footer">
          <span class="page-range-info">{{ paginationLabel }}</span>

          <div class="nav-controls">
            <button 
              @click="() => fetchMigrations(-1)" 
              :disabled="currentPage === 0 || isLoadingData"
              class="btn-nav"
            >
              &larr; Précédent
            </button>
            
            <span class="page-indicator">Page {{ currentPage + 1 }}</span>
            
            <button 
              @click="() => fetchMigrations(1)" 
              :disabled="isLoadingData || pagination.fin >= pagination.total"
              class="btn-nav"
            >
              Suivant &rarr;
            </button>
          </div>
        </div>
      </div>
    </main>
  </div>
</template>

<style scoped>
/* --- RESET & LAYOUT --- */
.dashboard-container {
  min-height: 100vh;
  background-color: #f3f4f6;
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
  display: flex;
  flex-direction: column;
}

.dashboard-header {
  background-color: white;
  padding: 1rem 2rem;
  box-shadow: 0 1px 3px rgba(0,0,0,0.1);
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 20px;
}

.header-content h1 {
  margin: 0;
  font-size: 1.5rem;
  color: #111827;
}

.subtitle {
  margin: 0;
  color: #6b7280;
  font-size: 0.9rem;
}

.main-content {
  flex: 1;
  padding: 2rem;
  /* C'est ICI qu'on règle la largeur : 95% de l'écran, centré, max 1600px */
  width: 95%;
  max-width: 1600px; 
  margin: 0 auto;
}

/* --- BARRE D'ACTIONS --- */
.actions-bar {
  display: flex;
  gap: 12px;
}

.search-wrapper {
  position: relative;
  display: flex;
  align-items: center;
}

.search-icon {
  position: absolute;
  left: 10px;
  color: #9ca3af;
}

.search-input {
  padding: 10px 10px 10px 35px;
  border: 1px solid #d1d5db;
  border-radius: 8px;
  width: 250px;
  outline: none;
  transition: all 0.2s;
}

.search-input:focus {
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
  width: 300px;
}

.btn-primary {
  background-color: #2563eb;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 8px;
  font-weight: 600;
  cursor: pointer;
  transition: background 0.2s;
  display: flex;
  align-items: center;
  justify-content: center;
  min-width: 100px;
}

.btn-primary:hover:not(:disabled) {
  background-color: #1d4ed8;
}

.btn-primary:disabled {
  opacity: 0.7;
  cursor: wait;
}

/* --- TABLEAU --- */
.table-card {
  background: white;
  border-radius: 12px;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
  overflow: hidden; /* Important pour les coins arrondis */
  display: flex;
  flex-direction: column;
  height: calc(100vh - 180px); /* Hauteur dynamique pour rester dans l'écran */
}

.table-responsive {
  overflow: auto;
  flex: 1;
}

.modern-table {
  width: 100%;
  border-collapse: separate;
  border-spacing: 0;
  font-size: 0.9rem;
}

.modern-table th {
  background-color: #f9fafb;
  color: #374151;
  font-weight: 600;
  text-transform: uppercase;
  font-size: 0.75rem;
  letter-spacing: 0.05em;
  padding: 16px;
  text-align: left;
  border-bottom: 1px solid #e5e7eb;
  position: sticky;
  top: 0;
  z-index: 10;
}

.modern-table td {
  padding: 14px 16px;
  border-bottom: 1px solid #f3f4f6;
  color: #1f2937;
  vertical-align: middle;
}

.modern-table tr:hover td {
  background-color: #f8fafc;
}

/* --- CELLULES SPÉCIFIQUES --- */
.path-cell {
  display: flex;
  flex-direction: column;
  max-width: 200px;
}

.path-id {
  font-size: 0.7rem;
  color: #9ca3af;
  font-family: monospace;
}

.path-text {
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.tag-type {
  background-color: #eff6ff;
  color: #1e40af;
  padding: 2px 8px;
  border-radius: 4px;
  font-size: 0.75rem;
  font-weight: 600;
}

.status-badge {
  display: inline-flex;
  padding: 4px 10px;
  border-radius: 9999px;
  font-size: 0.75rem;
  font-weight: 700;
}

.status-success {
  background-color: #dcfce7;
  color: #166534;
}

.status-error {
  background-color: #fee2e2;
  color: #991b1b;
}

.comment-cell {
  max-width: 150px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  color: #6b7280;
  font-style: italic;
}

.font-mono { font-family: 'Courier New', Courier, monospace; }
.whitespace-nowrap { white-space: nowrap; }

/* --- FOOTER PAGINATION --- */
.pagination-footer {
  padding: 12px 20px;
  background-color: white;
  border-top: 1px solid #e5e7eb;
  display: flex;
  justify-content: space-between; /* Aligne le texte à gauche et les boutons à droite */
  align-items: center;
}

.page-range-info {
  font-size: 0.85rem;
  color: #6b7280;
  font-weight: 500;
}

.nav-controls {
  display: flex;
  align-items: center;
  gap: 15px;
}

.btn-nav {
  background: white;
  border: 1px solid #d1d5db;
  padding: 6px 12px;
  border-radius: 6px;
  cursor: pointer;
  color: #374151;
  transition: all 0.2s;
}

.btn-nav:hover:not(:disabled) {
  border-color: #2563eb;
  color: #2563eb;
}

.btn-nav:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

/* --- EMPTY STATE --- */
.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 400px;
  color: #6b7280;
  text-align: center;
}

.empty-icon {
  font-size: 3rem;
  margin-bottom: 1rem;
  opacity: 0.5;
}

.btn-secondary {
  margin-top: 15px;
  background: white;
  border: 1px solid #d1d5db;
  padding: 8px 16px;
  border-radius: 6px;
  cursor: pointer;
}

/* --- LOADER --- */
.loader {
  border: 2px solid rgba(255,255,255,0.3);
  border-radius: 50%;
  border-top: 2px solid white;
  width: 16px;
  height: 16px;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}
</style>
