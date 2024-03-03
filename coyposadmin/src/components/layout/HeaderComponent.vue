<template>
  <div id="header" class="row">
    <div class="col-4"></div>
    <div class="col-5">
      <div
        v-if="$router.currentRoute.value.name == `ProductsView`"
        class="form-check"
      >
        <input type="text" v-model="search" class="form-control" id="search" />
      </div>
      <button
        v-if="$router.currentRoute.value.name == `ProductsView`"
        @click="addToSearchQuery"
        id="searchbutton"
        class="btn btn-success"
      >
        <i class="fa-solid fa-2xl fa-magnifying-glass"></i>
      </button>
    </div>
    <div class="col-2">Piotr Smilgin</div>
    <div class="col-1">
      <div @click="logout" class="btn btn-outline-warning">Wyloguj</div>
    </div>
  </div>
</template>
<script lang="ts">
import { defineComponent, ref } from "vue";

export default defineComponent({
  name: "HeaderComponent",
  setup() {
    let search = ref<string>();
    let searchHelper = ref<string>();
    return { search, searchHelper };
  },
  methods: {
    logout() {
      this.$storage.setStorageSync("logged", false);
      window.location.reload();
    },
    addToSearchQuery() {
      if (this.search) {
        const query = { ...this.$route.query, q: this.search };
        this.$router.replace({ query });
      } else {
        const query = { ...this.$route.query, q: null };
        this.$router.replace({ query });
      }
    },
    chechForChanges() {
      let interval = setInterval(() => {
        if (this.$router.currentRoute.value.name != `ProductsView`) {
          this.search = "";
          this.searchHelper = "";
          this.addToSearchQuery();
        }
        if (this.search != this.searchHelper && this.search != null) {
          this.addToSearchQuery();
        } else {
          this.searchHelper = this.search;
        }
      }, 200);
    },
  },
  mounted() {
    this.search = this.$route.query.q as string;
    this.chechForChanges();
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
.form-check {
  width: 70%;
  display: inline-block;
  #search {
    display: inline-block;
    padding: 10px;
    border-bottom-right-radius: 0;
    border-top-right-radius: 0;
  }
}
#searchbutton {
  padding: 10px;
  border-bottom-left-radius: 0;
  border-top-left-radius: 0;
}
</style>
