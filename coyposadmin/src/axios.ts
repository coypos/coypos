import Axios from "axios";
import type { App } from "vue";
import { setupCache, buildWebStorage } from "axios-cache-interceptor";
interface AxiosOptions {
  baseUrl?: string;
  token?: string;
}

export default {
  install: (app: App, options: AxiosOptions) => {
    app.config.globalProperties.$axios = setupCache(
      Axios.create({
        baseURL: options.baseUrl,
        headers: {
          "Content-Type": "application/json",
          Authorization: options.token ? `Bearer ${options.token}` : "",
          XApiKey: process.env.VUE_APP_TOKEN,
        },
      }),
      { storage: buildWebStorage(localStorage, "axios-cache:") }
    );
  },
};
