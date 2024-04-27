<template>
  <div v-if="logged">
    <sidebar-component v-if="$route.name != 'InstallView'"></sidebar-component>
    <div id="content">
      <header-component v-if="$route.name != 'InstallView'"></header-component>
      <router-view />
    </div>
  </div>
  <div v-else><login-view></login-view></div>
</template>

<style lang="scss">
:root {
  color: #fdfdfd;
  background-color: #1f1f1f;
}
#content {
  transition: 0.4s;

  margin-left: 20%;
}
#app {
  color: #fdfdfd;
  background-color: #1f1f1f;
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: left;
}
</style>
<script lang="ts">
import HeaderComponent from "@/components/layout/HeaderComponent.vue";
import SidebarComponent from "@/components/layout/SidebarComponent.vue";
import LoginView from "@/views/LoginView.vue";
import { defineComponent, ref } from "vue";
import type { AxiosInstance } from "axios";
import InstallView from "@/views/InstallView.vue";
import { ResponseModel } from "@/types/Response";
import { SettingModel } from "@/types/api/Setting";
declare module "@vue/runtime-core" {
  interface ComponentCustomProperties {
    $axios: AxiosInstance;
    catTags: string[];
  }
}
export default defineComponent({
  name: "App",
  components: {
    HeaderComponent,
    SidebarComponent,
    LoginView,
  },
  setup() {
    let logged = ref<boolean>(false);
    let settings = ref<SettingModel[]>();

    return { logged, settings };
  },
  methods: {
    InstallView,
    async openNav() {
      const sidebar = document.getElementById("content");
      if (sidebar) {
        sidebar.style.marginLeft = "20%";
      }
    },
    async closeNav() {
      const content = document.getElementById("content");
      if (content) {
        content.style.marginLeft = "100px";
      }
    },
    async getSettings() {
      try {
        await this.$axios.get(`/settings`).then((response) => {
          const resp: ResponseModel = response.data;
          this.settings = resp.response;
          if (this.settings.length < 3) {
            this.$router.push({ name: "InstallView" });
          }
        });
      } catch (e: any) {
        await this.$router.push({ name: "InstallView" });
      }
    },
  },

  mounted() {
    this.getSettings();

    setInterval(() => {
      if (this.$route.name == "InstallView") {
        this.$storage.setStorageSync("hidden", true);
      }
      if (this.$storage.getStorageSync("hidden")) {
        this.closeNav();
      } else {
        this.openNav();
      }
      this.logged = this.$storage.getStorageSync("logged") as boolean;
    }, 100);
  },
});
</script>
