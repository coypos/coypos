<template>
  <sidebar-component></sidebar-component>
  <div id="content">
    <header-component></header-component>
    <router-view />
  </div>
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
import { defineComponent, ref } from "vue";
export default defineComponent({
  name: "App",
  components: {
    HeaderComponent,
    SidebarComponent,
  },
  methods: {
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
  },
  mounted() {
    setInterval(() => {
      if (this.$storage.getStorageSync("hidden")) {
        this.closeNav();
      } else {
        this.openNav();
      }
    }, 100);
  },
});
</script>
