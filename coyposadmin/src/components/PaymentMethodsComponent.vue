<template>
  <div :id="'payment_method' + index" class="row payment_method">
    <div class="col-3">{{ payment_method.name }}</div>
    <div class="col-2">{{ payment_method.enabled }}</div>
    <div class="col-2">{{ payment_method.authData }}</div>

    <div class="col-1">
      <div class="btn btn-warning" @click="editpayment_method()">EDYTUJ</div>
    </div>
    <div class="col-1">
      <div class="btn btn-danger" @click="deletepayment_method()">USUŃ</div>
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
    payment_method: Object,
    index: Number,
  },
  setup() {
    let edit = ref<boolean>(false);
    let item = ref<DeleteItemModel>({ id: 0, what: "test", name: "test" });
    const toast = useToast();

    return { edit, item, toast };
  },
  methods: {
    async editpayment_method() {
      showModal();
      this.$emit("getpayment_methodedited", this.payment_method);
    },
    async deletepayment_method() {
      if (this.payment_method) {
        this.item.id = this.payment_method.id;
        this.item.what = "payment_method";
        this.item.name = this.payment_method.discountPercentage;
        showDeleteModal();
        this.$emit("getpayment_methoddeleted", this.item);
      }
    },

    showModal,
    showDeleteModal,
    async colorBackground() {
      if (this.index) {
        if (this.index % 2 == 1) {
          let elem = document.getElementById(
            "payment_method" + this.index
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
.payment_method {
  min-height: 50px;
  line-height: 50px;
}
input {
  width: 100%;
}
</style>
