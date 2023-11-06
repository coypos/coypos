import { createRouter, createWebHistory, RouteRecordRaw } from "vue-router";
import DashboardView from "@/views/DashboardView.vue";
import ProductsView from "@/views/ProductsView.vue";
import SettingsView from "@/views/SettingsView.vue";
const routes: Array<RouteRecordRaw> = [
  {
    path: "/",
    name: "Dashboard",
    component: DashboardView,
  },
  {
    path: "/products",
    name: "ProductsView",
    component: ProductsView,
  },
  {
    path: "/settings",
    name: "SettingsView",
    component: SettingsView,
  },
];

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes,
});

export default router;
