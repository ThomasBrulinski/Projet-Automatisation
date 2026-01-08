<script setup>
import { ref } from 'vue';

const selectedFile = ref(null);
const message = ref("");
const isUploading = ref(false);
const progressValue = ref(0); // État de la progression (0 à 100)

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

          if (data.complete) {
            message.value = `Import terminé ! ✅ ${data.inserted} lignes ajoutées, ${data.skipped} déjà présentes.`;
          }
        }
      }
    }

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
    </div>  
  </main>
</template>

<style scoped>
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
