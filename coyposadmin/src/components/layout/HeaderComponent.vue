<template>
  <div id="header" class="row">
    <div class="col-3">Panel</div>
    <div class="col-3">Searchplaceholder</div>
    <div class="col-2">buttons</div>
    <div class="col-2">Piotr Smilgin</div>
    <div class="col-2">
      <div @click="logout" class="btn btn-outline-warning">Wyloguj</div>
    </div>
  </div>
</template>
<script lang="ts">
import { defineComponent, ref } from "vue";

export default defineComponent({
  name: "HeaderComponent",

  methods: {
    logout() {
      this.$storage.setStorageSync("logged", false);
      window.location.reload();
    },
  },
});
</script>
<style scoped lang="scss">
#header {
  z-index: 10;
  background-color: #1f1f1f;
  border-bottom: 3px solid #2c2c2c;
  width: 103%;
  overflow: hidden;
  height: 72px;
  line-height: 72px;
  font-size: 1.2em;
  margin-left: -40px;
  margin-right: 200px;
}
</style>
