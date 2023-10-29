<template>
  <div class="row">
    <div class="col-12">Produkty</div>
  </div>
  <div class="row productheader">
    <div class="col-1">Nazwa</div>
    <div class="col-1">Kategoria</div>
    <div class="col-2">Kod Kreskowy</div>
    <div class="col-1">Cena</div>
    <div class="col-1">Waga</div>
    <div class="col-1">Zdjęcie</div>
    <div class="col-1">Opis</div>
    <div class="col-1">Na wage?</div>
    <div class="col-1">Włączony?</div>
    <div class="col-1">Edytuj</div>
    <div class="col-1">Usuń</div>
  </div>

  <product-component
    v-for="(product, index) in products"
    :key="index"
    :index="index"
    :product="product"
  ></product-component
  ><pagination-component
    :page="page"
    :itemsPerPage="itemsPerPage"
    :totalPages="totalPages"
  ></pagination-component>
</template>
<script lang="ts">
import { defineComponent, ref } from "vue";
import { ProductModel } from "@/types/api/Product";
import { ResponseModel } from "@/types/Response";

import ProductComponent from "@/components/ProductComponent.vue";
import PaginationComponent from "@/components/PaginationComponent.vue";
export default defineComponent({
  name: "ProductsView",
  components: { PaginationComponent, ProductComponent },
  setup() {
    let products = ref<ProductModel[]>([]);
    let column = ref<number>(0);
    let itemsPerPage = ref<number>(50);
    let page = ref<number>(1);
    let totalPages = ref<number>(1);
    let product = ref<ProductModel>();
    return { product, totalPages, products, column, itemsPerPage, page };
  },
  methods: {
    async getProducts() {
      try {
        await this.$axios
          .get(
            `/products?filter=AND&itemsPerPage=${this.itemsPerPage}&page=${this.page}`
          )
          .then((response) => {
            const resp: ResponseModel = response.data;
            this.products = resp.response;
            this.totalPages = resp.totalPages;
          });
      } catch (e) {
        console.log(e as string);
      }
    },
  },
  mounted() {
    this.page = parseInt(this.$router.currentRoute.value.query.page as string);
    this.itemsPerPage = parseInt(
      this.$router.currentRoute.value.query.itemsPerPage as string
    );
    this.getProducts();
  },
  updated() {
    if (
      this.page !=
        parseInt(this.$router.currentRoute.value.query.page as string) ||
      this.itemsPerPage !=
        parseInt(this.$router.currentRoute.value.query.itemsPerPage as string)
    ) {
      this.page = parseInt(
        this.$router.currentRoute.value.query.page as string
      );
      this.itemsPerPage = parseInt(
        this.$router.currentRoute.value.query.itemsPerPage as string
      );
      this.getProducts();
    }
  },
});
</script>
<style scoped lang="scss">
.productheader {
  height: 50px;
  line-height: 50px;
  background-color: #5c5c5c;
}
</style>
