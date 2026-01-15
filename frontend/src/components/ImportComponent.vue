<script setup>
import { ref, computed } from 'vue';
// Note: Pour les icônes, j'utilise des emojis pour la simplicité, 
// mais tu peux installer 'lucide-vue-next' pour un look pro.

const selectedFile = ref(null);
const message = ref("");
const isUploading = ref(false);
const progressValue = ref(0);
const dragActive = ref(false);

const handleFileChange = (event) => {
  const file = event.target.files[0] || event.dataTransfer.files[0];
  if (file && (file.type === "text/csv" || file.name.endsWith('.csv'))) {
    selectedFile.value = file;
    message.value = "";
    progressValue.value = 0;
  } else {
    selectedFile.value = null;
    message.value = "Format non supporté. CSV uniquement.";
  }
  dragActive.value = false;
};

const uploadFile = async () => {
  if (!selectedFile.value) return;

  isUploading.value = true;
  progressValue.value = 0;
  const formData = new FormData();
  formData.append('file', selectedFile.value);

  try {
    const response = await fetch('http://localhost:8000/api/files/', {
      method: 'POST',
      body: formData,
    });

    if (!response.ok) throw new Error("Erreur serveur");

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
              message.value = `Import réussi ! ${data.inserted} lignes traitées.`;
              selectedFile.value = null; // Reset après succès
            }
          } catch (e) { /* Ignorer les chunks mal formés */ }
        }
      }
    }
  } catch (error) {
    message.value = "La connexion au serveur a échoué.";
  } finally {
    isUploading.value = false;
  }
};
</script>

<template>
  <main class="min-h-screen bg-slate-50 flex items-center justify-center p-4 bg-gradient-to-br from-blue-50 to-indigo-100">
    <div class="w-full max-w-lg bg-white/80 backdrop-blur-md rounded-2xl shadow-2xl border border-white p-8">
      
      <div class="text-center mb-8">
        <div class="inline-block p-3 bg-blue-600 rounded-2xl shadow-lg mb-4">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-8 w-8 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 16a4 4 0 01-.88-7.903A5 5 0 1115.9 6L16 6a5 5 0 011 9.9M15 13l-3-3m0 0l-3 3m3-3v12" />
          </svg>
        </div>
        <h1 class="text-2xl font-extrabold text-slate-800">Importateur de Données</h1>
        <p class="text-slate-500 mt-1">Téléchargez vos fichiers CSV en un clic</p>
      </div>

      <div 
        @dragover.prevent="dragActive = true" 
        @dragleave.prevent="dragActive = false" 
        @drop.prevent="handleFileChange"
        :class="[
          'relative group transition-all duration-300 border-2 border-dashed rounded-xl p-8 flex flex-col items-center justify-center gap-4',
          dragActive ? 'border-blue-500 bg-blue-50 scale-[1.02]' : 'border-slate-300 hover:border-blue-400 bg-slate-50/50'
        ]"
      >
        <input 
          id="csv-input" 
          type="file" 
          class="absolute inset-0 w-full h-full opacity-0 cursor-pointer" 
          accept=".csv" 
          @change="handleFileChange"
          :disabled="isUploading"
        />
        
        <div v-if="!selectedFile" class="text-center">
          <p class="text-slate-600 font-medium text-lg">Déposez votre fichier ici</p>
          <p class="text-slate-400 text-sm">ou cliquez pour parcourir</p>
        </div>

        <div v-else class="flex items-center gap-3 bg-white p-3 rounded-lg shadow-sm border border-blue-100">
          <span class="text-2xl">📄</span>
          <div class="text-left">
            <p class="text-sm font-bold text-slate-700 truncate max-w-[200px]">{{ selectedFile.name }}</p>
            <p class="text-xs text-slate-400">{{ (selectedFile.size / 1024).toFixed(1) }} KB</p>
          </div>
          <button @click.stop="selectedFile = null" class="ml-2 text-slate-400 hover:text-red-500 transition-colors">✕</button>
        </div>
      </div>

      <div v-if="isUploading" class="mt-8 space-y-2">
        <div class="flex justify-between text-sm font-semibold text-blue-600">
          <span>Traitement en cours...</span>
          <span>{{ progressValue }}%</span>
        </div>
        <div class="w-full bg-slate-200 rounded-full h-3">
          <div 
            class="bg-blue-600 h-full rounded-full transition-all duration-500 ease-out shadow-[0_0_10px_rgba(37,99,235,0.4)]"
            :style="{ width: progressValue + '%' }"
          ></div>
        </div>
      </div>

      <Transition name="fade">
        <div v-if="message" :class="[
          'mt-6 p-4 rounded-lg text-sm font-medium text-center border',
          message.includes('Erreur') || message.includes('échec') ? 'bg-red-50 text-red-600 border-red-100' : 'bg-green-50 text-green-700 border-green-100'
        ]">
          {{ message }}
        </div>
      </Transition>

      <button 
        @click="uploadFile"
        :disabled="!selectedFile || isUploading"
        class="w-full mt-8 py-4 px-6 bg-slate-900 text-white font-bold rounded-xl shadow-xl hover:bg-slate-800 disabled:opacity-30 disabled:pointer-events-none transition-all active:scale-[0.98] flex items-center justify-center gap-2"
      >
        <span v-if="isUploading" class="animate-spin border-2 border-white/30 border-t-white rounded-full h-5 w-5"></span>
        {{ isUploading ? 'Importation...' : 'Lancer l\'import' }}
      </button>

    </div>
  </main>
</template>

<style scoped>
.fade-enter-active, .fade-leave-active {
  transition: opacity 0.3s ease;
}
.fade-enter-from, .fade-leave-to {
  opacity: 0;
}

@keyframes spin {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}
.animate-spin {
  animation: spin 1s linear infinite;
}
</style>
