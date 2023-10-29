<template>
  <div v-if="!edit" :id="'product' + index" class="row product">
    <div class="col-1">{{ product.name }}</div>
    <div class="col-1">{{ product.category.name }}</div>
    <div class="col-2">{{ product.barcode }}</div>
    <div class="col-1">{{ product.price }}</div>
    <div class="col-1">{{ product.weight }}</div>
    <div class="col-1">{{ product.image }}</div>
    <div class="col-1">{{ product.description }}</div>
    <div class="col-1">{{ product.isLoose }}</div>
    <div class="col-1">{{ product.enabled }}</div>
    <div class="col-1">
      <div class="btn btn-warning" @click="editView()">EDYTUJ</div>
    </div>
    <div class="col-1">
      <div class="btn btn-danger">USUŃ</div>
    </div>
  </div>
  <div v-if="edit" :id="'product' + index" class="row product">
    <div class="col-1"><input v-model="productEdit.name" /></div>
    <div class="col-1"><input v-model="productEdit.category.name" /></div>
    <div class="col-2"><input v-model="productEdit.barcode" /></div>
    <div class="col-1"><input v-model="productEdit.price" /></div>
    <div class="col-1"><input v-model="productEdit.weight" /></div>
    <div class="col-1"><input v-model="productEdit.image" /></div>
    <div class="col-1"><input v-model="productEdit.description" /></div>
    <div class="col-1">
      <input type="checkbox" v-model="productEdit.isLoose" />
    </div>
    <div class="col-1">
      <input type="checkbox" v-model="productEdit.enabled" />
    </div>
    <div class="col-1">
      <div class="btn btn-success">Zapisz</div>
    </div>
  </div>
</template>
<script lang="ts">
import { defineComponent, ref } from "vue";
import { ProductModel } from "@/types/api/Product";

export default defineComponent({
  name: "OneLineComponent",
  props: {
    product: Object,
    index: Number,
  },
  setup() {
    let productEdit = ref<ProductModel>();
    let edit = ref<boolean>(false);
    return { edit, productEdit };
  },
  methods: {
    async colorBackground() {
      if (this.index) {
        if (this.index % 2 == 1) {
          let elem = document.getElementById(
            "product" + this.index
          ) as HTMLElement;
          elem.style.backgroundColor = "#2c2c2c";
        }
      }
    },
    async editView() {
      this.edit = true;
    },
  },
  beforeCreate() {
    if (this.product) {
      this.productEdit = this.product as ProductModel;
    }
  },
  mounted() {
    this.colorBackground();
  },
});
</script>
<style scoped lang="scss">
.product {
  min-height: 50px;
  line-height: 50px;
}
input {
  width: 100%;
}
</style>
