<script setup>
import { ref } from 'vue';

const selectedFile = ref(null);
const message = ref("");
const isUploading = ref(false);
const progressValue = ref(0);
const dragActive = ref(false);
const messageType = ref(""); // 'success' ou 'error'

const handleFileChange = (event) => {
  const file = event.target.files[0] || event.dataTransfer.files[0];
  
  if (file && (file.type === "text/csv" || file.name.endsWith('.csv'))) {
    selectedFile.value = file;
    message.value = "";
    progressValue.value = 0;
  } else {
    selectedFile.value = null;
    message.value = "Format invalide. Veuillez sélectionner un fichier CSV.";
    messageType.value = "error";
  }
  dragActive.value = false;
};

const uploadFile = async () => {
  if (!selectedFile.value) return;

  isUploading.value = true;
  message.value = ""; // Clear previous messages
  progressValue.value = 0;
  
  const formData = new FormData();
  formData.append('file', selectedFile.value);

  try {
    const apiUrl = "/api/files/";
    const response = await fetch(apiUrl, {
      method: 'POST',
      body: formData,
    });

    if (!response.ok) throw new Error("Erreur lors de l'envoi");

    const reader = response.body.getReader();
    const decoder = new TextDecoder();

    while (true) {
      const { done, value } = await reader.read();
      if (done) break;

      const chunk = decoder.decode(value);
      const lines = chunk.split('\n');

      for (const line of lines) {
        if (line.trim()) {
          try {
            const data = JSON.parse(line);
            if (data.progress !== undefined) progressValue.value = data.progress;
            if (data.complete) {
              message.value = `Succès ! ${data.inserted} lignes importées.`;
              messageType.value = "success";
              selectedFile.value = null;
              // On laisse la barre à 100% un instant pour le visuel
              progressValue.value = 100;
            }
          } catch (e) { /* Ignorer les chunks partiels */ }
        }
      }
    }
  } catch (error) {
    message.value = "Échec de la connexion au serveur.";
    messageType.value = "error";
  } finally {
    isUploading.value = false;
  }
};
</script>

<template>
  <main class="import-page">
    <div class="upload-card">
      
      <div class="card-header">
        <div class="icon-wrapper">
          <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
            <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"></path>
            <polyline points="17 8 12 3 7 8"></polyline>
            <line x1="12" y1="3" x2="12" y2="15"></line>
          </svg>
        </div>
        <h1>Importateur CSV</h1>
        <p>Glissez votre fichier ou cliquez pour parcourir</p>
      </div>

      <div 
        @dragover.prevent="dragActive = true" 
        @dragleave.prevent="dragActive = false" 
        @drop.prevent="handleFileChange"
        :class="['drop-zone', { 'active': dragActive, 'has-file': selectedFile }]"
      >
        <input 
          type="file" 
          class="file-input" 
          accept=".csv" 
          @change="handleFileChange"
          :disabled="isUploading"
        />
        
        <div v-if="!selectedFile" class="drop-content">
          <span class="upload-icon">☁️</span>
          <span class="upload-text">Déposez votre CSV ici</span>
        </div>

        <div v-else class="file-info">
          <div class="file-icon">📄</div>
          <div class="file-details">
            <span class="file-name">{{ selectedFile.name }}</span>
            <span class="file-size">{{ (selectedFile.size / 1024).toFixed(1) }} KB</span>
          </div>
          <button @click.prevent="selectedFile = null" class="btn-remove">✕</button>
        </div>
      </div>

      <div v-if="isUploading || progressValue > 0" class="progress-section">
        <div class="progress-info">
          <span>Traitement...</span>
          <span>{{ progressValue }}%</span>
        </div>
        <div class="progress-track">
          <div class="progress-bar" :style="{ width: progressValue + '%' }"></div>
        </div>
      </div>

      <div v-if="message" :class="['message-box', messageType]">
        {{ message }}
      </div>

      <button 
        @click="uploadFile"
        :disabled="!selectedFile || isUploading"
        class="btn-upload"
      >
        <span v-if="isUploading" class="loader"></span>
        <span v-else>Lancer l'importation</span>
      </button>

    </div>
  </main>
</template>

<style scoped>
/* --- LAYOUT GLOBAL --- */
.import-page {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: #f3f4f6; /* Gris clair pro */
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
  padding: 20px;
}

/* --- CARTE PRINCIPALE --- */
.upload-card {
  background: white;
  width: 100%;
  max-width: 500px;
  padding: 40px;
  border-radius: 16px;
  box-shadow: 0 10px 25px -5px rgba(0, 0, 0, 0.1), 0 8px 10px -6px rgba(0, 0, 0, 0.1);
  text-align: center;
}

/* --- HEADER --- */
.card-header {
  margin-bottom: 30px;
}

.icon-wrapper {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 50px;
  height: 50px;
  background-color: #eff6ff; /* Bleu très clair */
  color: #2563eb; /* Bleu royal */
  border-radius: 12px;
  margin-bottom: 15px;
}

h1 {
  font-size: 1.5rem;
  color: #111827;
  margin: 0 0 5px 0;
  font-weight: 700;
}

p {
  color: #6b7280;
  font-size: 0.95rem;
  margin: 0;
}

/* --- ZONE DE DROP --- */
.drop-zone {
  position: relative;
  border: 2px dashed #d1d5db;
  border-radius: 12px;
  padding: 30px;
  transition: all 0.2s ease;
  background-color: #f9fafb;
  cursor: pointer;
}

.drop-zone:hover, .drop-zone.active {
  border-color: #2563eb;
  background-color: #eff6ff;
}

.drop-zone.has-file {
  border-style: solid;
  border-color: #bfdbfe;
  background-color: #fff;
  padding: 15px;
}

.file-input {
  position: absolute;
  inset: 0;
  width: 100%;
  height: 100%;
  opacity: 0;
  cursor: pointer;
  z-index: 2;
}

.drop-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 10px;
}

.upload-icon {
  font-size: 2rem;
  opacity: 0.7;
}

.upload-text {
  font-weight: 500;
  color: #4b5563;
}

/* --- FICHIER SÉLECTIONNÉ --- */
.file-info {
  display: flex;
  align-items: center;
  gap: 12px;
  position: relative;
  z-index: 3; /* Au-dessus de l'input invisible */
}

.file-icon {
  font-size: 1.5rem;
}

.file-details {
  flex: 1;
  text-align: left;
  overflow: hidden;
}

.file-name {
  display: block;
  font-weight: 600;
  color: #374151;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.file-size {
  font-size: 0.8rem;
  color: #9ca3af;
}

.btn-remove {
  background: none;
  border: none;
  color: #9ca3af;
  font-size: 1.2rem;
  cursor: pointer;
  padding: 5px;
  transition: color 0.2s;
}

.btn-remove:hover {
  color: #ef4444;
}

/* --- BARRE DE PROGRESSION --- */
.progress-section {
  margin-top: 25px;
  text-align: left;
}

.progress-info {
  display: flex;
  justify-content: space-between;
  font-size: 0.85rem;
  font-weight: 600;
  color: #2563eb;
  margin-bottom: 6px;
}

.progress-track {
  height: 8px;
  background-color: #e5e7eb;
  border-radius: 4px;
  overflow: hidden;
}

.progress-bar {
  height: 100%;
  background-color: #2563eb;
  border-radius: 4px;
  transition: width 0.3s ease;
}

/* --- MESSAGES --- */
.message-box {
  margin-top: 20px;
  padding: 12px;
  border-radius: 8px;
  font-size: 0.9rem;
  font-weight: 500;
}

.success {
  background-color: #dcfce7;
  color: #166534;
  border: 1px solid #bbf7d0;
}

.error {
  background-color: #fee2e2;
  color: #991b1b;
  border: 1px solid #fecaca;
}

/* --- BOUTON PRINCIPAL --- */
.btn-upload {
  width: 100%;
  margin-top: 30px;
  padding: 14px;
  background-color: #2563eb;
  color: white;
  border: none;
  border-radius: 8px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: background 0.2s;
  display: flex;
  align-items: center;
  justify-content: center;
}

.btn-upload:hover:not(:disabled) {
  background-color: #1d4ed8;
}

.btn-upload:disabled {
  background-color: #9ca3af;
  cursor: not-allowed;
  opacity: 0.7;
}

/* --- LOADER --- */
.loader {
  border: 2px solid rgba(255, 255, 255, 0.3);
  border-radius: 50%;
  border-top: 2px solid white;
  width: 18px;
  height: 18px;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}
</style>
