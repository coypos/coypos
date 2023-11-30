<template>
  <div :id="'category' + index" class="row category">
    <div class="col-3">{{ category.name.split("|").toString() }}</div>
    <div v-if="category.parentCategory" class="col-3">
      {{ category.parentCategory.name }}
    </div>
    <div v-else class="col-3">Brak</div>
    <div class="col-3">{{ category.isVisible }}</div>

    <div class="col-1">
      <div class="btn btn-warning" @click="editcategory()">EDYTUJ</div>
    </div>
    <div class="col-1">
      <div class="btn btn-danger" @click="deletecategory()">USUŃ</div>
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
    category: Object,
    index: Number,
  },
  setup() {
    let edit = ref<boolean>(false);
    let item = ref<DeleteItemModel>({ id: 0, what: "test", name: "test" });
    return { edit, item };
  },
  methods: {
    async editcategory() {
      showModal();
      this.$emit("getcategoryedited", this.category);
    },
    async deletecategory() {
      if (this.category) {
        this.item.id = this.category.id;
        this.item.what = "category";
        this.item.name = this.category.name;
        showDeleteModal();
        this.$emit("getcategorydeleted", this.item);
      }
    },
    showModal,
    showDeleteModal,
    async colorBackground() {
      if (this.index) {
        if (this.index % 2 == 1) {
          let elem = document.getElementById(
            "category" + this.index
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
.category {
  min-height: 50px;
  line-height: 50px;
}
input {
  width: 100%;
}
</style>
