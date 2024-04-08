<template>
  <div class="home">
    <dashboard-component />
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import DashboardComponent from "@/components/DashboardComponent.vue"; // @ is an alias to /src

export default defineComponent({
  name: "DashboardView",
  components: {
    DashboardComponent,
  },
});
</script>
