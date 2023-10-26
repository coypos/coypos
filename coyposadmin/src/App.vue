<template>
  <sidebar-component></sidebar-component>
  <div class="content">
    <header-component></header-component>
    <router-view />
  </div>
</template>

<style lang="scss">
:root {
  color: #fdfdfd;
  background-color: #1f1f1f;
}
.content {
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
<script setup lang="ts">
import HeaderComponent from "@/components/layout/HeaderComponent.vue";
import SidebarComponent from "@/components/layout/SidebarComponent.vue";
</script>
