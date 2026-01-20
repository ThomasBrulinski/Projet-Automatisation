import { defineStore } from 'pinia';
import { ref } from 'vue';

export const useImportStore = defineStore('import', () => {
  const selectedFile = ref(null);
  const message = ref("");
  const isUploading = ref(false);
  const progressValue = ref(0);
  const dragActive = ref(false);
  const messageType = ref(""); // 'success' ou 'error'

  const handleFileChange = (file) => {
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
    message.value = "";
    progressValue.value = 0;
    
    const formData = new FormData();
    formData.append('file', selectedFile.value);

    try {
      const apiUrl = "/api/files/";
      const response = await fetch(apiUrl, {
        method: 'POST',
        body: formData,
      });

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.error || "Erreur lors de l'envoi");
      }

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
                progressValue.value = 100;
              }
            } catch (e) { /* Ignorer les chunks partiels */ }
          }
        }
      }
    } catch (error) {
      message.value = error.message || "Échec de la connexion au serveur.";
      messageType.value = "error";
    } finally {
      isUploading.value = false;
    }
  };

  const clearFile = () => {
    selectedFile.value = null;
  };

  const setDragActive = (value) => {
    dragActive.value = value;
  };

  return {
    selectedFile,
    message,
    isUploading,
    progressValue,
    dragActive,
    messageType,
    handleFileChange,
    uploadFile,
    clearFile,
    setDragActive
  };
});
