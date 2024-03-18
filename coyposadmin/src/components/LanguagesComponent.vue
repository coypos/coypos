<template>
  <div :id="'language' + index" class="row language">
    <div class="col-3">{{ language.name }}</div>
    <div class="col-2">{{ language.countryCode }}</div>
    <div class="col-2">{{ language.enabled }}</div>

    <div class="col-1">
      <div class="btn btn-warning" @click="editlanguage()">EDYTUJ</div>
    </div>
    <div class="col-1">
      <div class="btn btn-danger" @click="deletelanguage()">USUŃ</div>
    </div>
  </div>
</template>
<script lang="ts">
import { defineComponent, ref } from "vue";
import { showModal, showDeleteModal } from "@/functions";
import { DeleteItemModel } from "@/types/DeleteItem";
import { useToast } from "vue-toastification";

export default defineComponent({
  name: "OneLineComponent",
  props: {
    language: Object,
    index: Number,
  },
  setup() {
    let edit = ref<boolean>(false);
    let item = ref<DeleteItemModel>({ id: 0, what: "test", name: "test" });
    const toast = useToast();

    return { edit, item, toast };
  },
  methods: {
    async editlanguage() {
      showModal();
      this.$emit("getlanguageedited", this.language);
    },
    async deletelanguage() {
      if (this.language) {
        this.item.id = this.language.id;
        this.item.what = "language";
        this.item.name = this.language.discountPercentage;
        showDeleteModal();
        this.$emit("getlanguagedeleted", this.item);
      }
    },

    showModal,
    showDeleteModal,
    async colorBackground() {
      if (this.index) {
        if (this.index % 2 == 1) {
          let elem = document.getElementById(
            "language" + this.index
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
.language {
  min-height: 50px;
  line-height: 50px;
}
input {
  width: 100%;
}
</style>
