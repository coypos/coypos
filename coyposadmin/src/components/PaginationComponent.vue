<template>
  <div class="row productbottom">
    <div class="col-2">Stron: {{ totalPages }}</div>
    <div class="col-8">
      <i
        v-if="page != 1"
        @click="goToPage(page - 1)"
        class="fa-solid fa-arrow-left fa-xl"
        style="color: #ffffff"
      ></i>
      &nbsp;
      <span v-for="page in totalPages" :key="page"
        >&nbsp;
        <a v-if="page == this.page" class="active" href="javascript:void(0)">{{
          page
        }}</a>
        <a v-else @click="goToPage(page)" href="javascript:void(0)">{{
          page
        }}</a>
      </span>
      &nbsp;
      <i
        v-if="page != totalPages"
        @click="goToPage(page + 1)"
        class="fa-solid fa-arrow-right fa-xl"
        style="color: #ffffff"
      ></i>
    </div>
    <div class="col-2">
      <a @click="setItems(10)" href="javascript:void(0)">10</a>&nbsp;
      <a @click="setItems(25)" href="javascript:void(0)">25</a>&nbsp;
      <a @click="setItems(50)" href="javascript:void(0)">50</a>&nbsp;
      <a @click="setItems(100)" href="javascript:void(0)">100</a>
    </div>
  </div>
</template>
<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "PaginationComponent",
  props: {
    totalPages: Number,
    page: Number,
    itemsPerPage: Number,
  },
  methods: {
    async setItems(items: number) {
      const currentParams = await this.$router.currentRoute.value.query;
      const currentName = this.$router.currentRoute.value.name;
      const mergedParams = { ...currentParams, itemsPerPage: items, page: 1 };

      if (currentName) {
        await this.$router.push({ name: currentName, query: mergedParams });
      }
    },
    async goToPage(page: number) {
      const currentParams = await this.$router.currentRoute.value.query;
      const currentName = this.$router.currentRoute.value.name;
      const mergedParams = { ...currentParams, page: page };

      if (currentName) {
        await this.$router.push({ name: currentName, query: mergedParams });
      }
    },
  },
});
</script>
<style scoped lang="scss">
.productbottom {
  height: 50px;
  line-height: 50px;
  background-color: #5c5c5c;
}

a {
  color: white;
}
.active {
  background-color: #6e47d6;
}
.col-8 {
  text-align: center;
}
</style>
