<template>
  <div :id="'promotion' + index" class="row promotion">
    <div class="col-4">
      <span v-for="(name, index2) in names" :key="index2"
        >{{ name }}<span v-if="index2 != names.length - 1">, </span></span
      >
    </div>
    <div class="col-2">{{ promotion.discountPercentage }}</div>
    <div class="col-2">{{ promotion.startDate }}</div>
    <div class="col-2">{{ promotion.endDate }}</div>
    <div class="col-1">
      <div class="btn btn-warning" @click="editpromotion()">EDYTUJ</div>
    </div>
    <div class="col-1">
      <div class="btn btn-danger" @click="deletepromotion()">USUŃ</div>
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
    promotion: Object,
    index: Number,
  },
  setup() {
    let edit = ref<boolean>(false);
    let item = ref<DeleteItemModel>({ id: 0, what: "test", name: "test" });
    let names = ref<string[]>([]);
    return { names, edit, item };
  },
  methods: {
    async editpromotion() {
      showModal();
      this.$emit("getpromotionedited", this.promotion);
    },
    async getItemsNames() {
      if (this.promotion) {
        if (this.promotion.affectedProducts) {
          for (let i = 0; this.promotion.affectedProducts.length > i; i++) {
            this.names.push(this.promotion.affectedProducts[i].name);
          }
        }
      }
    },
    async deletepromotion() {
      if (this.promotion) {
        this.item.id = this.promotion.id;
        this.item.what = "promotion";
        this.item.name = this.promotion.discountPercentage;
        showDeleteModal();
        this.$emit("getpromotiondeleted", this.item);
      }
    },
    showModal,
    showDeleteModal,
    async colorBackground() {
      if (this.index) {
        if (this.index % 2 == 1) {
          let elem = document.getElementById(
            "promotion" + this.index
          ) as HTMLElement;
          elem.style.backgroundColor = "#2c2c2c";
        }
      }
    },
  },
  mounted() {
    this.colorBackground();
    this.getItemsNames();
  },
});
</script>
<style scoped lang="scss">
.promotion {
  min-height: 50px;
  line-height: 50px;
}
input {
  width: 100%;
}
</style>
