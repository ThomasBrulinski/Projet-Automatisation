<script setup>
import { ref } from 'vue';

const selectedFile = ref(null);
const message = ref("");
const isUploading = ref(false);
const progressValue = ref(0); // État de la progression (0 à 100)
// ... tes autres imports
const migrations = ref([]);
const isLoadingData = ref(false);
const currentPage = ref(0); // Remplacer pagination par une ref

const fetchMigrations = async (pageOffset = 0) => {
  // On calcule la nouvelle page demandée
  const newPage = currentPage.value + pageOffset;
  if (newPage < 0) return; // Empêche d'aller en dessous de 0

  isLoadingData.value = true;
  try {
    const response = await fetch(`http://localhost:8000/api/files?page=${newPage}`);
    if (!response.ok) throw new Error("Erreur lors du chargement");
    
    const data = await response.json();
    
    // Si on a des données, on met à jour la liste et la page actuelle
    if (data.length > 0) {
      migrations.value = data;
      currentPage.value = newPage;
    } else if (pageOffset === 0) {
      // Si c'est le premier chargement et que c'est vide
      migrations.value = [];
    } else {
      message.value = "Fin des données disponibles.";
    }
  } catch (error) {
    message.value = "Impossible de charger les données.";
  } finally {
    isLoadingData.value = false;
  }
};

const handleFileChange = (event) => {
  const file = event.target.files[0];
  if (file && (file.type === "text/csv" || file.name.endsWith('.csv'))) {
    selectedFile.value = file;
    message.value = `Fichier sélectionné : ${file.name}`;
    progressValue.value = 0; // Reset la barre
  } else {
    selectedFile.value = null;
    message.value = "Veuillez sélectionner un fichier .csv uniquement.";
  }
};

const uploadFile = async () => {
  if (!selectedFile.value) return;

  isUploading.value = true;
  progressValue.value = 0;
  const formData = new FormData();
  formData.append('file', selectedFile.value);

  try {
    const response = await fetch('http://localhost:8000/api/files', {
      method: 'POST',
      body: formData,
    });

    if (!response.ok) throw new Error("Erreur serveur");

    // LECTURE DU STREAM (pour la barre de progression)
    const reader = response.body.getReader();
    const decoder = new TextDecoder();

    while (true) {
      const { done, value } = await reader.read();
      if (done) break;

      // Le serveur envoie des morceaux (chunks) de texte
      const chunk = decoder.decode(value);
      const lines = chunk.split('\n');

      for (const line of lines) {
        if (line.trim()) {
          const data = JSON.parse(line);
          
          if (data.error) {
            message.value = `Erreur: ${data.details}`;
            isUploading.value = false;
            return;
          }
          
          if (data.progress !== undefined) {
            progressValue.value = data.progress;
          }
        }
      }
    }

    message.value = "Traitement terminé avec succès !";
  } catch (error) {
    message.value = "Erreur de connexion avec l'API.";
    console.error(error);
  } finally {
    isUploading.value = false;
  }
};
</script>

<template>
  <main class="flex min-h-screen flex-col items-center justify-center p-6 bg-gray-50">
    <div class="w-full max-w-md p-8 bg-white rounded-xl shadow-lg">
      <h1 class="mb-6 text-3xl font-bold text-gray-800 text-center">Import CSV</h1>
      
      <div class="flex flex-col gap-4">
        <label v-if="!isUploading" for="csv-input" 
          class="flex flex-col items-center justify-center w-full h-32 border-2 border-dashed border-blue-300 rounded-lg cursor-pointer bg-blue-50 hover:bg-blue-100 transition-colors">
          <p class="text-sm text-blue-700 font-semibold text-center">Cliquez pour choisir un CSV</p>
          <input id="csv-input" type="file" class="hidden" accept=".csv" @change="handleFileChange" />
        </label>

        <div v-if="isUploading" class="w-full bg-gray-200 rounded-full h-4 overflow-hidden">
          <div 
            class="bg-blue-600 h-full transition-all duration-300 ease-out"
            :style="{ width: progressValue + '%' }"
          ></div>
        </div>
        <p v-if="isUploading" class="text-center text-sm font-bold text-blue-600">
          Progression : {{ progressValue }}%
        </p>

        <p v-if="message" :class="selectedFile ? 'text-green-600' : 'text-red-500'" class="text-sm font-medium text-center">
          {{ message }}
        </p>

        <button 
          @click="() => uploadFile()"
          :disabled="!selectedFile || isUploading"
          class="w-full py-3 px-4 bg-blue-600 text-white font-bold rounded-lg shadow-md hover:bg-blue-700 disabled:opacity-50 transition-all"
        >
          {{ isUploading ? 'Traitement en cours...' : 'Lancer l\'import' }}
        </button>
      </div>
      <button 
        @click="() => fetchMigrations()"
        :disabled="isLoadingData"
        class="btn-fetch"
      >
        {{ isLoadingData ? 'Chargement...' : 'Afficher les données de la BD' }}
      </button>

      <div v-if="migrations.length > 0" class="table-container">
        <table class="excel-table">
          <thead>
            <tr>
              <th>Date</th>
              <th>SubJob ID</th>
              <th>Titre</th>
              <th>Statut</th>
              <th>Taille</th>
              <th>Code Erreur</th>
              <th>Commentaire</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="m in migrations" :key="m.id">
              <td>{{ m.migrationStartTime }}</td>
              <td class="font-mono">{{ m.subJobId }}</td>
              <td>{{ m.title }}</td>
              <td>
                <span :class="['badge', m.status === 'Successful' ? 'success' : 'error']">
                  {{ m.status }}
                </span>
              </td>
              <td>{{ m.size }}</td>
              <td class="text-error">{{ m.errorCode || '-' }}</td>
              <td class="comment-cell">{{ m.comment || '-' }}</td>
            </tr>
          </tbody>
        </table>
      </div>
      <div v-if="migrations.length > 0" class="pagination-controls">
        <button 
          @click="() => fetchMigrations(-1)" 
          :disabled="currentPage === 0 || isLoadingData"
          class="page-btn"
        >
          ← Précédent
        </button>
        
        <span class="page-info">Page {{ currentPage + 1 }}</span>
        
        <button 
          @click="() => fetchMigrations(1)" 
          :disabled="isLoadingData || migrations.length < 20"
          class="page-btn"
        >
          Suivant →
        </button>
      </div>
    </div>
  </main>
</template>

<style scoped>

  /* Style pour harmoniser la zone d'import avec le tableau */
.max-w-md {
  max-width: 900px !important; /* On élargit pour que le tableau soit à l'aise */
}

/* Contrôles de pagination */
.pagination-controls {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 20px;
  margin-top: 15px;
  padding: 10px;
  background-color: #fff;
}

.page-btn {
  padding: 8px 16px;
  background-color: #f3f4f6;
  border: 1px solid #d1d5db;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 500;
  transition: all 0.2s;
}

.page-btn:hover:not(:disabled) {
  background-color: #e5e7eb;
}

.page-btn:disabled {
  opacity: 0.4;
  cursor: not-allowed;
}

.page-info {
  font-size: 14px;
  font-weight: bold;
  color: #4b5563;
}

/* Correction de la table-container pour éviter qu'elle soit trop petite */
.table-container {
  max-height: 500px;
  overflow-y: auto;
}

.table-container {
  margin-top: 20px;
  width: 100%;
  overflow-x: auto;
  border: 1px solid #ddd;
  border-radius: 8px;
}

.excel-table {
  width: 100%;
  border-collapse: collapse;
  font-family: sans-serif;
  font-size: 14px;
}

.excel-table th {
  background-color: #f4f4f4;
  padding: 12px;
  text-align: left;
  border-bottom: 2px solid #ddd;
  border-right: 1px solid #ddd;
  color: #333;
}

.excel-table td {
  padding: 10px;
  border-bottom: 1px solid #eee;
  border-right: 1px solid #ddd;
}

.excel-table tr:hover {
  background-color: #f9f9f9;
  color: #333;
}

.font-mono { font-family: monospace; font-size: 12px; }

.badge {
  padding: 4px 8px;
  border-radius: 12px;
  font-size: 11px;
  font-weight: bold;
}

.success { background-color: #dcfce7; color: #166534; }
.error { background-color: #fee2e2; color: #991b1b; }

.comment-cell {
  max-width: 200px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.btn-fetch {
  width: 100%;
  background-color: #10b981;
  color: white;
  padding: 12px;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-weight: bold;
  margin-top: 10px;
}

.btn-fetch:disabled { opacity: 0.5; cursor: not-allowed; }
</style>
