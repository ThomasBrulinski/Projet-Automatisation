import { createRouter, createWebHashHistory } from 'vue-router'
import ImportView from '../views/ImportView.vue'
import DataView from '../views/DataView.vue'

const router = createRouter({
  history: createWebHashHistory(),
  routes: [
    { path: '/', redirect: '/import' },
    { path: '/import', component: ImportView },
    { path: '/visualisation', component: DataView }
  ]
})

export default router
