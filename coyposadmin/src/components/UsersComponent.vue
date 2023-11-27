<template>
  <div :id="'user' + index" class="row user">
    <div class="col-2">{{ user.name }}</div>
    <div class="col-1">{{ user.role }}</div>
    <div class="col-2">{{ user.cardNumber }}</div>
    <div class="col-2">{{ user.phoneNumber }}</div>
    <div class="col-2">{{ user.email }}</div>
    <div class="col-1">{{ user.points }}</div>
    <div class="col-1">
      <div class="btn btn-warning" @click="edituser()">EDYTUJ</div>
    </div>
    <div class="col-1">
      <div class="btn btn-danger" @click="deleteuser()">USUŃ</div>
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
    user: Object,
    index: Number,
  },
  setup() {
    let edit = ref<boolean>(false);
    let item = ref<DeleteItemModel>({ id: 0, what: "test", name: "test" });
    return { edit, item };
  },
  methods: {
    async edituser() {
      showModal();
      this.$emit("getuseredited", this.user);
    },
    async deleteuser() {
      if (this.user) {
        this.item.id = this.user.id;
        this.item.what = "user";
        this.item.name = this.user.discountPercentage;
        showDeleteModal();
        this.$emit("getuserdeleted", this.item);
      }
    },
    showModal,
    showDeleteModal,
    async colorBackground() {
      if (this.index) {
        if (this.index % 2 == 1) {
          let elem = document.getElementById(
            "user" + this.index
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
.user {
  min-height: 50px;
  line-height: 50px;
}
input {
  width: 100%;
}
</style>
