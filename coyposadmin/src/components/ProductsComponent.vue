<template>
  <div :id="'product' + index" class="row product">
    <div class="col-3">{{ product.name }}</div>
    <div class="col-3">{{ product.category.name }}</div>
    <div class="col-2">{{ product.barcode }}</div>
    <div class="col-2">{{ product.price }}</div>

    <div class="col-1">
      <div class="btn btn-warning" @click="editproduct()">EDYTUJ</div>
    </div>
    <div class="col-1">
      <div class="btn btn-danger" @click="deleteproduct()">USUŃ</div>
    </div>
  </div>
</template>
<script lang="ts">
import { defineComponent, ref } from "vue";
import { showModal, showDeleteModal } from "@/functions";
import { DeleteItemModel } from "@/types/DeleteItem";

export default defineComponent({
  name: "OneLineComponent",
  props: {
    product: Object,
    index: Number,
  },
  setup() {
    let edit = ref<boolean>(false);
    let item = ref<DeleteItemModel>({ id: 0, what: "test", name: "test" });
    return { edit, item };
  },
  methods: {
    async editproduct() {
      showModal();
      this.$emit("getproductedited", this.product);
    },
    async deleteproduct() {
      if (this.product) {
        this.item.id = this.product.id;
        this.item.what = "product";
        this.item.name = this.product.name;
        showDeleteModal();
        this.$emit("getproductdeleted", this.item);
      }
    },
    showModal,
    showDeleteModal,
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
