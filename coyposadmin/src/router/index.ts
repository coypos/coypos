import { createRouter, createWebHistory, RouteRecordRaw } from "vue-router";
import DashboardView from "@/views/DashboardView.vue";
import ProductsView from "@/views/ProductsView.vue";
import SettingsView from "@/views/SettingsView.vue";
import CategoriesView from "@/views/CategoriesView.vue";
import PromotionsView from "@/views/PromotionsView.vue";
import UsersView from "@/views/UsersView.vue";
import PaymentMethodsView from "@/views/PaymentMethodsView.vue";
import EmployeesView from "@/views/EmployeesView.vue";
import LanguagesView from "@/views/LanguagesView.vue";

import StoreUsersView from "@/views/StoreUsersView.vue";

import InstallView from "@/views/InstallView.vue";

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
    path: "/categories",
    name: "CategoriesView",
    component: CategoriesView,
  },
  {
    path: "/promotions",
    name: "PromotionsView",
    component: PromotionsView,
  },
  {
    path: "/users",
    name: "UsersView",
    component: UsersView,
  },
  {
    path: "/store_users",
    name: "StoreUsersView",
    component: StoreUsersView,
  },
  {
    path: "/employees",
    name: "EmployeesView",
    component: EmployeesView,
  },
  {
    path: "/settings",
    name: "SettingsView",
    component: SettingsView,
  },
  {
    path: "/languages",
    name: "LanguagesView",
    component: LanguagesView,
  },
  {
    path: "/payment_methods",
    name: "PaymentMethodsView",
    component: PaymentMethodsView,
  },
  {
    path: "/install",
    name: "InstallView",
    component: InstallView,
  },
];

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes,
});

export default router;
