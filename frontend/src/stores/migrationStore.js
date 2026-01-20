import { defineStore } from 'pinia';
import { ref } from 'vue';

export const useMigrationStore = defineStore('migration', () => {
  const migrations = ref([]);
  const pagination = ref({ total: 0, debut: 0, fin: 0 });
  const currentPage = ref(0);
  const searchQuery = ref("");
  const isLoading = ref(false);
  const message = ref("");

  const fetchMigrations = async (pageOffset = 0) => {
    const newPage = currentPage.value + pageOffset;
    
    if (newPage < 0) return;

    isLoading.value = true;
    try {
      const response = await fetch(
        `/api/files/?page=${newPage}&query=${encodeURIComponent(searchQuery.value)}`
      );
      if (!response.ok) throw new Error("Erreur serveur");
      
      const result = await response.json();
      const payload = result.data;

      if (payload.migrations && payload.migrations.length > 0) {
        migrations.value = payload.migrations;
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
      message.value = "";
    } catch (error) {
      message.value = "Erreur de chargement des données.";
      console.error(error);
    } finally {
      isLoading.value = false;
    }
  };

  const setSearchQuery = (query) => {
    searchQuery.value = query;
    currentPage.value = 0; // Réinitialiser à la page 0
  };

  const resetPage = () => {
    currentPage.value = 0;
  };

  return {
    migrations,
    pagination,
    currentPage,
    searchQuery,
    isLoading,
    message,
    fetchMigrations,
    setSearchQuery,
    resetPage
  };
});
