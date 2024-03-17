<template>
  <div :id="'employee' + index" class="row employee">
    <div class="col-3">{{ employee.name }}</div>
    <div class="col-2">{{ employee.cardId }}</div>
    <div class="col-2">{{ employee.pin }}</div>
    <div class="col-2">{{ employee.admin }}</div>

    <div class="col-1">
      <div class="btn btn-warning" @click="editemployee()">EDYTUJ</div>
    </div>
    <div class="col-1">
      <div class="btn btn-danger" @click="deleteemployee()">USUŃ</div>
    </div>
  </div>
</template>
<script lang="ts">
import { defineComponent, ref } from "vue";
import { showModal, showDeleteModal } from "@/functions";
import { DeleteItemModel } from "@/types/DeleteItem";
import { POSITION, useToast } from "vue-toastification";

export default defineComponent({
  name: "OneLineComponent",
  props: {
    employee: Object,
    index: Number,
  },
  setup() {
    let edit = ref<boolean>(false);
    let item = ref<DeleteItemModel>({ id: 0, what: "test", name: "test" });
    const toast = useToast();

    return { edit, item, toast };
  },
  methods: {
    async editemployee() {
      showModal();
      this.$emit("getemployeeedited", this.employee);
    },
    async deleteemployee() {
      if (this.employee) {
        this.item.id = this.employee.id;
        this.item.what = "employee";
        this.item.name = this.employee.discountPercentage;
        showDeleteModal();
        this.$emit("getemployeedeleted", this.item);
      }
    },

    showModal,
    showDeleteModal,
    async colorBackground() {
      if (this.index) {
        if (this.index % 2 == 1) {
          let elem = document.getElementById(
            "employee" + this.index
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
.employee {
  min-height: 50px;
  line-height: 50px;
}
input {
  width: 100%;
}
</style>
