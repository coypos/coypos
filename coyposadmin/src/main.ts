import { createApp } from "vue";
import App from "./App.vue";
import router from "./router";
import i18n from "./i18n";
import axios from "./axios";
import "bootstrap/dist/css/bootstrap.min.css";
import "bootstrap";
import Vue3Storage, { StorageType } from "vue3-storage";
createApp(App)
  .use(i18n)
  .use(router)
  .use(Vue3Storage, { namespace: "pro_", storage: StorageType.Local })
  .use(axios, {
    baseUrl: process.env.VUE_APP_API_URL || "http://localhost:5016",
  })
  .mount("#app");
